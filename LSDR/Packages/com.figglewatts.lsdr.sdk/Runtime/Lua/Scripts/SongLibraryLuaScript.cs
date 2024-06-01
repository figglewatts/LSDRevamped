using LSDR.SDK.Assets;
using LSDR.SDK.Audio;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public sealed class SongLibraryLuaScript : AbstractLuaScript
    {
        public const string GETSONG_FUNCTION_NAME = "getSong";

        protected readonly SongListAsset _songList;
        protected DynValue _getSongFunc;

        public SongLibraryLuaScript(ILuaEngine engine, LuaScriptAsset script, SongListAsset songList) : base(engine,
            script)
        {
            _songList = songList;
            Script.Globals["songList"] = _songList;
            compile();
            loadFunctions();
        }

        public SongAsset GetSong(SongStyle style, int songNumber)
        {
            return _scriptAsset.HandleLuaErrorsFor(() =>
            {
                var result = Script.Call(_getSongFunc, style, songNumber);
                if (result.IsNil()) return null;
                return (SongAsset)result.UserData.Object;
            });
        }

        private void loadFunctions()
        {
            _getSongFunc = Script.Globals.Get(GETSONG_FUNCTION_NAME);
        }
    }
}
