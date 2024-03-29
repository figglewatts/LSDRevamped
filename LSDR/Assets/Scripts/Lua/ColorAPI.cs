using System;
using LSDR.SDK.Lua;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua
{
    public class ColorAPI : ILuaAPI
    {
        public static Color Red => Color.red;
        public static Color Black => Color.black;
        public static Color Blue => Color.blue;
        public static Color Clear => Color.clear;
        public static Color Cyan => Color.cyan;
        public static Color Gray => Color.gray;
        public static Color Grey => Color.grey;
        public static Color Magenta => Color.magenta;
        public static Color White => Color.white;
        public static Color Yellow => Color.yellow;
        public static Color Green => Color.green;

        public void Register(ILuaEngine engine, Script script) { }
    }
}
