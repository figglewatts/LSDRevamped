using LSDR.Entities.Player;
using LSDR.SDK.Entities;
using LSDR.SDK.Lua.Actions;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class GameObjectProxy : AbstractLuaProxy<GameObject>
    {
        [MoonSharpHidden]
        public GameObjectProxy(GameObject target) : base(target) { }

        public InteractiveObject InteractiveObject => getAs<InteractiveObject>();
        public PlayerMovement PlayerMovement => getAs<PlayerMovement>();
        public PlayerCameraRotation PlayerCamera => getAs<PlayerCameraRotation>();
        public LuaAsyncActionRunner Action => getAs<LuaAsyncActionRunner>();

        public Vector3 WorldPosition => _target.transform.position;
        public Vector3 ForwardDirection => _target.transform.forward;

        public void SetActive(bool active)
        {
            _target.SetActive(active);
        }

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
