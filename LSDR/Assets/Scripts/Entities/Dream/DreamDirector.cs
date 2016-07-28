using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Action;
using Entities.Player;
using Entities.WorldObject;
using Game;
using Types;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Entities.Dream
{
	public delegate void OnLevelFinishChangeHandler();

	public static class DreamDirector
	{
		public static int StaticityAccumulator = 0;
		public static int HappinessAccumulator = 0;

		public static GameObject Player;

		public static List<PlayerSpawn> PlayerSpawns = new List<PlayerSpawn>();
		public static bool PlayerSpawnForced = false;
		public static int ForcedSpawnIndex = 0;

		public static int CurrentDay = 1;

		public static string[] LevelPaths;

		public static DreamPayload Payload;

		public static event OnLevelFinishChangeHandler OnLevelFinishChange;

		private static GameObject _loadedDreamObject;

		private static float _playerHeightOffset = 0.6F;

		public static void BeginDream()
		{
			RandUtil.RefreshSeed();
			// TODO: save this seed somewhere for flashbacks

			StaticityAccumulator = 0;
			HappinessAccumulator = 0;

			// TODO: randomly generate level to load with seed
			// TODO: randomly pick texture set with seed

			// TODO: temporary
			string levelToLoad = RandUtil.RandomLevelFromDir("/");
			Debug.Log(levelToLoad);
			int textureSet = 2;

			// populate payload with textureset info and level to load
			Payload = GameObject.FindGameObjectWithTag("DreamPayload").GetComponent<DreamPayload>();
			Payload.InitialLevelToLoad = levelToLoad;
			Payload.InitialTextureSetIndex = textureSet;

			Shader.SetGlobalInt("_TextureSet", Payload.InitialTextureSetIndex);

			// load dream scene
			Fader.FadeIn(Color.black, 1.5F, () => {SceneManager.LoadScene("dream");});
		}

		// TODO: write BeginFlashback method that sets the seed as well as starting a dream

		public static void EndDream()
		{
			Fader.ClearHandler(); // clear all the junk from the post-fade event handler
			PlayerSpawns.Clear();
			Target.Targets.Clear();

			ResourceManager.ClearLifespan(ResourceLifespan.LEVEL);
			ResourceManager.ClearLifespan(ResourceLifespan.DREAM);
		
			// TODO: end dream stuff

			Player = null;
		}

		public static void SwitchDreamLevel(string levelPath, string spawnPoint = "")
		{
			OnLevelFinishChange = null; // clear prior events
			
			if (_loadedDreamObject)
			{
				UnityEngine.Object.Destroy(_loadedDreamObject);
				ResourceManager.ClearLifespan(ResourceLifespan.LEVEL);
			}

			PlayerSpawns.Clear();
			PlayerSpawnForced = false;

			Target.Targets.Clear();
			ActionSequence.Sequences.Clear();
		
			TMAP t;
			_loadedDreamObject = IOUtil.LoadToriiMap(levelPath, ResourceLifespan.DREAM, out t);
			Payload.LevelsVisited.Add(t.Header.Name);
			Payload.LevelsVisitedPreviews.Add(IOUtil.LoadPNGByteArray(t.Header.Preview));

			if (PlayerSpawns.Count > 0)
			{
				int spawnPointHandle = 0;
				if (spawnPoint.Equals(string.Empty)) // random spawn point
				{
					spawnPointHandle = (PlayerSpawnForced && CurrentDay == 1)
						? ForcedSpawnIndex
						: RandUtil.Int(0, PlayerSpawns.Count);
				}
				else
				{
					// named spawn point
					for (int i = 0; i < PlayerSpawns.Count; i++)
					{
						if (PlayerSpawns[i].Name.Equals(spawnPoint))
						{
							spawnPointHandle = i;
						}
					}
				}
				Vector3 spawnPos = SetPlayerSpawn(PlayerSpawns[spawnPointHandle].transform);
				
				Player.transform.position = spawnPos;
				Player.transform.rotation = Quaternion.Euler(0, PlayerSpawns[spawnPointHandle].transform.rotation.eulerAngles.y, 0);
			}
			else Debug.LogError("There are no player_spawn entities in the level, cannot spawn player!");

			if (OnLevelFinishChange != null) OnLevelFinishChange.Invoke();
		}

		private static Vector3 SetPlayerSpawn(Transform t)
		{
			return new Vector3(t.position.x, t.position.y + _playerHeightOffset,
					t.position.z);
		}
	}
}
