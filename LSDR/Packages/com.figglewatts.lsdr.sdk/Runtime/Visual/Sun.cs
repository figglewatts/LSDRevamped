using System;
using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Visual
{
    public class Sun : MonoBehaviour
    {
        public Texture2D SunTexture;
        public GameObject SunburstRayPrefab;

        protected const float SUNBURST_UPDATE_TIME_SECONDS = 0.1f;
        protected const int NUMBER_OF_SUNBURST_RAYS = 4;

        protected float _t = 0;
        protected Material _sunMaterial;
        protected MeshRenderer _sunRenderer;
        protected MeshRenderer[] _sunburstRays = new MeshRenderer[NUMBER_OF_SUNBURST_RAYS];
        protected bool _hasSunburst = false;
        protected float _sunburstScale = 1f;
        protected int _initialRenderOrder;

        public void Awake()
        {
            _sunRenderer = GetComponent<MeshRenderer>();
            _sunMaterial = new Material(Shader.Find("LSDR/Sun"));
            _sunMaterial.mainTexture = SunTexture;
            _sunRenderer.material = _sunMaterial;
            _initialRenderOrder = _sunMaterial.renderQueue;

            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                var ray = Instantiate(SunburstRayPrefab, _sunRenderer.transform);
                _sunburstRays[i] = ray.GetComponent<MeshRenderer>();
                _sunburstRays[i].enabled = false;
            }
            updateSunburst();
        }

        public void Update()
        {
            if (!_hasSunburst) return;

            if (_t > SUNBURST_UPDATE_TIME_SECONDS)
            {
                updateSunburst();
                _t = 0;
            }

            _t += Time.deltaTime;
        }

        public void SetScale(float scale)
        {
            _sunMaterial.SetFloat("_ScaleX", scale / 2f);
            _sunMaterial.SetFloat("_ScaleY", scale / 2f);
        }

        public void SetSunTintColour(Color color)
        {
            _sunMaterial.SetColor("_Tint", color);
        }

        public void SetSunburstTintColour(Color color)
        {
            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                _sunburstRays[i].material.SetColor("_Tint", color);
            }
        }

        public void SetRenderOrder(int order)
        {
            _sunMaterial.renderQueue = _initialRenderOrder + order;

            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                _sunburstRays[i].material.renderQueue = _initialRenderOrder + order - 1;
            }
        }

        public void ShowSunburst(float scale, Color color)
        {
            _hasSunburst = true;
            _sunburstScale = scale;
            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                _sunburstRays[i].enabled = true;
                _sunburstRays[i].material.SetColor("_Tint", color);
            }
            updateSunburst();
        }

        public void HideSunburst()
        {
            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                _sunburstRays[i].enabled = false;
            }
            _hasSunburst = false;
        }

        protected void updateSunburst()
        {
            for (int i = 0; i < NUMBER_OF_SUNBURST_RAYS; i++)
            {
                var ray = _sunburstRays[i];
                var longRay = RandUtil.OneIn(2);
                ray.material.SetFloat("_ScaleX", (longRay ? 4 : 2) * _sunburstScale);
                ray.material.SetFloat("_ScaleY", (1 / 16f) * _sunburstScale);
                ray.material.SetFloat("_RotationAngle", RandUtil.Float(0, 360));
            }
        }
    }
}
