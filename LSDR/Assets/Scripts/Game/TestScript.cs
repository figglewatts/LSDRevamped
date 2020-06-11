using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities.Dream;
using LSDR.Entities.Original;
using Torii.Resource;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        public TODAnimator Animator;
        public DreamSystem DreamSystem;

        [ContextMenu("Test")]
        public void Test()
        {
            DreamSystem.ApplyTextureSet(TextureSet.Normal);
            LBD lbd = ResourceManager.Load<LBD>(PathUtil.Combine(Application.streamingAssetsPath, "original", "STG00",
                "M000.LBD"));
            InteractiveObject.Create(lbd, 0, DreamSystem.LBDLoader.LBDDiffuse, "Head guy", 1, true);
        }
    }
}
