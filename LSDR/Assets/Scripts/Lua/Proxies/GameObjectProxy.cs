using LSDR.SDK.Entities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class GameObjectProxy : AbstractLuaProxy<GameObject>
    {
        [MoonSharpHidden]
        public GameObjectProxy(GameObject target) : base(target) { }

        public InteractiveObject InteractiveObject
        {
            get
            {
                InteractiveObject interactiveObject = _target.GetComponent<InteractiveObject>();
                if (interactiveObject == null)
                {
                    throw new ScriptRuntimeException($"GameObject '{_target}' is not an InteractiveObject");
                }

                return interactiveObject;
            }
        }
    }
}
