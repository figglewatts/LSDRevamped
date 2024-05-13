using System;

namespace LSDR.SDK.Entities
{
    [Flags]
    public enum DayKind
    {
        None = 0,

        Odd = 1 << 1,
        Even = 1 << 2,
        One = 1 << 3,
        Two = 1 << 4,
        Three = 1 << 5,
        Four = 1 << 6,
        Five = 1 << 7,
        Six = 1 << 8,
        Seven = 1 << 9,

        All = Odd | Even | One | Two | Three | Four | Five | Six | Seven
    }
}
