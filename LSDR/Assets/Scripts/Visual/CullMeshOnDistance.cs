using UnityEngine;

namespace Visual
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CullMeshOnDistance : MonoBehaviour
    {
        private MeshRenderer _renderer;
        private Transform _mainCamera;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
            _mainCamera = Camera.main.transform;
        }

        private void Update()
        {
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
