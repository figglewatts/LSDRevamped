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

        /// <summary>
        /// Sets the dream journal without loading data.
        /// Only use this for initial game start, as data has
        /// already been loaded.
        /// </summary>
        /// <param name="i">The index of the journal.</param>
	    public static void SetJournal(int i)
        {
            _currentJournalHandle = i;
            Debug.Log("Journal switched to " + CurrentJournal);
            GameSettings.CurrentSettings.CurrentJournalIndex = i;
        }

        /// <summary>
        /// The same as SetJournal() but also loads game data.
        /// To be used for when the user switches journals.
        /// </summary>
        /// <param name="i">The index of the journal.</param>
		public static void SwitchJournal(int i)
		{
			SetJournal(i);
			//SaveGameManager.LoadGame();
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
			foreach (string dir in Directory.GetDirectories(IOUtil.PathCombine(Application.streamingAssetsPath, "levels")))
			{
				LoadedJournals.Add(Path.GetFileNameWithoutExtension(dir));
			}
		}
	}
}
