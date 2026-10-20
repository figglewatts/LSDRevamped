using System;
using System.Collections.Generic;
using Torii.Util;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LSDR.UI.Modal
{
    public class UIModalController : MonoSingleton<UIModalController>
    {
        public GameObject Background;

        public delegate void OnModalCloseAction(int result = 0);

        protected readonly Stack<(OnModalCloseAction, GameObject)> _modals = new();

        // selection to restore when each modal closes
        protected readonly Stack<GameObject> _selectedBefore = new();

        public static GameObject Open { get; private set; }

        protected bool modalShowing
        {
            get => Background.activeSelf;
            set => Background.SetActive(value);
        }

        public void ShowModal(Func<GameObject> createModalFunc, OnModalCloseAction onModalClose = null)
        {
            GameObject modalObj = createModalFunc();
            modalObj.transform.SetParent(transform, worldPositionStays: false);
            if (!modalObj.TryGetComponent(out UIAutoNavigation _)) modalObj.AddComponent<UIAutoNavigation>();
            modalObj.SetActive(value: true);
            _modals.Push((onModalClose, modalObj));
            _selectedBefore.Push(EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null);
            Open = modalObj;
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
            Open = _modals.Count > 0 ? _modals.Peek().Item2 : null;
            GameObject selectedBefore = _selectedBefore.Pop();
            if (selectedBefore != null && selectedBefore.activeInHierarchy && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(selectedBefore);
            }

            onModalClose?.Invoke(result);
            Destroy(modalObj);

            if (_modals.Count <= 0)
            {
                modalShowing = false;
            }
        }
    }
}
