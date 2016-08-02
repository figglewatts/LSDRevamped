using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.Dream
{
	// TODO: hook into music volume

	public class MusicController : MonoBehaviour
	{
		public string MusicDirectory;
		public bool UseSubDirectories;

		public AudioSource Source;

		public string[] songsInSelection;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MusicController script = instantiated.AddComponent<MusicController>();

			script.Source = instantiated.AddComponent<AudioSource>();
			script.Source.loop = true;

			script.MusicDirectory = IOUtil.PathCombine("music", e.GetPropertyValue("Music directory"));

			script.UseSubDirectories = e.GetSpawnflagValue(0, 1);

			string selectedDir = IOUtil.PathCombine(Application.dataPath, script.MusicDirectory);
			if (script.UseSubDirectories)
			{
				string[] dirsToChooseFrom = Directory.GetDirectories(IOUtil.PathCombine(Application.dataPath, script.MusicDirectory));
				selectedDir = RandUtil.RandomArrayElement(dirsToChooseFrom);
				Debug.Log("Selected " + selectedDir);
			}

			string[] tracksToChooseFrom = Directory.GetFiles(selectedDir, "*.ogg");
			string selectedTrack = RandUtil.RandomArrayElement(tracksToChooseFrom);

			Debug.Log("Chose track: " + selectedTrack);

			DreamDirector.CurrentlyPlayingSong = Path.GetFileNameWithoutExtension(selectedTrack);

			script.StartCoroutine(IOUtil.LoadOGGIntoSource(selectedTrack, script.Source, true, true));

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
