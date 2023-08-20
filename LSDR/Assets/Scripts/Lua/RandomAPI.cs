using System;
using LSDR.SDK.Lua;
using LSDR.SDK.Util;
using MoonSharp.Interpreter;

namespace LSDR.Lua
{
    public class RandomAPI : ILuaAPI
    {
        public void Register(ILuaEngine engine, Script script) { }

        public static int IntMax(int max) { return RandUtil.Int(max); }

        public static int IntMinMax(int min, int max) => RandUtil.Int(min, max);

        public static float Float() => RandUtil.Float();

        public static float FloatMax(float max) => RandUtil.Float(max);

        public static float FloatMinMax(float min, float max) => RandUtil.Float(min, max);

        public static bool OneIn(float chance) => RandUtil.OneIn(chance);
    }
}
