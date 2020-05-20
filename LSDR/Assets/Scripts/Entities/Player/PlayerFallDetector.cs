using System.Collections;
using LSDR.Dream;
using UnityEngine;
using LSDR.Game;

namespace LSDR.Entities.Player
{
	/// <summary>
	/// Used to detect whether or not a player is currently falling.
	/// </summary>
	[RequireComponent(typeof(CharacterController))]
	public class PlayerFallDetector : MonoBehaviour
	{
		public SettingsSystem Settings;
		public DreamSystem DreamSystem;
		
		[SerializeField]
		private Camera _targetCamera;
		private CharacterController _playerController;
		private bool _hasFallen = false;
		
		private const float ROTATION_SPEED = 45F;
		private const float MAX_X_ROTATION = 80F;

		public void Awake()
		{
			_playerController = GetComponent<CharacterController>();
		}

		public void FixedUpdate()
		{
			if (_hasFallen) return;
			
			RaycastHit hitInfo;
			bool hit = Physics.SphereCast(transform.position, _playerController.radius, Vector3.down, out hitInfo);
			if (!hit)
			{
				_hasFallen = true;
				StartCoroutine(fall());
			}
		}

		private IEnumerator fall()
		{
			DreamSystem.EndDream(fromFall: true);

			var lookUpRot = Quaternion.AngleAxis(MAX_X_ROTATION, -_targetCamera.transform.right) *
			                _targetCamera.transform.rotation;

			while (true)
			{
				_targetCamera.transform.rotation = Quaternion.RotateTowards(_targetCamera.transform.rotation, lookUpRot,
					ROTATION_SPEED * Time.deltaTime);
				yield return null;
			}
		}
	}
}