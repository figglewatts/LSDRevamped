using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LSDR.Entities.Dream;
using LSDR.UI;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Game
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
