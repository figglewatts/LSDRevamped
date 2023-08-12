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
            TextureSetter.Instance.RegisterMaterial(_renderer.sharedMaterial);
        }
    }
}
