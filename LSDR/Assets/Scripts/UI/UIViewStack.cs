using System;
using System.Collections.Generic;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI
{
    public class UIViewStack : MonoBehaviour
    {
        public GameObject Current => _views.Peek();
        public float FadeDuration = 1;
        public GameObject InitialView;
        public int MaxViews = 2;
        
        private Stack<GameObject> _views;
        private bool _currentlyTransitioning = false;

        public void Awake()
        {
            _views = new Stack<GameObject>();
        }

        public void Start()
        {
            if (InitialView == null)
            {
                throw new NullReferenceException("InitialView cannot be null!");
            }
            
            _views.Push(InitialView);
        }

        public void Push(GameObject view)
        {
            if (_currentlyTransitioning) return;
            
            if (_views.Count >= MaxViews)
            {
                return;
            }
            
            transitionBetween(Current, view);
            _views.Push(view);
        }

        public void PushWithoutTransition(GameObject view)
        {
            if (_currentlyTransitioning) return;
            
            if (_views.Count >= MaxViews)
            {
                return;
            }
            
            Debug.Log(view);
            Debug.Log(Current.activeSelf);
            Debug.Log(view.activeSelf);
            
            Current.SetActive(false);
            view.SetActive(true);
            
            Debug.Log(Current.activeSelf);
            Debug.Log(view.activeSelf);
            
            _views.Push(view);
        }

        public void Pop()
        {
            if (_currentlyTransitioning) return;
            
            if (_views.Count == 1)
            {
                Debug.LogWarning("Attempting to pop the initial view in the stack!");
                return;
            }

            GameObject top = _views.Pop();
            transitionBetween(top, Current);
        }

        public void PopWithoutTransition()
        {
            if (_currentlyTransitioning) return;
            
            if (_views.Count == 1)
            {
                Debug.LogWarning("Attempting to pop the initial view in the stack!");
                return;
            }

            GameObject top = _views.Pop();
            top.SetActive(false);
            Current.SetActive(true);
        }

        private void transitionBetween(GameObject from, GameObject to)
        {
            _currentlyTransitioning = true;
            ToriiFader.Instance.FadeIn(Color.black, FadeDuration, () =>
            {
                from.SetActive(false);
                to.SetActive(true);
                _currentlyTransitioning = false;
                ToriiFader.Instance.FadeOut(Color.black, FadeDuration);
            });
        }
    }
}