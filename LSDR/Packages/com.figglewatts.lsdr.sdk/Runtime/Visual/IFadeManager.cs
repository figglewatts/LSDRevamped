using System;
using UnityEngine;

namespace LSDR.SDK.Visual
{
    public interface IFadeManager
    {
        void FadeIn(float duration, Action onFinish = null, bool forced = true);

        void FadeIn(Color color, float duration, Action onFinish = null, float initialAlpha = -1, bool forced = true);

        public void FadeOut(float duration, Action onFinish = null, bool forced = true);

        public void FadeOut(Color color, float duration, Action onFinish = null, float initialAlpha = -1,
            bool forced = true);
    }
}
