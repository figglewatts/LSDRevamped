using System;
using System.Collections;
using Torii.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Torii.UI
{
    public class ToriiFader : MonoSingleton<ToriiFader>
    {
        private RawImage _fadeImage;
        private Canvas _fadeCanvas;

        private UnityEngine.Coroutine _currentFade;
        private Action _currentOnFinish;

        public override void Init()
        {
            GameObject fadeCanvas = new GameObject("FadeCanvas");
            fadeCanvas.transform.parent = transform;
            _fadeCanvas = fadeCanvas.AddComponent<Canvas>();
            _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = fadeCanvas.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            
            GameObject fadeImage = new GameObject("FadeImage");
            fadeImage.transform.parent = fadeCanvas.transform;
            _fadeImage = fadeImage.AddComponent<RawImage>();
            RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            _fadeImage.color = Color.clear;
        }

        public void FadeIn(float duration, Action onFinish = null)
        {
            beginFade(fadeTo(1, duration, onFinish));
        }

        public void FadeIn(Color color, float duration, Action onFinish = null, float initialAlpha = -1)
        {
            _fadeImage.color = new Color(color.r, color.g, color.b,
                Math.Abs(initialAlpha - (-1)) < float.Epsilon ? _fadeImage.color.a : initialAlpha);
            
            FadeIn(duration, onFinish);
        }

        public void FadeOut(float duration, Action onFinish = null)
        {
            beginFade(fadeTo(0, duration, onFinish));
        }

        public void FadeOut(Color color, float duration, Action onFinish = null, float initialAlpha = -1)
        {
            _fadeImage.color = new Color(color.r, color.g, color.b,
                Math.Abs(initialAlpha - (-1)) < float.Epsilon ? _fadeImage.color.a : initialAlpha);
            
            FadeOut(duration, onFinish);
        }

        private void beginFade(IEnumerator fade)
        {
            // if there was an existing fade, stop it and execute its callback if it existed
            if (_currentFade != null) StopCoroutine(_currentFade);
            _currentOnFinish?.Invoke();
            
            _currentFade = StartCoroutine(fade);
        }

        private IEnumerator fadeTo(float targetOpacity, float duration, Action onFinish)
        {
            // remember the current callback in case this fade is exited early
            _currentOnFinish = onFinish;

            // remember initial state
            float startOpacity = _fadeImage.color.a;
            Color color = _fadeImage.color;

            float t = 0;
            while (t < duration)
            {
                t += Time.deltaTime;

                // calculate the blend factor
                float blend = Mathf.Clamp01(t / duration);

                // blend
                color.a = Mathf.Lerp(startOpacity, targetOpacity, blend);
                _fadeImage.color = color;
                
                yield return null;
            }
            
            _currentOnFinish = null;
            _currentFade = null;
            onFinish?.Invoke();
        }
    }
}
