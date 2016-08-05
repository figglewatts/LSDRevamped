using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Util;

namespace Game
{
	// TODO: handle journal not found

	public static class DreamJournalManager
	{
		public static List<string> LoadedJournals = new List<string>();

		public static string CurrentJournal
		{
			get
			{
				return _initialized ? LoadedJournals[_currentJournalHandle] : "";
			}
		}

		public static void SwitchJournal(string name)
		{
			for (int i = 0; i < LoadedJournals.Count; i++)
			{
				if (LoadedJournals[i].Equals(name))
				{
					SwitchJournal(i);
					return;
				}
			}
			Debug.LogWarning("Could not find journal: " + name);
		}

		public static void SwitchJournal(int i)
		{
			_currentJournalHandle = i;
			Debug.Log("Journal switched to " + CurrentJournal);
			GameSettings.CurrentJournalIndex = i;
		}

		private static int _currentJournalHandle = 0;
		private static bool _initialized;

		public static void Initialize()
		{
			LoadJournals();
			_initialized = true;
		}

		public static void LoadJournals()
		{
			foreach (string dir in Directory.GetDirectories(IOUtil.PathCombine(Application.dataPath, "levels")))
			{
				LoadedJournals.Add(Path.GetFileNameWithoutExtension(dir));
			}
		}
	}
}
