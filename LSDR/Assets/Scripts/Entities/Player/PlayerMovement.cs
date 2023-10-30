using System;
using System.Collections;
using LSDR.Dream;
using LSDR.Game;
using LSDR.InputManagement;
using LSDR.SDK.Audio;
using LSDR.SDK.Entities;
using Torii.Audio;
using Torii.Console;
using Torii.Util;
using UnityEngine;

namespace LSDR.Entities.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        public SettingsSystem Settings;
        public ControlSchemeLoaderSystem ControlScheme;
        public DreamSystem DreamSystem;
        public Transform Camera;
        public PlayerRotation Rotation;
        public AudioClip FootstepClip;

        [Console] public float GravityMultiplier = 1f;
        [Console] public float LinkDelay = 1.7F;

        [Console] public float WalkMoveSpeed = 0.6f;
        [Console] public float RunMoveSpeed = 2.4f;
        [Console] public float StepTimeSeconds = 0.25f;

        [Console] public float HeadBobAmount = 0.1f;
        [Console] public bool DebugLog;
        public bool CanLink = true;

        protected CharacterController _controller;
        protected bool _currentlySprinting;
        protected bool _currentlyStepping;

        protected float _initialCameraOffset;
        protected Vector2 _inputDir;
        protected bool _inputLocked;
        protected Vector2 _lockedInput;

        protected bool _externalInput;
        protected Vector2 _externalInputDirection;

        protected FixedTimeSince _timeColliding;
        protected TimeSince _timeSinceStartedMoving;
        protected bool _playedWalkStep;
        protected Collider[] _depenetrateOverlapBuffer = new Collider[32];

        protected Vector3 _boxBottom => new Vector3(
            transform.position.x,
            transform.position.y + _controller.skinWidth * 2,
            transform.position.z);

        protected Vector3 _boxTop => new Vector3(
            transform.position.x,
            _boxBottom.y + _controller.height,
            transform.position.z);

        public void Start()
        {
            _controller = GetComponent<CharacterController>();

            EntityIndex.Instance.Register("__player", gameObject, force: true);

            _initialCameraOffset = _controller.height;

            resetHeadbob();

            DevConsole.Register(this);
        }

        public void Update()
        {
            // if we're not grounded we want to apply gravity
            if (!_controller.isGrounded && Settings.PlayerGravity)
                _controller.Move(Physics.gravity * (GravityMultiplier * Time.deltaTime));

            // exit early if movement is disabled
            if (!_inputLocked && !Settings.CanControlPlayer) return;

            _inputDir = getInputDirection();

            if (_inputDir == Vector2.zero && !_currentlyStepping)
            {
                // reset head bob if we're not moving
                resetHeadbob();
            }

            // check to see if we should sprint
            if (ControlScheme.InputActions.Game.Run.IsPressed() && canStartSprinting()) _currentlySprinting = true;

            // perform a single step
            float moveSpeed = _currentlySprinting ? RunMoveSpeed : WalkMoveSpeed;
            if (!ControlScheme.Current.FpsControls && !_currentlyStepping && _inputDir.sqrMagnitude > 0)
            {
                StartCoroutine(takeStep(_inputDir, moveSpeed));
            }
            else if (ControlScheme.Current.FpsControls && _inputDir.sqrMagnitude > 0)
            {
                if (!_currentlyStepping)
                {
                    _currentlyStepping = true;
                    _timeSinceStartedMoving = 0;
                }

                if (!_playedWalkStep && _timeSinceStartedMoving > StepTimeSeconds / 2)
                {
                    playFootstepSound();
                    _playedWalkStep = true;
                }

                if (_timeSinceStartedMoving > StepTimeSeconds)
                {
                    if (_currentlySprinting) playFootstepSound();
                    _playedWalkStep = false;
                    _timeSinceStartedMoving = 0;
                }

                moveController(_inputDir, moveSpeed);
            }

            // if run is not pressed and no movement keys are pressed, we should no longer sprint
            if ((!ControlScheme.InputActions.Game.Run.IsPressed()
                 && !ControlScheme.InputActions.Game.Move.IsPressed()) || _externalInput)
            {
                _currentlySprinting = false;
            }
        }

        public void OnDestroy() { DevConsole.Deregister(this); }

        public void UseExternalInput(Vector2 direction)
        {
            _externalInput = true;
            _externalInputDirection = direction;
            Settings.CanMouseLook = false;
        }

        public void StopExternalInput()
        {
            _externalInput = false;
            _externalInputDirection = Vector2.zero;
            Settings.CanMouseLook = true;
        }

        public void SetInputLock()
        {
            _inputLocked = true;
            _lockedInput = _inputDir;
        }

        public void ClearInputLock() { _inputLocked = false; }

        private IEnumerator takeStep(Vector2 input, float speedUnitsPerSecond)
        {
            _currentlyStepping = true;
            bool playedFootstep = false;

            float t = 0;
            while (t < StepTimeSeconds)
            {
                t += Time.deltaTime;
                float progress = t / StepTimeSeconds;

                // make sure we don't do this for too many frames or it'll look jerky
                if (StepTimeSeconds - t < 0) break;

                // we play the footstep when we reach the top (which is half way through, hence 0.5f)
                if (!playedFootstep && progress > 0.5f)
                {
                    playedFootstep = true;
                    playFootstepSound();
                }

                moveController(input, speedUnitsPerSecond);
                headBob(progress);

                yield return null;
            }

            // move again after finishing, so there's not a one frame gap between the next step
            moveController(input, speedUnitsPerSecond);

            // if we're sprinting we want to play the footstep sound on the downstep too
            if (_currentlySprinting) playFootstepSound();

            _currentlyStepping = false;
        }

        private void headBob(float t)
        {
            if (!Settings.Settings.HeadBobEnabled) return;

            float sinePos = t * 2 * Mathf.PI - Mathf.PI / 2;

            // the +1 here is to make the whole wave positive, and the divide by 2 is to normalize it to [0, 1]
            float yAddition = (Mathf.Sin(sinePos) + 1) / 2f;

            // scale it so it's not massive
            yAddition *= HeadBobAmount;

            float newCameraY = _initialCameraOffset + yAddition;
            Camera.localPosition = new Vector3(Camera.localPosition.x, newCameraY, Camera.localPosition.z);
        }

        /// <summary>
        ///     Reset the head bob sine wave position to the start.
        /// </summary>
        private void resetHeadbob()
        {
            Camera.localPosition = new Vector3(Camera.localPosition.x, _initialCameraOffset, Camera.localPosition.z);
        }

        private void moveController(Vector2 input, float speedUnitsPerSecond)
        {
            Vector3 moveAmount = transform.forward * input.y + transform.right * input.x;

            // apply the movement speed and delta time
            moveAmount *= speedUnitsPerSecond * Time.deltaTime;

            // perform collision detection (to override default Unity char controller's 'slide along walls' for
            // LSD's janky 'lets just stop in place')
            if (movingIntoWall(moveAmount))
            {
                // we shouldn't move anymore
                moveAmount = Vector3.zero;

                // check to see if we should link
                if (_timeColliding > LinkDelay && CanLink)
                {
                    CanLink = false;
                    DreamSystem.Transition();
                }
            }
            else
                _timeColliding = 0;

            // move the controller
            var flags = _controller.Move(moveAmount);

            // this is a bit of a hack for when our box ends up inside geometry without hitting the boxcast
            // normally we'd slide here but we can detect this state with this statement and force a link instead
            // I have successfully gaslit myself that this is the least worst option
            if (flags.HasFlag(CollisionFlags.Sides) && isOverlapping() && !_externalInput)
            {
                CanLink = false;
                DreamSystem.Transition();
            }
        }

        private bool isOverlapping()
        {
            Vector3 topBottom = _boxTop - _boxBottom;
            Vector3 boxCenter = _boxBottom + topBottom / 2;
            Vector3 halfExtents = new Vector3(_controller.radius, topBottom.y / 2, _controller.radius);
            return Physics.CheckBox(boxCenter, halfExtents);
        }

        /// <summary>
        ///     Perform a bunch of checks to see if we are trying to move into a wall.
        /// </summary>
        /// <param name="desiredMove">The vector we're trying to move along.</param>
        /// <returns>True if we are moving into a wall, false otherwise.</returns>
        private bool movingIntoWall(Vector3 desiredMove)
        {
            float moveDistance = desiredMove.magnitude;
            Vector3 castDirection = desiredMove.normalized;
            float stepTopYPos = transform.position.y + _controller.stepOffset + _controller.skinWidth;
            Vector3 stepTopPos = new Vector3(transform.position.x, stepTopYPos, transform.position.z);
            Vector3 boxTop = _boxTop;
            Vector3 boxBottom = _boxBottom;

            if (DebugLog)
                Debug.Log(
                    $"1. StepTop: {stepTopPos:F5}, _boxBottom: {boxBottom:F5}, _boxTop: {boxTop:F5}");

            (bool hitAboveStepHeight, RaycastHit hit) = castController(stepTopPos, boxTop, _controller.radius,
                castDirection, _controller.skinWidth * 2);
            if (DebugLog)
            {
                Debug.Log(
                    $"2. hitAboveStepHeight: {hitAboveStepHeight}, collider: {hit.collider}, norm: {hit.normal}");
            }
            float angle = Mathf.Acos(hit.normal.y) * Mathf.Rad2Deg;

            if (hitAboveStepHeight && !hit.collider.isTrigger && angle > _controller.slopeLimit) return true;

            bool hitSomething;
            (hitSomething, hit) = castController(boxBottom, boxTop, _controller.radius, castDirection,
                moveDistance);
            Vector3 axis = Vector3.Cross(transform.up, castDirection);
            float slopeAngle = Vector3.SignedAngle(hit.normal, transform.up, axis);
            bool hitOverSlopeLimit = hitSomething && slopeAngle > _controller.slopeLimit;
            bool hitSeemsLikeAStep = (slopeAngle > 80 && slopeAngle < 100) || hit.distance < 1E-06;

            if (DebugLog)
            {
                Debug.Log(
                    $"3. hitSomething: {hitSomething}, hitNormal: {hit.normal}, slopeAngle: {slopeAngle}, " +
                    $"hitOverSlope: {hitOverSlopeLimit}, hitSeemsLikeAStep: {hitSeemsLikeAStep}");
            }

            return hit.collider && !hit.collider.isTrigger && hitOverSlopeLimit && !hitSeemsLikeAStep;
        }

        protected void playFootstepSound()
        {
            if (!Settings.Settings.EnableFootstepSounds) return;
            var footstep = getFootstepSound();
            if (footstep == null || footstep.Sound == null) return;
            AudioPlayer.Instance.PlayClip(footstep.Sound, loop: false, pitch: footstep.Pitch, mixerGroup: "SFX");
        }

        protected Footstep getFootstepSound()
        {
            (bool hitSomething, RaycastHit hit) = castController(_boxBottom, _boxTop, _controller.radius,
                -transform.up,
                1);

            if (!hitSomething || Settings.CurrentJournal.FootstepIndex == null) return null;

            return Settings.CurrentJournal.FootstepIndex.GetFootstep(hit);
        }

        protected (bool, RaycastHit) castController(Vector3 bottomPos, Vector3 topPos, float halfExtent,
            Vector3 direction, float distance)
        {
            Vector3 topBottom = topPos - bottomPos;
            Vector3 boxCenter = bottomPos + topBottom / 2;
            Vector3 halfExtents = new Vector3(halfExtent, topBottom.y / 2, halfExtent);
            bool hitSomething = Physics.BoxCast(boxCenter, halfExtents, direction,
                out RaycastHit hitInfo, Quaternion.identity, distance);
            return (hitSomething, hitInfo);
        }

        private Vector2 getInputDirection()
        {
            if (Time.timeScale <= 0) return Vector2.zero;

            // handle input lock
            if (_inputLocked) return _lockedInput;

            // if we can't control the player return zero for input direction
            if (!Settings.CanControlPlayer) return Vector2.zero;

            // get vector axes from input system
            Vector2 move = ControlScheme.InputActions.Game.Move.ReadValue<Vector2>();
            move.x = ControlScheme.Current.FpsControls ? move.x : 0;

            // handle input from external sources (i.e. Lua scripts controlling the player)
            if (_externalInput) move = _externalInputDirection;

            // no partial movement in classic mode
            if (!ControlScheme.Current.FpsControls)
            {
                if (move.x > 0) move.x = 1;
                else if (move.x < 0) move.x = -1;
                if (move.y > 0) move.y = 1;
                else if (move.y < 0) move.y = -1;
            }

            return move;
        }

        /// <summary>
        ///     We can start sprinting if we're moving on the X axis (strafing) or if we're moving forwards.
        ///     We can only sprint if input is not being overridden.
        /// </summary>
        /// <returns>True if we can start sprinting, false otherwise.</returns>
        private bool canStartSprinting()
        {
            Vector2 move = ControlScheme.InputActions.Game.Move.ReadValue<Vector2>();
            return !_externalInput && (move.x != 0 || move.y > 0);
        }
    }
}
