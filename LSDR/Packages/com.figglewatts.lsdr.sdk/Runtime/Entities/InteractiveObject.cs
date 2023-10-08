using System;
using LSDR.SDK.Animation;
using LSDR.SDK.Assets;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Lua;
using LSDR.SDK.Lua.Actions;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(LuaAsyncActionRunner))]
    public class InteractiveObject : BaseEntity
    {
        public enum InteractionType
        {
            Proximity,
            View
        }

        public LuaScriptAsset Script;
        public InteractionType InteractionKind;
        public float InteractionDistance = 3;

        public Animator Animator => _animator;
        public AnimatedObject AnimatedObject => _animatedObject;

        protected const float UPDATE_INTERVAL = 0.25f;

        protected Animator _animator;
        protected bool _interacted;
        protected InteractiveObjectLuaScript _luaScript;
        protected Transform _player;
        protected Camera _playerCamera;
        protected AnimatedObject _animatedObject;
        protected float _t;

        public LuaAsyncActionRunner Action { get; protected set; }

        public override void Init()
        {
            _playerCamera = EntityIndex.Instance.GetComponent<Camera>("__camera");
            _player = EntityIndex.Instance.GetComponent<Transform>("__player");

            _animator = GetComponent<Animator>();
            Action = GetComponent<LuaAsyncActionRunner>();
            _animatedObject = GetComponent<AnimatedObject>();

            if (Script) _luaScript = new InteractiveObjectLuaScript(LuaManager.Managed, Script, this);
        }

        public void Update()
        {
            _luaScript?.Update();

            _t += Time.deltaTime;
            if (_t > UPDATE_INTERVAL)
            {
                _t = 0;
                processPlayerInteraction();
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(r: 1, g: 0, b: 0, a: 0.6f);
            Gizmos.DrawWireSphere(transform.position, InteractionDistance);
        }

        public void OnValidate()
        {
            if (InteractionDistance < 0)
            {
                InteractionDistance = 1;
            }
        }

        protected void processPlayerInteraction()
        {
            if (_interacted) return;

            float playerDistance = Vector3.Distance(transform.position, _player.position);
            switch (InteractionKind)
            {
                case InteractionType.Proximity:
                    processProximityInteraction(playerDistance);
                    break;
                case InteractionType.View:
                    processViewInteraction(playerDistance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void processProximityInteraction(float playerDistance)
        {
            if (playerDistance <= InteractionDistance) interact();
        }

        protected void processViewInteraction(float playerDistance)
        {
            if (playerDistance > InteractionDistance) return;

            // use the camera to see if this object is on screen
            Vector3 screenPoint = _playerCamera.WorldToScreenPoint(transform.position);
            if (screenPoint.z < 0 ||
                screenPoint.x < 0 || screenPoint.x > Screen.width ||
                screenPoint.y < 0 || screenPoint.y > Screen.height)
            {
                return;
            }

            interact();
        }

        protected void interact()
        {
            _luaScript?.Interact();
            _interacted = true;
        }
    }
}
