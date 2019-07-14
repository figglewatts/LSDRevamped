using UnityEngine;
using System.IO;
using LSDR.Util;

namespace LSDR.UI
{
	/// <summary>
	/// Randomly plays a song from the title screen music folder.
	/// TODO: refactor UIMainMenuMusic to define these songs somewhere? Instead of just having them in a folder
	/// </summary>
	public class UIMainMenuMusic : MonoBehaviour
	{
		public AudioSource source;

		void Start()
		{
			string[] titleScreenSongs = Directory.GetFiles(IOUtil.PathCombine(Application.streamingAssetsPath, "music", "title"), "*.ogg");
			int songHandle = RandUtil.Int(titleScreenSongs.Length);
			StartCoroutine(IOUtil.LoadOGGIntoSource(titleScreenSongs[songHandle], source, true, true));
		}
	}
}