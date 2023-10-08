using System.Linq;
using LSDR.SDK.Data;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using UnityEngine;

namespace LSDR.Lua
{
    public class JournalScriptLoader : ScriptLoaderBase
    {
        public DreamJournal Journal { get; protected set; }

        public JournalScriptLoader(DreamJournal journal)
        {
            Journal = journal;
            ModulePaths = new[] { "?" };
        }

        public override bool ScriptFileExists(string name)
        {
            return Journal.LuaScriptIncludes.FirstOrDefault(s => s.name == name) != null;
        }

        public override object LoadFile(string file, Table globalContext)
        {
            var script = Journal.LuaScriptIncludes.FirstOrDefault(s => s.name == file);
            if (script != null) return script.ScriptText;

            Debug.LogError($"unable to load Lua script {file}");
            return null;
        }
    }
}
