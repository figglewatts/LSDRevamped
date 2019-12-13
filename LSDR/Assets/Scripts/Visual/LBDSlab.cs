using UnityEngine;

namespace LSDR.Visual
{
    /// <summary>
    /// Handles culling and fog of all tiles within an LBD.
    /// </summary>
    public class LBDSlab : MonoBehaviour
    {
        /// <summary>
        /// Array of MeshFog scripts in each of the tiles of the LBD.
        /// </summary>
        public MeshFog[] MeshFog;
        
        /// <summary>
        /// Array of CullMeshOnDistance scripts in each of the tiles of the LBD.
        /// </summary>
        public CullMeshOnDistance[] CullMesh;
        
        /// <summary>
        /// Array of MeshRenderers in each of the tiles of the LBD.
        /// </summary>
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
                
                if (value == false)
                {
                    foreach (MeshRenderer r in MeshRenderers)
                    {
                        r.enabled = false;
                    }
                }
                
                _active = value;
            }
        }

        private void Start()
        {
            active = false;
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            // don't consider height when looking to cull
            var position = transform.position;
            Vector3 thisPos = new Vector3(position.x, 0, position.z);
            var camPosition = _mainCamera.position;
            Vector3 camPos = new Vector3(camPosition.x, 0, camPosition.z);
            
            float distance = Vector3.Distance(thisPos, camPos);

            if (!active && distance < RenderSettings.fogEndDistance + 20)
            {
                active = true;
            }
            else if (active && distance >= RenderSettings.fogEndDistance + 20)
            {
                active = false;
            }
        }
    }
}
