using System;
using System.Collections;
using Torii.Coroutine;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Fader is used to fade a color in/out of the entire screen.
	/// </summary>
	[Obsolete("LSDR.UI.Fader is obsolete, please use ToriiFader instead.")]
	public static class Fader
	{
		private static GameObject _fadeTexture;
		private static Image _fadeImage;
		private static Animator _fadeAnimator;

		private static Coroutine _onFadeCompleteCoroutine;
		private static Action _onFadeComplete;

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
		/*public static void FadeIn(float duration)
		{
			_fadeAnimator.speed = 1 / duration;
			_fadeAnimator.Play("FadeIn", -1, 0);
			_currentlyFading = true;
		}*/
		
		/// <summary>
		/// Fade in with a given color for a given duration.
		/// </summary>
		/// <param name="c">The color to fade in with.</param>
		/// <param name="duration">Duration in seconds to fade in.</param>
		/*public static void FadeIn(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 0);
			FadeIn(duration);
		}*/
		
		/// <summary>
		/// Fade in with a given color for a given duration, and execute a callback when complete.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		/// <param name="callback">The callback to run when complete.</param>
		/*public static void FadeIn(Color c, float duration, Action callback)
		{
			beginCallback(callback, duration);
			FadeIn(c, duration);
		}*/

		/// <summary>
		/// Fade out for a given duration in seconds.
		/// </summary>
		/// <param name="duration">Duration in seconds.</param>
		/*public static void FadeOut(float duration)
		{
			_fadeAnimator.speed = 1/duration;
			_fadeAnimator.Play("FadeOut", -1, 0);
			_currentlyFading = true;
		}*/
		
		/// <summary>
		/// Fade out from a given color for a given duration in seconds.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		/*public static void FadeOut(Color c, float duration)
		{
			_fadeImage.color = new Color(c.r, c.g, c.b, 1);
			FadeOut(duration);
		}*/
		
		/// <summary>
		/// Fade out from a given color for a given duration in seconds and execute a callback when complete.
		/// </summary>
		/// <param name="c">The color.</param>
		/// <param name="duration">The duration in seconds.</param>
		/// <param name="callback">The callback to execute when done.</param>
		/*public static void FadeOut(Color c, float duration, Action callback)
		{
			beginCallback(callback, duration);
			FadeOut(c, duration);
		}*/

		public static void ClearHandler() { _onFadeComplete = null; }

		private static void beginCallback(Action callback, float delay)
		{
			if (_onFadeCompleteCoroutine != null)
			{
				// if we call this again, stop the existing coroutine and invoke the callback immediately
				Coroutines.Instance.StopCoroutine(_onFadeCompleteCoroutine);
				_onFadeComplete();
			}
			
			_onFadeComplete = callback;
			_onFadeCompleteCoroutine = Coroutines.Instance.StartCoroutine(callbackAfterDelay(callback, delay));
		}

		private static IEnumerator callbackAfterDelay(Action callback, float delay)
		{
			yield return new WaitForSeconds(delay);
			callback();
		}
	}
}
