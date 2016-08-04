using UnityEngine;
using System.Collections;
using System.IO;
using Util;

namespace UI
{
	/// <summary>
	/// Randomly plays a song from the title screen music folder
	/// </summary>
	public class UIMainMenuMusic : MonoBehaviour
	{
		public AudioSource source;

		void Start()
		{
			string[] titleScreenSongs = Directory.GetFiles(IOUtil.PathCombine(Application.dataPath, "music", "title"), "*.ogg");
			int songHandle = RandUtil.Int(titleScreenSongs.Length);
			StartCoroutine(IOUtil.LoadOGGIntoSource(titleScreenSongs[songHandle], source, true, true));
		}
	}
}