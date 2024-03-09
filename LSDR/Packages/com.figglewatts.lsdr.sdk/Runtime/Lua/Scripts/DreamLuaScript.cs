using LSDR.SDK.Assets;
using LSDR.SDK.Data;

namespace LSDR.SDK.Lua
{
    public class DreamLuaScript : AbstractStartUpdateLuaScript
    {
        protected Dream _dream;

        public DreamLuaScript(ILuaEngine engine, LuaScriptAsset script, Dream dream) : base(engine, script)
        {
            _dream = dream;
            Script.Globals["this"] = dream;
            compile();
        }
    }
}
