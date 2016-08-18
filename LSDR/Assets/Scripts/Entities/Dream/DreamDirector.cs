using System;
using System.Collections.Generic;
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

		public static List<Vector2> GraphSquares = new List<Vector2>();

		public static List<Types.Dream> PriorDreams = new List<Types.Dream>();

		public static string CurrentlyPlayingSong = "";

		public static GameObject Player;

		public static List<PlayerSpawn> PlayerSpawns = new List<PlayerSpawn>();
		public static bool PlayerSpawnForced = false;
		public static int ForcedSpawnIndex = 0;

		public static int CurrentDay;

		public static string[] LevelPaths;

		public static DreamPayload Payload;

		public static event OnLevelFinishChangeHandler OnLevelFinishChange;

		// works like OnLevelFinishChange but is cleared before each level is loaded
		public static event OnLevelFinishChangeHandler PostLoadEvent;

		public static TextureSet CurrentTextureSet { get; private set; }

		public static bool CurrentlyInDream = false;

		public const float DREAM_MAX_TIME = 600;

		private static GameObject _loadedDreamObject;

		private static float _playerHeightOffset = 0.65F;

		// how much to affect the happiness value if the dream is ended by falling
		private const int FALLING_HAPPINESS_PENALTY = -2;

		public static void AddGraphSquare(int x, int y)
		{
			GraphSquares.Add(new Vector2(Mathf.Clamp(x, -9, 9), Mathf.Clamp(y, -9, 9)));
		}

		/// <summary>
		/// Used to remember prior dreams. Converts payload values to Dream struct and adds to list.
		/// </summary>
		public static void AddPayloadToPriorDreams(DreamPayload p)
		{
			Types.Dream d;
			d.LevelsVisited = p.LevelsVisited.ToArray();
			d.TimeInDream = p.TimeInDream;
			d.Seed = p.DreamSeed;
			PriorDreams.Add(d);
		}

		public static void BeginDream()
		{
			if (GraphSquares.Count > 0)
			{
				int xPos = (int) GraphSquares[GraphSquares.Count - 1].x;
				int yPos = (int) GraphSquares[GraphSquares.Count - 1].y;
				string level;
				if (GetLevelFromGraphPosition(xPos, yPos, out level))
				{
					BeginDream(level);
					return;
				}
			}
			BeginDream(RandUtil.RandomLevelFromDir(DreamJournalManager.CurrentJournal));
		}

		public static void BeginDream(string level)
		{
			if (CurrentlyInDream) return;

			CurrentlyInDream = true;

			RandUtil.RefreshSeed();

			StaticityAccumulator = 0;
			HappinessAccumulator = 0;

			string levelToLoad = level;

			RefreshTextureSet(true);

			// populate payload with textureset info and level to load
			Payload = GameObject.FindGameObjectWithTag("DreamPayload").GetComponent<DreamPayload>();
			Payload.DreamSeed = RandUtil.CurrentSeed;
			Payload.InitialLevelToLoad = levelToLoad;

			ResourceManager.ClearLifespan(ResourceLifespan.MENU);

			// load dream scene
			Fader.FadeIn(Color.black, 1.5F, () => { SceneManager.LoadScene("dream"); });
		}

		// TODO: write BeginFlashback method that sets the seed as well as starting a dream

		/// <summary>
		/// Ends the dream with the happiness penalty for falling
		/// </summary>
		public static void EndDreamFall()
		{
			if (!CurrentlyInDream) return;

			HappinessAccumulator += FALLING_HAPPINESS_PENALTY;

			EndDream(2F);
		}

		public static void EndDream(float fadeInSpeed = 5F)
		{
			if (!CurrentlyInDream) return;

			CurrentlyInDream = false;
		
			DreamCleanup();

			Payload.DreamEnded = true;

			AddGraphSquare(StaticityAccumulator, HappinessAccumulator);

			Fader.FadeIn(Color.black, fadeInSpeed, () =>
			{
				SceneManager.LoadScene("titlescreen");
				GameSettings.SetCursorViewState(true);
			});
		}

		/// <summary>
		/// Exits the dream without saving state.
		/// Used for quitting from pause menu.
		/// </summary>
		public static void ExitDream()
		{
			if (!CurrentlyInDream) return;

			CurrentlyInDream = false;
		
			DreamCleanup();

			Payload.ClearPayload();

			GameSettings.PauseGame(false);

			GameSettings.SetCursorViewState(true);

			Fader.FadeIn(Color.black, 1F, () =>
			{
				SceneManager.LoadScene("titlescreen");
				Fader.FadeOut(0.5F);
			});
		}

		public static void SwitchDreamLevel(string levelPath, string spawnPoint = "")
		{
			PostLoadEvent = null;
			
			if (_loadedDreamObject)
			{
				UnityEngine.Object.Destroy(_loadedDreamObject);
				ResourceManager.ClearLifespan(ResourceLifespan.LEVEL);
			}

			GameObject.FindGameObjectWithTag("EnvironmentController")
				.GetComponent<EnvironmentController>()
				.EnvironmentEntity = null;

			PlayerSpawns.Clear();
			PlayerSpawnForced = false;

			Target.Targets.Clear();
			ActionSequence.Sequences.Clear();
		
			TMAP t;
			_loadedDreamObject = IOUtil.LoadToriiMap(levelPath, ResourceLifespan.DREAM, out t);
			Payload.LevelsVisited.Add(t.Header.Name);

			if (PlayerSpawns.Count > 0)
			{
				int spawnPointHandle = 0;
				if (spawnPoint.Equals(string.Empty)) // random spawn point
				{
					spawnPointHandle = (PlayerSpawnForced || CurrentDay == 1)
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

			if (PostLoadEvent != null) PostLoadEvent.Invoke();
			if (OnLevelFinishChange != null) OnLevelFinishChange.Invoke();
		}

		public static void RefreshTextureSet(bool canUseCurrent)
		{
			TextureSet textureSet = (TextureSet)RandUtil.Int(1, 5);
			if (!canUseCurrent && textureSet == CurrentTextureSet)
			{
				RefreshTextureSet(false);
			}
			CurrentTextureSet = textureSet;
			Shader.SetGlobalInt("_TextureSet", (int)textureSet);
		}

		/// <summary>
		/// Called when a dream is ended, be it manually or automatically
		/// </summary>
		private static void DreamCleanup()
		{
			Fader.ClearHandler(); // clear all the junk from the post-fade event handler
			PlayerSpawns.Clear();
			Target.Targets.Clear();

			OnLevelFinishChange = null;
			PostLoadEvent = null;

			ResourceManager.ClearLifespan(ResourceLifespan.LEVEL);
			ResourceManager.ClearLifespan(ResourceLifespan.DREAM);

			Player = null;
		}

		private static Vector3 SetPlayerSpawn(Transform t)
		{
			return new Vector3(t.position.x, t.position.y + _playerHeightOffset,
					t.position.z);
		}

		private static bool GetLevelFromGraphPosition(int x, int y, out string level)
		{
			// 9 is squares to middle of graph, 19 is dimension of graph
			int actualX = x + 9;
			int actualY = Mathf.Abs((y + 9) - 18); // GetPixels32 is flipped

			int textureArrayIndex = (actualY*19) + actualX;

			try
			{
				Texture2D graphTexMap =
					ResourceManager.Load<Texture2D>(IOUtil.PathCombine("levels", DreamJournalManager.CurrentJournal + ".png"),
						ResourceLifespan.MENU);
				Color32[] colorTest = graphTexMap.GetPixels32();
				int levelIndex = graphTexMap.GetPixels32()[textureArrayIndex].a;

				level = IOUtil.GetLevelFromIndex(levelIndex, DreamJournalManager.CurrentJournal);

				return true;
			}
			catch (ResourceManager.ResourceLoadException e)
			{
				Debug.Log("Could not find graph spawn texture, defaulting to random spawn");
			}

			level = null;
			return false;
		}
	}
}
