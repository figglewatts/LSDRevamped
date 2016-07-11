using Game;
using UnityEngine;
using InputManagement;

namespace Entities.Player
{
    [RequireComponent(typeof (CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
		public float MovementSpeed;

		public bool m_IsWalking;
        public float m_GravityMultiplier;

        private Camera m_Camera;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!m_CharacterController.isGrounded && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, ~0, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (!m_CharacterController.isGrounded)
            {
				m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
			}
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);
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
            m_Input = new Vector2(moveDirLeftRight, moveDirFrontBack);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
