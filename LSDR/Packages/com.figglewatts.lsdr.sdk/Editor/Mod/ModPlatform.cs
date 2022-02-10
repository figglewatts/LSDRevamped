using System;

namespace LSDR.SDK.Editor.Mod
{
    [Flags]
    public enum ModPlatform
    {
        Nothing = 0,
        Windows = 1 << 0,
        Linux = 1 << 1,
        OSX = 1 << 2,
        Everything = Windows | Linux | OSX
    }
}
