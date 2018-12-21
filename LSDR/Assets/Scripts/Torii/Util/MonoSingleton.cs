using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Torii.Util
{
    /// <summary>
    /// A Singleton that inherits from MonoBehaviour allowing it to be attached to GameObjects.
    /// Create subclasses of this to use it.
    /// </summary>
    /// <typeparam name="T">The type of the Singleton. Should be same as subclass type.</typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static bool _initialised = false;

        private static T _instance = null;

        /// <summary>
        /// Get this singleton's instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                // if it was already created, return it
                if (_instance) return _instance;

                // if the object exists but we don't know about it, get it and return it
                _instance = FindObjectOfType<T>();
                if (_instance) return _instance;

                // if the object doesn't exist, create it
                _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                return _instance;
            }
        }

        /// <summary>
        /// Unity Awake() function. Checks for other instances and initialises singleton state.
        /// </summary>
        public void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning("Multiple instances of MonoSingleton detected! Comitting seppuku.");
                DestroyImmediate(this);
                return;
            }

            if (!_initialised)
            {
                Init();
                _initialised = true;
            }
        }

        /// <summary>
        /// Override this and put what you'd normally put in Awake() in here.
        /// Awake() is already in use to check for other instances and perform initialisation.
        /// </summary>
        public virtual void Init() { }
    }
}