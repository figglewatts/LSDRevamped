using System.Collections;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;

namespace LSDR.Entities.Player
{
    /// <summary>
    ///     Used to detect whether or not a player is currently falling.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerFallDetector : MonoBehaviour
    {
        private const float FALL_SPEED = 5f;
        private const float LOOK_DROP_SPEED = 6f;
        private const float LOOK_DISTANCE = 5f;
        public SettingsSystem Settings;
        public DreamSystem DreamSystem;

        [SerializeField] private Camera _targetCamera;

        private bool _hasFallen;
        private CharacterController _playerController;

        public void Awake() { _playerController = GetComponent<CharacterController>(); }

        public void Update()
        {
            if (!Settings.CanControlPlayer || !Settings.PlayerGravity || _hasFallen) return;

            RaycastHit hitInfo;
            bool hit = Physics.SphereCast(transform.position + Vector3.up * _playerController.radius * 2,
                _playerController.radius, Vector3.down,
                out hitInfo);
            if (!hit)
            {
                _hasFallen = true;
                StartCoroutine(fall());
            }
        }

        private IEnumerator fall()
        {
            DreamSystem.EndDream(fromFall: true);
            Settings.PlayerGravity = false;

            Quaternion startRotation = _targetCamera.transform.rotation;
            Vector3 tiltAxis = -_targetCamera.transform.right;
            float t = 0;
            while (true)
            {
                t += Time.deltaTime;
                _playerController.Move(Vector3.down * (FALL_SPEED * Time.deltaTime));

                // the view tilts up as the point it's looking at drops below it
                float tilt = Mathf.Atan2(LOOK_DROP_SPEED * t, LOOK_DISTANCE) * Mathf.Rad2Deg;
                _targetCamera.transform.rotation = Quaternion.AngleAxis(tilt, tiltAxis) * startRotation;
                yield return null;
            }
        }
    }
}
