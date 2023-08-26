using UnityEngine;

namespace LSDR.SDK.Visual
{
    [RequireComponent(typeof(Renderer))]
    public class ShaderSetter : MonoBehaviour
    {
        protected Renderer _renderer;

        public void Start()
        {
            _renderer = GetComponent<Renderer>();
            foreach (var mat in _renderer.sharedMaterials)
            {
                TextureSetter.Instance.RegisterMaterial(mat);
            }
        }
    }
}
