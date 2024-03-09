using LSDR.SDK.Assets;
using LSDR.SDK.Data;

namespace LSDR.SDK.Lua
{
    public class JournalLuaScript : AbstractStartUpdateLuaScript
    {
        protected readonly DreamJournal _journal;

        public JournalLuaScript(ILuaEngine engine, LuaScriptAsset script, DreamJournal journal) : base(engine, script)
        {
            _journal = journal;
            Script.Globals["this"] = _journal;
            compile();
        }
    }
}
