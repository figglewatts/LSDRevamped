using UnityEngine;
using System.Collections;

namespace AutoUpdate
{
	public class UpdateChecker : MonoBehaviour
	{
		public string PlatformIdentifier;
		public string ClientIdentifier;

		public const string BUILDSERVER = "version.figglewatts.co.uk"; // where the builds are located
		public const string FILESERVER = "version.figglewatts.co.uk"; // where to download the patches from

		public GameObject UpdateDialogPrefab;
		public GameObject ErrorDialogPrefab;

		// Use this for initialization
		void Start()
		{
			SetPlatformIdentifier();
			CheckForGameUpdate(BuildNumber.Get());
		}

		/// <summary>
		/// Starts the coroutine that checks the server for an update
		/// </summary>
		/// <param name="buildNum">The client build number</param>
		public void CheckForGameUpdate(int buildNum)
		{
			if (string.IsNullOrEmpty(ClientIdentifier))
			{
				Debug.LogError("Cannot check for update as ClientIdentifier is null or empty");
				return;
			}
			if (string.IsNullOrEmpty(PlatformIdentifier))
			{
				Debug.LogError("Cannot check for update as PlatformIdentifier is null or empty");
				return;
			}
			StartCoroutine(BeginUpdateProcess(buildNum));
		}

		/// <summary>
		/// Asks the server if there are any updates available, and if there are starts downloading them.
		/// </summary>
		/// <param name="buildNum">The client build number</param>
		private IEnumerator BeginUpdateProcess(int buildNum)
		{
			// construct the URL query string
			string client = "?client=" + ClientIdentifier;
			string platform = "&platform=" + PlatformIdentifier;
			string build = "&build=" + buildNum.ToString();

			// contact the server and download it's response
			WWW www = new WWW(BUILDSERVER + client + platform + build);
			yield return www;

			// if there is an update available create the updater script and set its state
			string[] updateStrings;
			if (ProcessUpdateString(www.text, out updateStrings))
			{
				// create updating script (for the download) and pass relevant info to it
				this.gameObject.AddComponent<UpdaterScript>();
				UpdaterScript updater = this.gameObject.GetComponent<UpdaterScript>();
				updater.ServerVersion = int.Parse(updateStrings[1]);
				updater.NumberOfPatchesToDownload = updateStrings.Length - 2; // -2 due to values 0 and 1 in array not being patch numbers
				updater.platformIdentifier = PlatformIdentifier;
				updater.clientIdentifier = ClientIdentifier;
				int[] patchNumbers = new int[updater.NumberOfPatchesToDownload];
				for (int i = 0; i < patchNumbers.Length; i++)
				{
					patchNumbers[i] = int.Parse(updateStrings[i + 2]);
				}
				updater.patchNumbers = patchNumbers;

				// set updatedialog in updaterscript to instantiated dialog
				updater.UpdateDialog = Instantiate(UpdateDialogPrefab).GetComponent<UpdateMenu>();
				updater.UpdateDialog.transform.SetParent(GameObject.FindGameObjectWithTag("UpdateDialogContainer").transform, false);

				updater.ErrorDialogPrefab = ErrorDialogPrefab;
			}
		}

		/// <summary>
		/// Returns true if the game needs to update
		/// </summary>
		/// <param name="updateString">The raw string obtained from the build server</param>
		/// <param name="updateStrings">The processed strings, ready to be analysed</param>
		private bool ProcessUpdateString(string updateString, out string[] updateStrings)
		{
			// update string comes in format:
			// (index: explanation)
			// 0: a value that tells us whether to update or not, 1 if we need to update
			// 1: the latest build number the server has, so the server's version
			// 2: the oldest build number to download the manifest and patch for that is still newer than the client version
			// 3: the second oldest build
			// 4: and so on
			updateStrings = updateString.Split(new char[] {' '});
			return updateStrings[0] == "1";
		}

		/// <summary>
		/// Simply determines the client platform.
		/// </summary>
		private void SetPlatformIdentifier()
		{
			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				{
					PlatformIdentifier = "win";
				}
					break;
				case RuntimePlatform.LinuxPlayer:
				{
					PlatformIdentifier = "lin";
				}
					break;
				case RuntimePlatform.OSXPlayer:
				{
					PlatformIdentifier = "mac";
				}
					break;
			}
		}
	}
}
