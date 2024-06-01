using System;
using System.Collections.Generic;
using Torii.Util;
using UnityEngine;

namespace LSDR.UI.Modal
{
    public class UIModalController : MonoSingleton<UIModalController>
    {
        public GameObject Background;

        public delegate void OnModalCloseAction(int result = 0);

        protected readonly Stack<(OnModalCloseAction, GameObject)> _modals = new();

        protected bool modalShowing
        {
            get => Background.activeSelf;
            set => Background.SetActive(value);
        }

        public void ShowModal(Func<GameObject> createModalFunc, OnModalCloseAction onModalClose = null)
        {
            GameObject modalObj = createModalFunc();
            modalObj.transform.SetParent(transform, worldPositionStays: false);
            modalObj.SetActive(value: true);
            _modals.Push((onModalClose, modalObj));
            modalShowing = true;
        }

        public void HideModal(int result = 0)
        {
            if (_modals.Count <= 0)
            {
                Debug.LogWarning("HideModal(): Attempted to hide non-existent modal");
                return;
            }

            (OnModalCloseAction onModalClose, GameObject modalObj) = _modals.Pop();
            onModalClose?.Invoke(result);
            Destroy(modalObj);

            if (_modals.Count <= 0)
            {
                modalShowing = false;
            }
        }
    }
}
