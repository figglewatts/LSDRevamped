using System;
using System.Collections.Generic;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI
{
    public class UIViewStack : MonoBehaviour
    {
        public float FadeDuration = 1;
        public GameObject InitialView;
        public int MaxViews = 2;
        private bool _currentlyTransitioning;

        private Stack<GameObject> _views;
        public GameObject Current => _views.Peek();

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

            Current.SetActive(value: false);
            view.SetActive(value: true);

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
            top.SetActive(value: false);
            Current.SetActive(value: true);
        }

        private void transitionBetween(GameObject from, GameObject to)
        {
            _currentlyTransitioning = true;
            ToriiFader.Instance.FadeIn(Color.black, FadeDuration, () =>
            {
                from.SetActive(value: false);
                to.SetActive(value: true);
                _currentlyTransitioning = false;
                ToriiFader.Instance.FadeOut(Color.black, FadeDuration);
            });
        }
    }
}
