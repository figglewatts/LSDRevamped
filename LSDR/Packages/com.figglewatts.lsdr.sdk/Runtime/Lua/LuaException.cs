using System;

namespace LSDR.SDK.Lua
{
    public class LuaException : Exception
    {
        public LuaException(string message, Exception innerException) : base(message, innerException) { }
    }
}
