using LSDR.Entities.Player;
using LSDR.SDK.Animation;
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

        public AnimatedObject AnimatedObject => getAs<AnimatedObject>();

        public MovieClip VideoClip => getAs<MovieClip>();

        public string Name => _target.name;

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

        public bool Active => _target.activeInHierarchy;

        public void SetActive(bool active)
        {
            _target.SetActive(active);
        }

        public GameObject GetChildByName(string name)
        {
            var found = _target.transform.Find(name);
            if (found == null) Debug.LogWarning($"unable to find GameObject with name '{name}'");
            return found.gameObject;
        }

        public Vector3 PositionToWorld(Vector3 position)
        {
            return _target.transform.TransformPoint(position);
        }

        public Vector3 DirectionToWorld(Vector3 direction)
        {
            return _target.transform.TransformDirection(direction);
        }

        public Vector3 PositionFromWorld(Vector3 worldPosition)
        {
            return _target.transform.InverseTransformPoint(worldPosition);
        }

        public Vector3 DirectionFromWorld(Vector3 worldDirection)
        {
            return _target.transform.InverseTransformDirection(worldDirection);
        }

        public void LookAt(Vector3 worldPosition)
        {
            _target.transform.LookAt(worldPosition, _target.transform.up);
        }

        public void LookAtPlane(Vector3 worldPosition)
        {
            var target = worldPosition;
            target.y = _target.transform.position.y;
            LookAt(target);
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
