using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI
{
    // keeps the selection in view when not using the mouse
    [RequireComponent(typeof(ScrollRect))]
    public class UIScrollToSelection : MonoBehaviour
    {
        public ControlSchemeLoaderSystem ControlScheme;

        protected ScrollRect _scrollRect;
        protected readonly Vector3[] _corners = new Vector3[4];

        public void Awake() { _scrollRect = GetComponent<ScrollRect>(); }

        public void LateUpdate()
        {
            if (ControlScheme.LastUsedDevice is Mouse || EventSystem.current == null) return;

            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null || !selected.transform.IsChildOf(_scrollRect.content)) return;

            RectTransform viewport = _scrollRect.viewport != null
                ? _scrollRect.viewport
                : (RectTransform)_scrollRect.transform;

            // how far the selection is outside the viewport
            ((RectTransform)selected.transform).GetWorldCorners(_corners);
            float bottom = viewport.InverseTransformPoint(_corners[0]).y;
            float top = viewport.InverseTransformPoint(_corners[1]).y;
            Rect view = viewport.rect;

            float offset = 0;
            if (top > view.yMax) offset = top - view.yMax;
            else if (bottom < view.yMin) offset = bottom - view.yMin;
            if (Mathf.Approximately(offset, 0)) return;

            Vector2 position = _scrollRect.content.anchoredPosition;
            position.y -= offset;
            _scrollRect.content.anchoredPosition = position;
        }
    }
}