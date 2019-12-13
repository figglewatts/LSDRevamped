using UnityEngine;

namespace LSDR.Visual
{
    /// <summary>
    /// When attached to a GameObject, will disable the renderer when the camera is a certain distance away.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class CullMeshOnDistance : MonoBehaviour
    {
        private MeshRenderer _renderer;
        private Transform _mainCamera;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }
        
        private bool lazyLoadCamera()
        {
            if (_mainCamera != null) return true;
            
            if (Camera.main != null)
            {
                _mainCamera = Camera.main.transform;
                return true;
            }

            return false;
        }

        private void LateUpdate()
        {
            if (!lazyLoadCamera()) return;
            
            // don't consider height when looking to cull
            var position = transform.position;
            Vector3 thisPos = new Vector3(position.x, 0, position.z);
            var camPosition = _mainCamera.position;
            Vector3 camPos = new Vector3(camPosition.x, 0, camPosition.z);
            
            float distance = Vector3.Distance(thisPos, camPos);

            if (distance > RenderSettings.fogEndDistance)
            {
                _renderer.enabled = false;
            }
            else
            {
                _renderer.enabled = true;
            }
        }
    }
}
