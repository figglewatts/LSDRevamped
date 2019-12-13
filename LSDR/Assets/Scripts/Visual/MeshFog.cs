using LSDR.Game;
using UnityEngine;

namespace LSDR.Visual
{
    /// <summary>
    /// When attached to a GameObject, will set its fog color based on distance from camera, and render settings.
    /// TODO: clean MeshFog up a bit
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class MeshFog : MonoBehaviour
    {
        public SettingsSystem Settings;

        private MeshRenderer _renderer;
        private MaterialPropertyBlock _propertyBlock;
        private Transform _mainCamera;
        private static readonly int _fogAmountID = Shader.PropertyToID("_FogAmount");

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (!_renderer.enabled) return; // if the renderer is disabled, we don't want to do anything
            
            _renderer.GetPropertyBlock(_propertyBlock);
            
            // don't consider height
            var position = transform.position;
            Vector3 thisPos = new Vector3(position.x, 0, position.z);
            var camPosition = _mainCamera.position;
            Vector3 camPos = new Vector3(camPosition.x, 0, camPosition.z);
            float distance = Vector3.Distance(thisPos, camPos);

            // calculate fog amount
            float fogAmt = (RenderSettings.fogEndDistance - distance) /
                           (RenderSettings.fogEndDistance - RenderSettings.fogStartDistance);
            fogAmt = Mathf.Clamp(fogAmt, 0, 1);
            
            // quantize fog amount
            fogAmt = Mathf.Round(fogAmt / 0.1f) * 0.1f;

            // set the fog amount
            _propertyBlock.SetFloat(_fogAmountID, fogAmt);
            _renderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
