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
    [RequireComponent(typeof(LuaAsyncActionRunner))]
    public class InteractiveObject : BaseEntity
    {
        public enum InteractionType
        {
            Proximity,
            View,
            None
        }

        public LuaScriptAsset Script;
        public InteractionType InteractionKind;
        public float InteractionDistance = 3;
        public AnimatedObject AnimatedObject;

        public float UpdateIntervalSeconds => _currentUpdateInterval <= 0 ? Time.deltaTime : _currentUpdateInterval;

        protected bool _interacted;
        protected InteractiveObjectLuaScript _luaScript;
        protected Transform _player;
        protected Camera _playerCamera;
        protected float _t;
        protected float _currentUpdateInterval = 0.25f;

        public LuaAsyncActionRunner Action { get; protected set; }

        public override void Init()
        {
            _playerCamera = EntityIndex.Instance.GetComponent<Camera>("__camera");
            _player = EntityIndex.Instance.GetComponent<Transform>("__player");

            Action = GetComponent<LuaAsyncActionRunner>();
            if (AnimatedObject == null) AnimatedObject = GetComponent<AnimatedObject>();

            if (Script)
            {
                _luaScript = new InteractiveObjectLuaScript(LuaManager.Managed, Script, this);
                _luaScript?.IntervalUpdate();
            }
        }

        public void Update()
        {
            if (_player == null) return;

            _luaScript?.Update();

            _t += Time.deltaTime;
            if (_t > _currentUpdateInterval)
            {
                _t = 0;
                processPlayerInteraction();
                _luaScript?.IntervalUpdate();
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(r: 1, g: 0, b: 0, a: 0.6f);
            Gizmos.DrawWireSphere(transform.position, InteractionDistance);
        }

        public void OnValidate()
        {
            base.OnValidate();
            if (InteractionDistance < 0)
            {
                InteractionDistance = 1;
            }
        }

        public void SetUpdateIntervalSeconds(float seconds)
        {
            _currentUpdateInterval = seconds;
        }

        protected void processPlayerInteraction()
        {
            if (_interacted || InteractionKind == InteractionType.None) return;

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
