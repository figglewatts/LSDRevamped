using System;
using System.Collections.Generic;
using System.IO;
using Game;
using Util;
using UnityEngine;

namespace InputManagement
{
	// TODO: handle scheme not found

	public static class ControlSchemeManager
	{
		public static List<ControlScheme> LoadedSchemes = new List<ControlScheme>();
		public static ControlScheme CurrentScheme
		{
			get { return LoadedSchemes[_currentSchemeHandle]; }
		}

		private static int _currentSchemeHandle;

		public static void Initialize()
		{
			LoadControlSchemes();
		}

		public static void AddScheme(ControlScheme scheme)
		{
			for (int i = 0; i < LoadedSchemes.Count; i++)
			{
				if (LoadedSchemes[i].SchemeName.Equals(scheme.SchemeName))
				{
					LoadedSchemes[i] = scheme;
					return;
				}
			}
			LoadedSchemes.Add(scheme);
		}

		public static void SwitchToScheme(string name)
		{
			for (int i = 0; i < LoadedSchemes.Count; i++)
			{
				if (LoadedSchemes[i].SchemeName.Equals(name))
				{
					SwitchToScheme(i);
				}
			}
		}

		public static void SwitchToScheme(int i)
		{
			_currentSchemeHandle = i;
			GameSettings.CurrentControlSchemeIndex = i;
			ControlSchemeSwitched();
		}

		public static void SerializeSchemes()
		{
			string pathToControlSchemes = IOUtil.PathCombine(Application.persistentDataPath, "Settings", "ControlSchemes");

			foreach (ControlScheme scheme in LoadedSchemes)
			{
				IOUtil.WriteJSONToDisk(scheme.SerializeToJSON(), IOUtil.PathCombine(pathToControlSchemes, scheme.SchemeName + ".json"));
			}
		}

		public static void LoadControlSchemes()
		{
			LoadedSchemes.Clear();
		
			string pathToControlSchemes = IOUtil.PathCombine(Application.persistentDataPath, "Settings", "ControlSchemes");

			string[] schemes;
			try
			{
				schemes = Directory.GetFiles(pathToControlSchemes);
			}
			catch (DirectoryNotFoundException e)
			{
				Debug.Log("Could not find controlschemes folder, should be first startup!");
				Directory.CreateDirectory(pathToControlSchemes);
				LoadedSchemes.Add(new ControlScheme());
				SerializeSchemes();
				return;
			}
			catch (IOException e)
			{
				Debug.LogException(e);
				return;
			}
			
			foreach (string schemeFile in schemes)
			{
				LoadedSchemes.Add(new ControlScheme(schemeFile));
			}
		}

		private static void ControlSchemeSwitched()
		{
			foreach (string inputName in CurrentScheme.Controls.Keys)
			{
				InputHandler.RebindInput(inputName, CurrentScheme.Controls[inputName]);
			}
		}
	}
}
