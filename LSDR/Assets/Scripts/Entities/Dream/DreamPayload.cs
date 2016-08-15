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
		public float TimeInDream { get { return _dreamTimer; } }

		// did the dream end by timer or falling?
		// a flag that, when set, shows the graph on main menu payload receive
		// it gets set in the DreamDirector.EndDream() method
		public bool DreamEnded;

		private float _dreamTimer = 0;

		public void Update()
		{
			if (DreamDirector.CurrentlyInDream) _dreamTimer += Time.deltaTime;

			if (_dreamTimer > DreamDirector.DREAM_MAX_TIME) DreamDirector.EndDream();
		}

		public void ClearPayload()
		{
			DreamSeed = 0;
			InitialLevelToLoad = string.Empty;
			LevelsVisited.Clear();
			LevelsVisitedPreviews.Clear();
			DreamEnded = false;
			_dreamTimer = 0;
		}
	}
}