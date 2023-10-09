using LSDR.SDK.Assets;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.SDK.Lua
{
    public abstract class AbstractStartUpdateLuaScript : AbstractLuaScript
    {
        public const string START_FUNCTION_NAME = "start";
        public const string UPDATE_FUNCTION_NAME = "update";

        private DynValue _startFunc;
        private DynValue _updateFunc;

        protected AbstractStartUpdateLuaScript(ILuaEngine engine, LuaScriptAsset script) : base(engine, script) { }

        public void Start()
        {
            if (_startFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_startFunc); });
        }

        public void Update()
        {
            if (_updateFunc.IsNil()) return;
            _scriptAsset.HandleLuaErrorsFor(() => { Script.Call(_updateFunc); });
        }

        protected override void compile()
        {
            base.compile();
            loadStartUpdateFunctions();
        }

        private void loadStartUpdateFunctions()
        {
            _startFunc = Script.Globals.Get(START_FUNCTION_NAME);
            _updateFunc = Script.Globals.Get(UPDATE_FUNCTION_NAME);
        }
    }
}
