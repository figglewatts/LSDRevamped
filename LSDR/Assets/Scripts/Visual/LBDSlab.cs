using UnityEngine;

namespace Visual
{
    public class LBDSlab : MonoBehaviour
    {
        public MeshFog[] MeshFog;
        public CullMeshOnDistance[] CullMesh;
        public MeshRenderer[] MeshRenderers;

        private Transform _mainCamera;

        private bool _active = false;
        private bool active
        {
            get { return _active; }
            set
            {
                foreach (MeshFog fog in MeshFog)
                {
                    fog.enabled = value;
                }

                foreach (CullMeshOnDistance cullMesh in CullMesh)
                {
                    cullMesh.enabled = value;
                }

                if (!value)
                {
                    foreach (MeshRenderer r in MeshRenderers)
                    {
                        r.enabled = false;
                    }
                }
                
                _active = value;
            }
        }

        private void Awake() { _mainCamera = Camera.main.transform; }

        private void Start() { active = false; }

        private void Update()
        {
            // don't consider height when looking to cull
            var position = transform.position;
            Vector3 thisPos = new Vector3(position.x, 0, position.z);
            var camPosition = _mainCamera.position;
            Vector3 camPos = new Vector3(camPosition.x, 0, camPosition.z);
            
            float distance = Vector3.Distance(thisPos, camPos);

            if (!active && distance < RenderSettings.fogEndDistance + 20)
            {
                Debug.Log("Setting active");
                active = true;
            }
            else if (active && distance >= RenderSettings.fogEndDistance + 20)
            {
                Debug.Log("Setting inactive");
                active = false;
            }
        }
    }
}
