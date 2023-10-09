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

#endregion

#region Functions

        public IPredicate WaitForAnimation(int index)
        {
            return new WaitForSecondsPredicate(_target.AnimatedObject.Clips[index].length);
        }

        public void PlayAnimation(int index)
        {
            if (index >= _target.AnimatedObject.Clips.Length)
            {
                Debug.LogWarning($"Unable to play animation {index} on object '{_gameObject.name}', " +
                                 $"object only has {_target.AnimatedObject.Clips.Length} animations");
                return;
            }

            _target.Animator.enabled = true;
            _target.Animator.Play(_target.AnimatedObject.Clips[index].name);
        }

        public void ResumeAnimation() { _target.Animator.enabled = true; }

        public void StopAnimation() { _target.Animator.enabled = false; }

        public bool MoveTowards(Vector3 worldPosition, float speed)
        {
            var toTarget = (worldPosition - _target.transform.position);
            if (toTarget.magnitude < 0.001)
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
            _target.transform.LookAt(-worldPosition, _target.transform.up);
        }

        public bool LookTowards(Vector3 worldPosition, float speed)
        {
            var current = _target.transform.rotation;
            var toTarget = (_target.transform.position - worldPosition).normalized;
            toTarget.y = 0; // flatten
            var desired = Quaternion.LookRotation(toTarget, _target.transform.up);

            if (Quaternion.Dot(current, desired) > 0.99f)
            {
                // we are basically already at the rotation
                _target.transform.rotation = desired;
                return true;
            }

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

#endregion
    }
}
