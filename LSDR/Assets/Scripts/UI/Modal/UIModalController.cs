using System;
using System.Collections.Generic;
using Torii.Util;
using UnityEngine;

namespace LSDR.UI.Modal
{
    public class UIModalController : MonoSingleton<UIModalController>
    {
        public GameObject Background;

        protected readonly Stack<(Action, GameObject)> _modals = new Stack<(Action, GameObject)>();

        protected bool modalShowing
        {
            get => Background.activeSelf;
            set => Background.SetActive(value);
        }

        public void ShowModal(Func<GameObject> createModalFunc, Action onModalClose = null)
        {
            GameObject modalObj = createModalFunc();
            modalObj.transform.SetParent(transform, false);
            modalObj.SetActive(true);
            _modals.Push((onModalClose, modalObj));
            modalShowing = true;
        }

        public void HideModal()
        {
            if (_modals.Count <= 0)
            {
                Debug.LogWarning("HideModal(): Attempted to hide non-existent modal");
                return;
            }

            (Action onModalClose, GameObject modalObj) = _modals.Pop();
            onModalClose?.Invoke();
            Destroy(modalObj);

            if (_modals.Count <= 0)
            {
                modalShowing = false;
            }
        }
    }
}
