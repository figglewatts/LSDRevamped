using LSDR.SDK.DreamControl;
using LSDR.SDK.Entities;
using LSDR.SDK.Lua.Actions;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class InteractiveObjectProxy : AbstractLuaProxy<InteractiveObject>
    {
        private readonly GameObject _gameObject;

        [MoonSharpHidden]
        public InteractiveObjectProxy(InteractiveObject target) : base(target) { _gameObject = target.gameObject; }

#region Properties

        public Vector3 Forward => _gameObject.transform.forward;
        public Vector3 Up => _gameObject.transform.up;
        public Vector3 Right => _gameObject.transform.right;

        public GameObject GameObject => _gameObject;

        public LuaAsyncActionRunner Action => _target.Action;

        public float InteractionDistance
        {
            get => _target.InteractionDistance;
            set => _target.InteractionDistance = value;
        }

#endregion

#region Functions

        public void Init()
        {
            _target.Init();
        }

        public IPredicate WaitForAnimation(int index, float offsetSeconds = 0)
        {
            return new WaitForSecondsPredicate(_target.AnimatedObject.Clips[index].length + offsetSeconds);
        }

        public float GetAnimationLengthSeconds(int index)
        {
            return _target.AnimatedObject.Clips[index].length;
        }

        public void PlayAnimation(int index)
        {
            _target.AnimatedObject.Play(index);
        }

        public void ResumeAnimation() { _target.AnimatedObject.Resume(); }

        public void StopAnimation() { _target.AnimatedObject.Stop(); }

        public bool MoveTowards(Vector3 worldPosition, float speed)
        {
            var toTarget = (worldPosition - _target.transform.position);
            if (toTarget.magnitude < 0.1)
            {
                _target.transform.position = worldPosition;
                return true;
            }
            var toTargetDirection = toTarget.normalized;
            MoveInDirection(toTargetDirection, speed);
            return false;
        }

        public void MoveInDirection(Vector3 worldDirection, float speed)
        {
            var moveVec = worldDirection * speed * Time.deltaTime;
            _target.transform.position += moveVec;
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

        public void LookInDirection(Vector3 worldDirection)
        {
            _target.transform.LookAt(_target.transform.position + worldDirection, _target.transform.up);
        }

        public bool LookTowards(Vector3 worldPosition, float speed)
        {
            var current = _target.transform.rotation;
            var toTarget = (worldPosition - _target.transform.position).normalized;
            toTarget.y = 0; // flatten

            if (toTarget.sqrMagnitude < 0.01f)
            {
                return true;
            }

            var desired = Quaternion.LookRotation(toTarget, _target.transform.up);

            if (Quaternion.Dot(current, desired) > 0.99f)
            {
                // we are basically already at the rotation
                _target.transform.rotation = desired;
                return true;
            }

            Debug.Log("looking towards");

            _target.transform.rotation = Quaternion.RotateTowards(current, desired, speed * Time.deltaTime);
            return false;
        }

        public void SnapToFloor(float checkHeight = 10)
        {
            bool hit = Physics.Raycast(_target.transform.position + new Vector3(0, checkHeight, 0),
                Vector3.down,
                out RaycastHit hitInfo,
                Mathf.Infinity);
            if (!hit) return; // do nothing if we didn't hit anything

            // otherwise set our position to where we hit
            _target.transform.position = hitInfo.point;
        }

        public void StretchShrink(float factor)
        {
            _target.transform.localScale = new Vector3(1, factor, 1);
        }

        public void SetChildVisible(bool visible)
        {
            _target.transform.GetChild(0).gameObject.SetActive(visible);
        }

        public void SetRenderersActive(bool active)
        {
            foreach (var r in _target.GetComponentsInChildren<Renderer>())
            {
                r.gameObject.SetActive(active);
            }
        }

        public void SetUpdateIntervalSeconds(float seconds)
        {
            _target.SetUpdateIntervalSeconds(seconds);
        }

        public void LogGraphContribution(int dynamicness, int upperness)
        {
            DreamControlManager.Managed.LogGraphContributionFromEntity(dynamicness, upperness, _target);
        }

#endregion
    }
}
