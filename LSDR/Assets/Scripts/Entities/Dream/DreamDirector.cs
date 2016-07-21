using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entities.Dream
{
	public static class DreamDirector
	{
		public static int StaticityAccumulator = 0;
		public static int HappinessAccumulator = 0;

		public static void BeginDream()
		{
			StaticityAccumulator = 0;
			HappinessAccumulator = 0;

			// randomly generate level to load with seed
			// randomly pick texture set with seed

			// TODO: temporary
			string levelToLoad = "levels/testDream.tmap";
			int textureSet = 1;

			// populate payload with textureset info and level to load
			DreamPayload payload = GameObject.FindGameObjectWithTag("DreamPayload").GetComponent<DreamPayload>();
			payload.LevelToLoad = levelToLoad;
			payload.TextureSetIndex = textureSet;

			// load dream scene
			SceneManager.LoadScene("dream");
		}
	}
}
