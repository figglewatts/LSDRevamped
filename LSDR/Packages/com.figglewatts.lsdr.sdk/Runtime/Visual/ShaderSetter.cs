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
            Material[] materials = _renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = TextureSetter.Instance.RegisterMaterial(materials[i]);
            }
            _renderer.sharedMaterials = materials;
        }
    }
}
