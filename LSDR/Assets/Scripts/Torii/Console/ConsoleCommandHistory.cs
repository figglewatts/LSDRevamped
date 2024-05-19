using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Torii.Console
{
    public class ConsoleCommandHistory
    {
        private const string HISTORY_FILE = ".cmd_history";

        private string _historyFilePath => Path.Combine(Application.persistentDataPath, HISTORY_FILE);

        private List<string> _history;

        public ConsoleCommandHistory()
        {
            createOrLoad();
        }

        public List<string> History => _history;

        public void StoreCommand(string command)
        {
            File.AppendAllText(_historyFilePath, $"{command}\n");
        }

        private void createOrLoad()
        {
            if (!File.Exists(_historyFilePath))
            {
                File.Create(_historyFilePath).Close();
            }
            _history = loadFileHistory();
        }

        private List<string> loadFileHistory()
        {
            return File.ReadAllLines(_historyFilePath).Reverse().ToList();
        }
    }
}
