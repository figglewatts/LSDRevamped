using Game;
using UnityEngine;
using InputManagement;

namespace Entities.Player
{
    /// <summary>
	/// Handles player motion. Moving forwards and backwards, and left to right if FPS movement is enabled.
	/// </summary>
	[RequireComponent(typeof (CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
		public float MovementSpeed;
        public float GravityMultiplier;

        private Vector2 _inputVector;
        private Vector3 _moveDir = Vector3.zero;
        private CharacterController _characterController;
        private CollisionFlags _collisionFlags;
        private bool _previouslyGrounded;

        // Use this for initialization
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!_characterController.isGrounded && _previouslyGrounded)
            {
                _moveDir.y = 0f;
            }

            _previouslyGrounded = _characterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*_inputVector.y + transform.right*_inputVector.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out hitInfo,
                               _characterController.height/2f, ~0, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            _moveDir.x = desiredMove.x*speed;
            _moveDir.z = desiredMove.z*speed;


            if (!_characterController.isGrounded)
            {
				_moveDir += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;
			}
            _collisionFlags = _characterController.Move(_moveDir*Time.fixedDeltaTime);
        }

        private void GetInput(out float speed)
        {
	        float moveDirFrontBack = 0;
	        if (InputHandler.CheckButtonState("Forward", ButtonState.HELD))
	        {
		        moveDirFrontBack = 1;
	        }
			else if (InputHandler.CheckButtonState("Backward", ButtonState.HELD))
			{
				moveDirFrontBack = -1;
			}

	        float moveDirLeftRight = 0;
	        if (GameSettings.FPSMovementEnabled)
	        {
		        if (InputHandler.CheckButtonState("Left", ButtonState.HELD))
		        {
			        moveDirLeftRight = -1;
		        }
				else if (InputHandler.CheckButtonState("Right", ButtonState.HELD))
				{
					moveDirLeftRight = 1;
				}
	        }
		
			// set the desired speed to be walking or running
            speed = MovementSpeed;
            _inputVector = new Vector2(moveDirLeftRight, moveDirFrontBack);

            // normalize input if it exceeds 1 in combined length:
            if (_inputVector.sqrMagnitude > 1)
            {
                _inputVector.Normalize();
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(_characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
