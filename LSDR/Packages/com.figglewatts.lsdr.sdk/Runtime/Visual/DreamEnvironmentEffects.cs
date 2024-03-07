using System;
using System.Collections.Generic;
using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Visual
{
    public class DreamEnvironmentEffects : MonoBehaviour
    {
        public GameObject SunPrefab;
        public GameObject CloudsPrefab;
        public GameObject StarsPrefab;
        public bool Created { get; protected set; }

        protected List<GameObject> _managedEffectObjects = new();
        protected int _numberOfSuns = 0;

        public struct SunburstConfig
        {
            public float Scale;
            public Color Color;
        }

        public void Start()
        {
            randomiseRotation();
        }

        public void ClearEffects()
        {
            foreach (var obj in _managedEffectObjects)
            {
                Destroy(obj);
            }
            _managedEffectObjects.Clear();
            Created = false;
        }

        public void CreateClouds(int cloudCount, Color cloudColor)
        {
            Created = true;
            for (int i = 0; i < cloudCount; i++)
            {
                var cloudObject = Instantiate(CloudsPrefab, transform);
                cloudObject.transform.localPosition = new Vector3(RandUtil.Float(-8, 8), RandUtil.Float(2, 3),
                    RandUtil.Float(-8, 8));

                var cloudMaterial = cloudObject.GetComponent<MeshRenderer>().material;
                cloudMaterial.SetColor("_Tint", cloudColor);

                float scale = RandUtil.Float(1, 4);
                cloudObject.transform.localScale = new Vector3(scale, scale, scale);

                float yaw = RandUtil.Float(0, 360);
                cloudObject.transform.Rotate(Vector3.up, yaw, Space.World);

                _managedEffectObjects.Add(cloudObject);
            }
        }

        public void CreateStars(int starCount, Color starColor)
        {
            Created = true;
            for (int i = 0; i < starCount; i++)
            {
                var starsObject = Instantiate(StarsPrefab, transform);
                starsObject.transform.localPosition = new Vector3(RandUtil.Float(-8, 8), RandUtil.Float(2, 3),
                    RandUtil.Float(-8, 8));

                var cloudMaterial = starsObject.GetComponent<MeshRenderer>().material;
                cloudMaterial.SetColor("_Tint", starColor);

                float scale = RandUtil.Float(1, 4);
                starsObject.transform.localScale = new Vector3(scale, scale, scale);

                float yaw = RandUtil.Float(0, 360);
                starsObject.transform.Rotate(Vector3.up, yaw, Space.World);

                _managedEffectObjects.Add(starsObject);
            }
        }

        public void CreateSun(Color color, float scale, float height, float offset,
            SunburstConfig? sunburstConfig = null)
        {
            Created = true;
            var sunObject = Instantiate(SunPrefab, transform);
            sunObject.transform.localPosition = new Vector3(5, height, offset);

            var sun = sunObject.GetComponent<Sun>();
            sun.SetRenderOrder(_numberOfSuns++);
            sun.SetSunTintColour(color);
            sun.SetScale(scale);

            if (sunburstConfig != null)
            {
                sun.ShowSunburst(sunburstConfig.Value.Scale, sunburstConfig.Value.Color);
            }

            _managedEffectObjects.Add(sunObject);
        }

        protected void randomiseRotation()
        {
            transform.Rotate(0, RandUtil.Float(0, 360), 0);
        }
    }
}
