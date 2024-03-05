using LSDR.Entities.Player;
using MoonSharp.Interpreter;
using UnityEngine;

namespace LSDR.Lua.Proxies
{
    public class PlayerMovementProxy : AbstractLuaProxy<PlayerMovement>
    {
        [MoonSharpHidden]
        public PlayerMovementProxy(PlayerMovement target) : base(target) { }

        public PlayerRotation Rotation => _target.Rotation;

        public GameObject Camera => _target.Camera.gameObject;

        public void SetCanControl(bool canControl)
        {
            _target.Settings.CanControlPlayer = canControl;
        }

        public void StartMovingForward()
        {
            _target.Settings.CanMouseLook = false;
            _target.UseExternalInput(new Vector2(0, 1));
            _target.Rotation.UseExternalInput(0);
        }

        public void OverrideMovement()
        {
            _target.Settings.CanMouseLook = false;
            _target.UseExternalInput(Vector2.zero);
        }

        public void RotateTowards(Vector3 position)
        {
            _target.Settings.CanMouseLook = false;

            // use cross product to figure out if we should rotate left or right
            var playerForward = _target.Camera.forward;
            var direction = position - _target.transform.position;
            var cross = Vector3.Cross(playerForward, direction);
            float inputDir = cross.y > 0 ? 1 : -1;

            _target.Rotation.UseExternalInput(inputDir);
        }

        public void StopMovementAndRotation()
        {
            _target.StopExternalInput();
            _target.Rotation.StopExternalInput();
            _target.Settings.CanMouseLook = true;
        }
    }
}
