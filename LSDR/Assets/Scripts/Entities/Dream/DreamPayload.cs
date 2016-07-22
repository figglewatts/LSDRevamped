using UnityEngine;
using System.Collections.Generic;

namespace Entities.Dream
{
	public class DreamPayload : MonoBehaviour
	{
		public string InitialLevelToLoad;
		public int InitialTextureSetIndex;
		public List<string> LevelsVisited;
		public List<Texture2D> LevelsVisitedPreviews;
	}
}