using System;
using System.Collections;
using UnityEngine;

namespace LSDR.SDK.Util
{
    public class NextFrameUtil : MonoBehaviour
    {
        private static NextFrameUtil _instance;

        /// <summary>
        ///     Get this singleton's instance.
        /// </summary>
        public static NextFrameUtil Instance
        {
            get
            {
                // if it was already created, return it
                if (_instance) return _instance;

                // if the object exists but we don't know about it, get it and return it
                _instance = FindObjectOfType<NextFrameUtil>();
                if (_instance) return _instance;

                // if the object doesn't exist, create it
                _instance = new GameObject("NextFrameUtil").AddComponent<NextFrameUtil>();
                DontDestroyOnLoad(_instance);
                return _instance;
            }
        }

        public void RunNextFrame(Action action)
        {
            StartCoroutine(nextFrame(action));
        }

        protected IEnumerator nextFrame(Action action)
        {
            yield return null;
            action();
        }
    }
}
