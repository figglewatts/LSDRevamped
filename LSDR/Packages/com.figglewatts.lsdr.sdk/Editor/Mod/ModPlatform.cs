using System;

namespace LSDR.SDK.Editor.Mod
{
    [Flags]
    public enum ModPlatform
    {
        Windows = 1 << 0,
        Linux = 1 << 1,
        OSX = 1 << 2,
    }
}
