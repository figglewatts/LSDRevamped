using System;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua
{
    public interface ILuaEngine
    {
        Script CreateBaseAPI();
        void RegisterGlobalObject(object obj, string alias = "");
        void RegisterEnum<T>() where T : Enum;
    }
}
