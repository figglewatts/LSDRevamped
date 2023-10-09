using System;
using UnityEngine;
using MoonSharp.Interpreter;

namespace LSDR.SDK.Lua.Actions
{
    public class ActionPredicates : ILuaAPI
    {
        public void Register(ILuaEngine engine, Script script)
        {
            UserData.RegisterType<GenericPredicate>();
            UserData.RegisterType<WaitForSecondsPredicate>();
        }

        public static IPredicate Default() { return new GenericPredicate(() => true); }

        public static IPredicate WaitForSeconds(float numSeconds) { return new WaitForSecondsPredicate(numSeconds); }

        public static IPredicate WaitForLinearMove(GameObject obj, Vector3 end)
        {
            return new GenericPredicate(() =>
            {
                var length = (end - obj.transform.position).magnitude;
                return length < 0.2f;
            });
        }

        public static IPredicate PointingAt(GameObject obj, Vector3 worldPos)
        {
            return new GenericPredicate(() =>
            {
                var direction = worldPos - obj.transform.position;
                direction.y = 0; // cancel out Y, so we can walk up/down slopes
                float curAngle = Vector3.Angle(direction, obj.transform.forward);
                return curAngle < 1;
            });
        }

        public static IPredicate Custom(Closure closure) { return new GenericPredicate(() => closure.Call().Boolean); }
    }
}
