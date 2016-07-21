using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public static class Fader
	{
		private static GameObject _fadeTexture;
		private static Image _fadeImage;
		private static Animator _fadeAnimator;

		public delegate void FadeCompleteHandler();

		private static event FadeCompleteHandler _onFadeComplete;

		private static bool _currentlyFading;

		static Fader()
		{
			_fadeTexture = GameObject.FindGameObjectWithTag("FadeTexture");
			_fadeImage = _fadeTexture.GetComponent<Image>();
			_fadeAnimator = _fadeTexture.GetComponent<Animator>();
		}

		public static void SnapIn(Color c) { _fadeImage.color = new Color(c.r, c.g, c.b, 1); }

		public static void SnapOut() { _fadeImage.color = new Color(1, 1, 1, 0); }

		public static void FadeIn(float duration)
		{
			_fadeAnimator.speed = 1 / duration;
			_fadeAnimator.Play("FadeIn", -1, 0);
		}
		public static void FadeIn(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 0);
			FadeIn(duration);
		}
		public static void FadeIn(Color c, float duration, FadeCompleteHandler callback)
		{
			AddHandler(callback);
			FadeIn(c, duration);
		}

		public static void FadeOut(float duration)
		{
			_fadeAnimator.speed = 1/duration;
			_fadeAnimator.Play("FadeOut", -1, 0);
		}
		public static void FadeOut(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 1);
			FadeOut(duration);
		}
		public static void FadeOut(Color c, float duration, FadeCompleteHandler callback)
		{
			AddHandler(callback);
			FadeOut(c, duration);	
		}

		public static void InvokeOnFadeComplete() { if (_onFadeComplete != null) _onFadeComplete.Invoke(); }

		private static void AddHandler(FadeCompleteHandler h)
		{
			_onFadeComplete += h;
			_onFadeComplete += () => { _onFadeComplete = null; }; // remove it after first execution
		}
	}
}
