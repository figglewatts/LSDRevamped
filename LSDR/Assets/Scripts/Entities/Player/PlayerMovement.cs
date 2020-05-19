using System;
using InControl;
using LSDR.Dream;
using LSDR.Game;
using LSDR.InputManagement;
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
        
        public float MovementSpeed = 5f;
        public float Size = 1f;
        public float GravityMultiplier = 1f;
        public float LinkDelay = 0.7F;

        private CharacterController _controller;
        private Vector3 _moveDir;
        private FixedTimeSince _timeColliding;
        private bool _canLink = true;

        public void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        public void Update() { _moveDir = getInput(); }

        void FixedUpdate () {
            Vector3 desiredMove = transform.forward*_moveDir.y + transform.right*_moveDir.x;
            
            // apply the movement speed
            desiredMove *= MovementSpeed;

            if (movingIntoWall(desiredMove))
            {
                desiredMove = Vector3.zero;
                if (_timeColliding > LinkDelay && _canLink)
                {
                    _canLink = false;
                    DreamSystem.Transition(RandUtil.RandColor());
                }
            }
            else
            {
                _timeColliding = 0;
            }
            
            // if we're not grounded we want to apply gravity to the movement direction
            if (!_controller.isGrounded)
            {
                desiredMove += Physics.gravity * GravityMultiplier;
            }

            _controller.Move(desiredMove * Time.fixedDeltaTime);
        }

        /// <summary>
        /// Perform a bunch of checks to see if we are trying to move into a wall.
        /// </summary>
        /// <param name="desiredMove">The vector we're trying to move along.</param>
        /// <returns>True if we are moving into a wall, false otherwise.</returns>
        private bool movingIntoWall(Vector3 desiredMove)
        {
            float stepTopYPos = (transform.position.y - _controller.height / 2f) + _controller.stepOffset +
                                _controller.skinWidth;
            Vector3 stepTopPos = new Vector3(transform.position.x, stepTopYPos, transform.position.z);
            Vector3 capsuleTop = new Vector3(transform.position.x, transform.position.y + _controller.height / 2f,
                transform.position.z);
            Vector3 capsuleBottom = new Vector3(transform.position.x, transform.position.y - _controller.height / 2f,
                transform.position.z);

            RaycastHit hit;
            bool hitAboveStepHeight = Physics.CapsuleCast(stepTopPos, capsuleTop, _controller.radius, desiredMove, out hit,
                _controller.skinWidth * 2);
            if (hit.collider == null) return false;
            if (hitAboveStepHeight && !hit.collider.isTrigger) return true;
            
            bool hitSomething = Physics.CapsuleCast(capsuleBottom, capsuleTop, _controller.radius, desiredMove, out hit,
                _controller.skinWidth * 2);
            Vector3 axis = Vector3.Cross(transform.up, desiredMove);
            bool hitOverSlopeLimit =
                hitSomething && Vector3.SignedAngle(hit.normal, transform.up, axis) > _controller.slopeLimit;
            bool hitSeemsLikeAStep = hit.normal.y < 0.1f || hit.distance < 1E-06;
            
            Debug.Log("Slope limit: " + hitOverSlopeLimit);
            Debug.Log("It's a step: " + hitSeemsLikeAStep);
            Debug.Log("Normal: " + hit.normal);
            Debug.Log("Distance: " + hit.distance);
            Debug.Log("Angle: " + Vector3.SignedAngle(hit.normal, transform.up, axis));

            return !hit.collider.isTrigger && hitOverSlopeLimit && !hitSeemsLikeAStep;
        }

        private Vector2 getInput()
        {
            // if we can't control the player return zero for input direction
            if (!Settings.CanControlPlayer) return Vector2.zero;
            // get vector axes from input system
            float moveDirFrontBack = ControlScheme.Current.Actions.MoveY;
            float moveDirLeftRight = ControlScheme.Current.FpsControls ? ControlScheme.Current.Actions.MoveX : 0f;
            Vector2 input = new Vector2(moveDirLeftRight, moveDirFrontBack);

            // normalize input if it exceeds 1 in combined length (for diagonal movement)
            if (input.sqrMagnitude > 1)
            {
                input.Normalize();
            }

            return input;
        }
    }
}
