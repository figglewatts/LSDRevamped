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
	}
}