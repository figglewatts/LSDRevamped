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

		public static string CurrentlyPlayingSong = "";

		public static GameObject Player;

		public static List<PlayerSpawn> PlayerSpawns = new List<PlayerSpawn>();
		public static bool PlayerSpawnForced = false;
		public static int ForcedSpawnIndex = 0;

		public static int CurrentDay = 1;

		public static string[] LevelPaths;

		public static DreamPayload Payload;

		public static event OnLevelFinishChangeHandler OnLevelFinishChange;

		public static TextureSet CurrentTextureSet { get; private set; }

		public static bool CurrentlyInDream = false;

		public const float DREAM_MAX_TIME = 7;

		private static GameObject _loadedDreamObject;

		private static float _playerHeightOffset = 0.65F;

		// how much to affect the happiness value if the dream is ended by falling
		private const int FALLING_HAPPINESS_PENALTY = -2;

		public static void BeginDream()
		{
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

			ResourceManager.ClearLifespan(ResourceLifespan.LEVEL);
			ResourceManager.ClearLifespan(ResourceLifespan.DREAM);

			Player = null;
		}

		private static Vector3 SetPlayerSpawn(Transform t)
		{
			return new Vector3(t.position.x, t.position.y + _playerHeightOffset,
					t.position.z);
		}
	}
}
