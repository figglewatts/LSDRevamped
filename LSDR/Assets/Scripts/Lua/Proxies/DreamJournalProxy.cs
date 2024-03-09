using System.Collections.Generic;
using LSDR.SDK.Data;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class DreamJournalProxy : AbstractLuaProxy<DreamJournal>
    {
        [MoonSharpHidden]
        public DreamJournalProxy(DreamJournal target) : base(target) { }

        public string Name => _target.Name;
        public string Author => _target.Author;
        public List<LSDR.SDK.Data.Dream> Dreams => _target.Dreams;

        public LSDR.SDK.Data.Dream GetLinkableDream(LSDR.SDK.Data.Dream current)
        {
            return _target.GetLinkable(current);
        }

        public LSDR.SDK.Data.Dream GetDreamFromGraph(int x, int y) => _target.GetDreamFromGraph(x, y);
    }
}
