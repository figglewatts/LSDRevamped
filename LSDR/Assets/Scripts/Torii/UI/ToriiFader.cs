using System;
using System.Collections;
using LSDR.SDK.Visual;
using Torii.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Torii.UI
{
    public class ToriiFader : MonoSingleton<ToriiFader>, IFadeManager
    {
        private UnityEngine.Coroutine _currentFade;
        private Action _currentOnFinish;
        private Canvas _fadeCanvas;
        private RawImage _fadeImage;

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

        public void SetSortOrder(int idx) { _fadeCanvas.sortingOrder = idx; }

        public void SetFade(Color color)
        {
            _fadeImage.color = color;
        }

        public void FadeIn(float duration, Action onFinish = null, bool forced = true)
        {
            beginFade(fadeTo(targetOpacity: 1, duration, onFinish), forced);
        }

        public void FadeIn(Color color, float duration, Action onFinish = null, float initialAlpha = -1,
            bool forced = true)
        {
            _fadeImage.color = new Color(color.r, color.g, color.b,
                Math.Abs(initialAlpha - -1) < float.Epsilon ? _fadeImage.color.a : initialAlpha);

            FadeIn(duration, onFinish, forced);
        }

        public void FadeOut(float duration, Action onFinish = null, bool forced = true)
        {
            beginFade(fadeTo(targetOpacity: 0, duration, onFinish), forced);
        }

        public void FadeOut(Color color, float duration, Action onFinish = null, float initialAlpha = -1,
            bool forced = true)
        {
            _fadeImage.color = new Color(color.r, color.g, color.b,
                Math.Abs(initialAlpha - -1) < float.Epsilon ? _fadeImage.color.a : initialAlpha);

            FadeOut(duration, onFinish, forced);
        }

        private void beginFade(IEnumerator fade, bool forced)
        {
            Debug.Log("beginning fade");

            // if there was an existing fade, stop it and execute its callback if it existed
            if (_currentFade != null)
            {
                Debug.Log("fade has _currentFade!=null");
                if (!forced)
                {
                    Debug.Log("this fade isn't forced, so we don't want to stop the current fade, returning");
                    return;
                } // don't force it unless we want to
                _currentOnFinish?.Invoke();
                StopCoroutine(_currentFade);
            }

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

            var finalCol = _fadeImage.color;
            finalCol.a = targetOpacity;
            _fadeImage.color = finalCol;

            _currentOnFinish = null;
            _currentFade = null;
            onFinish?.Invoke();
        }
    }
}
