using System;

namespace Torii.Console
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class ConsoleAttribute : Attribute
    {
        // intentionally empty
    }
}