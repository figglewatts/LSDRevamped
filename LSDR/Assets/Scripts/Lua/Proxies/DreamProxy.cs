using System.Collections.Generic;
using LSDR.SDK.Data;
using MoonSharp.Interpreter;

namespace LSDR.Lua.Proxies
{
    public class DreamProxy : AbstractLuaProxy<SDK.Data.Dream>
    {
        [MoonSharpHidden]
        public DreamProxy(SDK.Data.Dream target) : base(target) { }

        public string Name => _target.Name;
        public string Author => _target.Author;
        public bool GreyMan => _target.GreyMan;
        public bool Linkable => _target.Linkable;
        public List<DreamEnvironment> Environments => _target.Environments;
    }
}
