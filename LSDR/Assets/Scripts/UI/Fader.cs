using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Fader is used to fade a color in/out of the entire screen.
	/// </summary>
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

		/// <summary>
		/// Set the screen to be a single color.
		/// </summary>
		/// <param name="c"></param>
		public static void SnapIn(Color c) { _fadeImage.color = new Color(c.r, c.g, c.b, 1); }

		/// <summary>
		/// Set the screen to be visible.
		/// </summary>
		public static void SnapOut() { _fadeImage.color = new Color(1, 1, 1, 0); }

		/// <summary>
		/// Fade in for a given duration.
		/// </summary>
		/// <param name="duration">Duration in seconds to fade in.</param>
		public static void FadeIn(float duration)
		{
			_fadeAnimator.speed = 1 / duration;
			_fadeAnimator.Play("FadeIn", -1, 0);
		}
		
		/// <summary>
		/// Fade in with a given color for a given duration.
		/// </summary>
		/// <param name="c">The color to fade in with.</param>
		/// <param name="duration">Duration in seconds to fade in.</param>
		public static void FadeIn(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 0);
			FadeIn(duration);
		}
		
		/// <summary>
		/// Fade in with a given color for a given duration, and execute a callback when complete.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		/// <param name="callback">The callback to run when complete.</param>
		public static void FadeIn(Color c, float duration, FadeCompleteHandler callback)
		{
			AddHandler(callback);
			FadeIn(c, duration);
		}

		/// <summary>
		/// Fade out for a given duration in seconds.
		/// </summary>
		/// <param name="duration">Duration in seconds.</param>
		public static void FadeOut(float duration)
		{
			_fadeAnimator.speed = 1/duration;
			_fadeAnimator.Play("FadeOut", -1, 0);
		}
		
		/// <summary>
		/// Fade out from a given color for a given duration in seconds.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		public static void FadeOut(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 1);
			FadeOut(duration);
		}
		
		/// <summary>
		/// Fade out from a given color for a given duration in seconds and execute a callback when complete.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		/// <param name="callback">The callback to execute when done.</param>
		public static void FadeOut(Color c, float duration, FadeCompleteHandler callback)
		{
			AddHandler(callback);
			FadeOut(c, duration);	
		}

		public static void InvokeOnFadeComplete() { if (_onFadeComplete != null) _onFadeComplete.Invoke(); }

		public static void ClearHandler() { _onFadeComplete = null; }

		private static void AddHandler(FadeCompleteHandler h)
		{
			_onFadeComplete += h;
			_onFadeComplete += () => { _onFadeComplete -= h; }; // remove it after first execution
		}
	}
}
