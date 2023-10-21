using System;
using UnityEngine;

namespace LSDR.SDK.Visual
{
    public interface IFadeManager
    {
        void FadeIn(float duration, Action onFinish = null);

        void FadeIn(Color color, float duration, Action onFinish = null, float initialAlpha = -1);

        public void FadeOut(float duration, Action onFinish = null);

        public void FadeOut(Color color, float duration, Action onFinish = null, float initialAlpha = -1);
    }
}
