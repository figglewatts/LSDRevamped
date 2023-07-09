using System;
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
        public bool HasGraphContribution;
        public GraphContribution GraphContribution;
        public InteractionType InteractionKind;
        public float InteractionDistance = 3;

        public LuaAsyncActionRunner Action { get; protected set; }

        protected const float UPDATE_INTERVAL = 0.25f;

        protected float _t = 0;
        protected Animator _animator;
        protected InteractiveObjectLuaScript _luaScript;
        protected Camera _playerCamera;
        protected Transform _player;
        protected bool _interacted = false;

        public override void Start()
        {
            base.Start();

            _playerCamera = EntityIndex.Instance.GetComponent<Camera>("__camera");
            _player = EntityIndex.Instance.GetComponent<Transform>("__player");

            _animator = GetComponent<Animator>();
            Action = GetComponent<LuaAsyncActionRunner>();

            if (Script) _luaScript = new InteractiveObjectLuaScript(LuaManager.Managed, Script, this);
        }

        public void Update()
        {
            _luaScript.Update();

            _t += Time.deltaTime;
            if (_t > UPDATE_INTERVAL)
            {
                _t = 0;
                processPlayerInteraction();
            }
        }

        public void OnValidate()
        {
            if (InteractionDistance < 0)
            {
                InteractionDistance = 1;
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.6f);
            Gizmos.DrawWireSphere(transform.position, InteractionDistance);
        }

        protected void processPlayerInteraction()
        {
            if (_interacted) return;

            var playerDistance = Vector3.Distance(transform.position, _player.position);
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
            var screenPoint = _playerCamera.WorldToScreenPoint(transform.position);
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
            _luaScript.Interact();
            DreamControlManager.Managed.LogGraphContributionFromEntity(GraphContribution.Dynamic,
                GraphContribution.Upper);
            _interacted = true;
        }
    }
}
