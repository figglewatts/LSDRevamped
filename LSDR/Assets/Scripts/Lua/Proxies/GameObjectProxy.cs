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

        public DreamAudio DreamAudio => getAs<DreamAudio>();

        public Vector3 WorldPosition
        {
            get => _target.transform.position;
            set => _target.transform.position = value;
        }

        public Vector3 LocalPosition
        {
            get => _target.transform.localPosition;
            set => _target.transform.localPosition = value;
        }

        public Vector3 WorldRotation
        {
            get => _target.transform.rotation.eulerAngles;
            set => _target.transform.rotation = Quaternion.Euler(value);
        }

        public Vector3 LocalRotation
        {
            get => _target.transform.localRotation.eulerAngles;
            set => _target.transform.localRotation = Quaternion.Euler(value);
        }

        public Vector3 Scale
        {
            get => _target.transform.localScale;
            set => _target.transform.localScale = value;
        }

        public Vector3 ForwardDirection => _target.transform.forward;
        public Vector3 RightDirection => _target.transform.right;
        public Vector3 UpDirection => _target.transform.up;

        public void SetActive(bool active)
        {
            _target.SetActive(active);
        }

        // TODO: PositionToWorld(Vector3 position)

        // TODO: PositionFromWorld(Vector3 worldPosition)

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
