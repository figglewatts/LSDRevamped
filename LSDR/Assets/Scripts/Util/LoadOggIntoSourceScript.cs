using UnityEngine;
using System.Collections;

namespace Util
{
	// TODO: is LoadOggIntoSourceScript redundant?
	public class LoadOggIntoSourceScript : MonoBehaviour
	{
		public AudioSource Source;
		public string FilePath;
		public bool PlayOnLoad;
		public bool AbsolutePath;

		// Use this for initialization
		void Start()
		{
			if (!Source || FilePath == null)
			{
				Debug.LogWarning("Field on LoadOggIntoSourceScript not set!");
				return;
			}
			StartCoroutine(IOUtil.LoadOGGIntoSource(FilePath, Source, PlayOnLoad, AbsolutePath));
		}
	}
}