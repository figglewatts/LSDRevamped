using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Entities.Dream;
using UI;
using UnityEngine;
using Util;

namespace Game
{
	// TODO: Refactor DevConsole (also DreamDirector refactor)
	public static class DevConsole
	{
		public static UIDevConsole ConsoleUI;
	
		public static void ProcessConsoleCommand(string command)
		{
			List<string> commandFragments = SplitConsoleCommand(command);

			switch (commandFragments[0].ToLowerInvariant())
			{
				case "switchjournal":
				{
					DreamJournalManager.SwitchJournal(commandFragments[1]);
					break;
				}

				case "loadlevel":
				{
					string levelName = commandFragments[1];

					// if there is a journal specified switch to it
					if (commandFragments.Count > 2) DreamJournalManager.SwitchJournal(commandFragments[2]);

					string levelPath = IOUtil.PathCombine(Application.streamingAssetsPath, "levels", DreamJournalManager.CurrentJournal, levelName + ".tmap");

					// check if the level exists before doing anything
					if (!File.Exists(levelPath))
					{
						Debug.LogError("Level " + levelName + " does not exist");
						break;
					}

					// if we're not in a dream begin one with the specified level
					if (!DreamDirector.CurrentlyInDream)
					{
						DreamDirector.BeginDream(levelPath);
						ConsoleUI.SetConsoleState(false);
						break;
					}
					else
					{
						// otherwise just swap out the level for the specified one
						DreamDirector.SwitchDreamLevel(levelPath);
						break;
					}
				}

				case "textureset":
				{
					int set = int.Parse(commandFragments[1], CultureInfo.InvariantCulture);
					Shader.SetGlobalInt("_TextureSet", set);
					Debug.Log("Switched texture set to " + (TextureSet)set);
					break;
				}

				case "enddream":
				{
					DreamDirector.EndDream();
					break;
				}

				case "greyman":
				{
					if (!DreamDirector.CurrentlyInDream)
					{
						Debug.LogWarning("Not in dream!");
						break;
					}

					GreymanController c = GameObject.FindGameObjectWithTag("GreymanController").GetComponent<GreymanController>();
					c.SpawnGreyman();
					break;
				}

				default:
				{
					Debug.LogWarning("Did not recognize command: " + commandFragments[0]);
					break;
				}
			}
		}

		private static List<string> SplitConsoleCommand(string command)
		{
			return command.Split('"')
					 .Select((element, index) => index % 2 == 0  // If even index
										   ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
										   : new string[] { element })  // Keep the entire item
					 .SelectMany(element => element).ToList();
		}
	}
}
