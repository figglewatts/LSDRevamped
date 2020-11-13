using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities.Original;
using LSDR.Visual;
using Torii.Resource;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        public TODAnimator Animator;
        public DreamSystem DreamSystem;

        public void Start() { spawnObject(); }

        private void spawnObject()
        {
            LBD lbd = ResourceManager.Load<LBD>(PathUtil.Combine(Application.streamingAssetsPath, "original", "STG00",
                "M000.LBD"));

            var texSetOpts = TextureSetOptions.GetFromLBDPath(
                PathUtil.Combine(Application.streamingAssetsPath, "original", "STG00"),
                Shader.Find("LSDR/ClassicDiffuse"),
                Shader.Find("LSDR/RevampedDiffuse"));
            Material mat = new Material(DreamSystem.GetShader(alpha: false));
            DreamSystem.TextureSetSystem.RegisterMaterial(mat, texSetOpts);
            InteractiveObject.Create(lbd, 0, mat, "Head guy", 1, false, "lua/test.lua");
        }
    }
}
