using System.IO;
using LSDR.Types;
using UnityEngine;
using UnityEngine.Audio;
using LSDR.Util;

namespace LSDR.Entities.Dream
{
	// TODO: refactor MusicController to be more reliable
	public class MusicController : MonoBehaviour
	{
		public string MusicDirectory;
		public bool UseSubDirectories;

		public AudioSource Source;

		public string[] songsInSelection;

		private static AudioMixer _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MusicController script = instantiated.AddComponent<MusicController>();

			script.Source = instantiated.AddComponent<AudioSource>();
			script.Source.loop = true;
			script.Source.spatialBlend = 0; // 2d audio
			script.Source.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("Music")[0];

			script.MusicDirectory = IOUtil.PathCombine("music", e.GetPropertyValue("Music directory"));

			script.UseSubDirectories = e.GetSpawnflagValue(0, 1);

			string selectedDir = IOUtil.PathCombine(Application.streamingAssetsPath, script.MusicDirectory);
			if (script.UseSubDirectories)
			{
				string[] dirsToChooseFrom = Directory.GetDirectories(IOUtil.PathCombine(Application.streamingAssetsPath, script.MusicDirectory));
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
