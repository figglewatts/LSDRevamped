using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Dream;
using SimpleJSON;
using Types;
using UnityEngine;
using Util;

namespace Game
{
	// TODO: redo all of this
	public static class SaveGameManager
	{
		private static JSONClass _gameState;
	
		public static string SaveGamePath
		{
			get
			{
				return IOUtil.PathCombine(Application.persistentDataPath, "saves", DreamJournalManager.CurrentJournal + ".json");
			}
		}

		public static void SaveCurrentGame()
		{
			if (_gameState == null) _gameState = new JSONClass(); // new savefile
		
			_gameState["dayNumber"].AsInt = DreamDirector.CurrentDay;

			int dreamIndex = _gameState["dayNumber"].AsInt - 2; // -1 for zero index, -1 for previous day

			_gameState["graphSquares"][dreamIndex]["x"].AsInt = (int) DreamDirector.GraphSquares[dreamIndex].x;
			_gameState["graphSquares"][dreamIndex]["y"].AsInt = (int) DreamDirector.GraphSquares[dreamIndex].y;

			AppendPayload(DreamDirector.Payload, dreamIndex, ref _gameState);

			IOUtil.WriteJSONToDisk(_gameState, SaveGamePath);
		}

		/*
		public static void LoadGame()
		{
			Debug.Log("SaveGameManager: Loading game");
		
			try
			{
				_gameState = ResourceManager.Load<JSONClass>(SaveGamePath, ResourceLifespan.GLOBAL, true);

				DreamDirector.CurrentDay = _gameState["dayNumber"].AsInt;

				DreamDirector.GraphSquares.Clear();
				foreach (JSONClass square in _gameState["graphSquares"].AsArray)
				{
					DreamDirector.AddGraphSquare(square["x"].AsInt, square["y"].AsInt);
				}

				// load prior dreams
				foreach (JSONClass dream in _gameState["previousDreams"].AsArray)
				{
					Dream d;
					d.LevelsVisited = new string[dream["levelsVisited"].AsArray.Count];
					for (int i = 0; i < d.LevelsVisited.Length; i++)
					{
						d.LevelsVisited[i] = dream["levelsVisited"][i];
					}
					d.TimeInDream = dream["timeInDream"].AsFloat;
					d.Seed = dream["dreamSeed"].AsInt;
					DreamDirector.PriorDreams.Add(d);
				}
			}
			catch (ResourceManager.ResourceLoadException e)
			{
				// must be first startup, ignore
				Debug.Log("Could not find savefile, assuming new game started");
				DreamDirector.CurrentDay = 1;
			}
		}
	*/
		private static void AppendPayload(DreamPayload p, int index, ref JSONClass savedGame)
		{
			for (int j = 0; j < p.LevelsVisited.Count; j++)
			{
				savedGame["previousDreams"][index]["levelsVisited"][j] = p.LevelsVisited[j];
			}
			savedGame["previousDreams"][index]["timeInDream"].AsFloat = p.TimeInDream;
			savedGame["previousDreams"][index]["dreamSeed"].AsInt = p.DreamSeed;

			
		}
	}
}
