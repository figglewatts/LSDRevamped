using System;
using UnityEngine;

namespace LSDR.Visual
{
    [ExecuteInEditMode]
    public class FlashbackImageEffect : MonoBehaviour
    {
        protected Material _imageEffectMaterial;

        public void Awake()
        {
            _imageEffectMaterial = new Material(Shader.Find("Hidden/FlashbackImageEffect"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, _imageEffectMaterial);
        }
    }
}
