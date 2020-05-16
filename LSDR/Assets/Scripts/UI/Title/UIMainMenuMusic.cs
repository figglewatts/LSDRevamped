using UnityEngine;
using System.IO;
using LSDR.Util;
using Torii.Audio;
using Torii.Resource;
using Torii.Util;

namespace LSDR.UI.Title
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
			string[] titleScreenSongs =
				Directory.GetFiles(PathUtil.Combine(Application.streamingAssetsPath, "music", "title"), "*.ogg");
			int songHandle = RandUtil.Int(titleScreenSongs.Length);
			var toriiAudioClip = ResourceManager.Load<ToriiAudioClip>(titleScreenSongs[songHandle], "global");
			source.clip = toriiAudioClip.Clip;
			source.loop = true;
			source.Play();
		}
	}
}