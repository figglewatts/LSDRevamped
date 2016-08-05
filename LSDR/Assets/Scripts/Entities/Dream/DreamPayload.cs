using UnityEngine;
using System.Collections.Generic;

namespace Entities.Dream
{
	public class DreamPayload : MonoBehaviour
	{
		public int DreamSeed;
		public string InitialLevelToLoad;
		public List<string> LevelsVisited;
		public List<Texture2D> LevelsVisitedPreviews;

		public void ClearPayload()
		{
			DreamSeed = 0;
			InitialLevelToLoad = string.Empty;
			LevelsVisited.Clear();
			LevelsVisitedPreviews.Clear();
		}
	}
}