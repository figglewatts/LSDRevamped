using LSDR.SDK.Entities;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class GameObjectProxy : AbstractLuaProxy<GameObject>
    {
        [MoonSharpHidden]
        public GameObjectProxy(GameObject target) : base(target) { }

        public InteractiveObject InteractiveObject => getAs<InteractiveObject>();

        protected T getAs<T>() where T : MonoBehaviour
        {
            T component = _target.GetComponent<T>();
            if (component == null)
            {
                throw new ScriptRuntimeException($"GameObject '{_target}' is not a {typeof(T)}");
            }
            return component;
        }
    }
}
