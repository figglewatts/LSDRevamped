using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;
using Ionic.Zip;
using Util;

namespace AutoUpdate
{
	public class UpdaterScript : MonoBehaviour
	{
		public int ServerVersion;
		public int NumberOfPatchesToDownload;
		public string platformIdentifier;
		public string clientIdentifier;
		public int[] patchNumbers;

		public UpdateMenu UpdateDialog;

		public GameObject ErrorDialogPrefab;

		private int patchIndex;
		private bool ErrorOccurred;

		private int NumberOfPatchesDownloaded;
		private int CurrentPatchBuildNumber;
		private bool FinishedPatching;
		private string StatusMessage;

		/// <summary>
		/// For each patch available start the coroutine that downloads it and wait for it to complete.
		/// </summary>
		public IEnumerator DownloadPatches()
		{
			Debug.Log("Beginning patch process...");

			CreateTempDirectory();

			patchIndex = 1;
			FinishedPatching = false;
			ErrorOccurred = false;

			// loop through the patches and download and apply each one
			foreach (int patchNumber in patchNumbers)
			{
				StatusMessage = "Downloading patch " + patchIndex + " of " + patchNumbers.Length + "...";

				yield return StartCoroutine(DownloadArchive(resolvePatchDir(platformIdentifier, clientIdentifier), patchNumber));

				if (ErrorOccurred)
				{
					UpdateDialog.gameObject.SetActive(false);
					GameObject dialog = Instantiate(ErrorDialogPrefab);
					dialog.transform.SetParent(GameObject.FindGameObjectWithTag("UpdateDialogContainer").transform, false);

					break;
				}
			}

			UpdateDialog.updateInstallContainer.SetActive(false);
			UpdateDialog.successfulInstallContainer.SetActive(true);

			DeleteTempDirectory();
		}

		/// <summary>
		/// Download the zip file that contains the patch.
		/// </summary>
		/// <param name="patchDir">Where the patches are stored on the server</param>
		/// <param name="buildNumber">The client build number</param>
		private IEnumerator DownloadArchive(string patchDir, int buildNumber)
		{
			// start the archive download and wait for it to complete
			WWW archiveDownload = new WWW(resolveArchiveUrl(patchDir, buildNumber));
			while (!archiveDownload.isDone)
			{
				UpdateDialog.downloadSlider.value = archiveDownload.progress; //  update download progress slider
				yield return null;
			}
			if (!string.IsNullOrEmpty(archiveDownload.error))
			{
				// something went wrong with the archive download
				Debug.LogError("Could not download patch archive");
				Debug.LogError("WWW error: " + archiveDownload.error);
				ErrorOccurred = true;
				yield break;
			}
			else
			{
				byte[] data = archiveDownload.bytes;
				try
				{
					File.WriteAllBytes(tempDir() + "/" + buildNumber.ToString() + ".zip", data);
				}
				catch (IOException e)
				{
					Debug.LogException(e);
					ErrorOccurred = true;
					yield break;
				}

				yield return StartCoroutine(DownloadManifest(patchDir, buildNumber));
			}
		}

		/// <summary>
		/// Download the patch manifest that contains a textual diff of the prior and current version.
		/// </summary>
		/// <param name="patchDir">Where the patches are stored on the server</param>
		/// <param name="buildNumber">The client build number</param>
		private IEnumerator DownloadManifest(string patchDir, int buildNumber)
		{
			// start the manifest download and wait for it to complete
			WWW manifestDownload = new WWW(resolveManifestUrl(patchDir, buildNumber));
			while (!manifestDownload.isDone)
			{
				yield return null;
			}
			if (!string.IsNullOrEmpty(manifestDownload.error))
			{
				// something went wrong with downloading the manifest
				Debug.LogError("Could not download patch manifest");
				Debug.LogError("WWW error: " + manifestDownload.error);
				ErrorOccurred = true;
				yield break;
			}
			else
			{
				byte[] data = manifestDownload.bytes;
				try
				{
					File.WriteAllBytes(tempDir() + "/" + buildNumber.ToString() + ".json", data);
				}
				catch (IOException e)
				{
					Debug.LogException(e);
					ErrorOccurred = true;
					yield break;
				}

				if (!InstallPatch(buildNumber))
				{
					ErrorOccurred = true;
					yield break;
				}

				patchIndex++;
				NumberOfPatchesDownloaded++;

				// update patch progress bar
				UpdateDialog.patchSlider.value = ((float) NumberOfPatchesDownloaded/(float) NumberOfPatchesToDownload);
			}
		}

		/// <summary>
		/// Installs the patch by unzipping it into the game's directory
		/// </summary>
		/// <param name="patchNumber">The build number of the patch</param>
		private bool InstallPatch(int patchNumber)
		{
			StatusMessage = "Installing patch number " + patchNumber.ToString();
			if (!RemoveFilesUsingManifest(patchNumber)) // first remove deleted files
			{
				return false; // an error has occurred when removing files
			}

			// then install the patch by extracting the archive into the game's directory
			using (ZipFile archive = ZipFile.Read(tempDir() + "/" + patchNumber + ".zip"))
			{
				foreach (ZipEntry e in archive)
				{
					List<string> extractedFiles = new List<string>();
					try
					{
						e.Extract(Path.Combine(Application.dataPath, "../"), ExtractExistingFileAction.OverwriteSilently);
						extractedFiles.Add(Path.Combine(Application.dataPath, "../") + Path.DirectorySeparatorChar + e.FileName);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);

						// undo the patch installation
						foreach (string file in extractedFiles)
						{
							File.Delete(file);
						}

						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Deletes files that are no longer present in the updated version of the game.
		/// </summary>
		/// <param name="patchNumber">The build number of the patch</param>
		private bool RemoveFilesUsingManifest(int patchNumber)
		{
			string manifestText = File.ReadAllText(tempDir() + "/" + patchNumber.ToString() + ".json");
			JSONNode manifestJson = JSON.Parse(manifestText);
			JSONArray removedFiles = manifestJson["removedFiles"].AsArray;

			Dictionary<string, byte[]> deletedFiles = new Dictionary<string, byte[]>();

			foreach (JSONNode node in removedFiles)
			{
				// if we have no removed files
				if (node == "" || ((string) node).Contains("output_log")) // skip output log or we'll get sharing violations
				{
					break;
				}

				// delete the file with path node.ToString() from filesystem
				try
				{
					// optimization possible here by simply moving the files to the temp directory then deleting it when done
					// if any files need to be restored, they can just be moved out of the temp directory back into the game directory
					deletedFiles.Add(Path.Combine(Application.dataPath, "../") + node,
						File.ReadAllBytes(Path.Combine(Application.dataPath, "../") + node));
					File.Delete(Path.Combine(Application.dataPath, "../") + node);
				}
				catch (IOException e)
				{
					Debug.LogException(e);

					// restore any deleted files
					foreach (string path in deletedFiles.Keys)
					{
						File.WriteAllBytes(path, deletedFiles[path]);
					}

					return false;
				}
			}
			return true;
		}

		private string resolvePatchDir(string platformIdentifier, string clientIdentifier)
		{
			return IOUtil.PathCombine(UpdateChecker.FILESERVER, "patchstore", clientIdentifier, platformIdentifier);
		}

		private string resolveArchiveUrl(string patchDir, int patchNumber)
		{
			return IOUtil.PathCombine(patchDir, patchNumber.ToString() + ".zip");
		}

		private string resolveManifestUrl(string patchDir, int patchNumber)
		{
			return IOUtil.PathCombine(patchDir,patchNumber.ToString() + ".json");
		}

		private void CreateTempDirectory()
		{
			Directory.CreateDirectory(tempDir());
		}

		private void DeleteTempDirectory()
		{
			Directory.Delete(tempDir(), true);
		}

		private string tempDir()
		{
			return IOUtil.PathCombine(Application.persistentDataPath, "updatetemp");
		}
	}
}