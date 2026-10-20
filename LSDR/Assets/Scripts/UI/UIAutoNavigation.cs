using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    // builds explicit navigation from on-screen positions, skipping anything unusable
    public class UIAutoNavigation : MonoBehaviour
    {
        private static readonly List<UIAutoNavigation> _active = new List<UIAutoNavigation>();

        private readonly List<Selectable> _found = new List<Selectable>();
        private readonly List<Selectable> _selectables = new List<Selectable>();
        private readonly List<Rect> _rects = new List<Rect>();
        private readonly Vector3[] _corners = new Vector3[4];
        private UITabView _tabView;

        public static void RebuildAll()
        {
            foreach (UIAutoNavigation navigation in _active) navigation.Rebuild();
        }

        public void Awake() { TryGetComponent(out _tabView); }

        public void OnEnable() { _active.Add(this); }

        public void OnDisable() { _active.Remove(this); }

        public void Rebuild()
        {
            collect();
            for (int i = 0; i < _selectables.Count; i++) link(i);
        }

        private void collect()
        {
            _selectables.Clear();
            _rects.Clear();
            GetComponentsInChildren(_found);
            foreach (Selectable selectable in _found)
            {
                if (!selectable.IsInteractable() || selectable.navigation.mode == Navigation.Mode.None) continue;

                // dropdown lists handle their own navigation
                if (selectable is Scrollbar) continue;
                Transform parent = selectable.transform.parent;
                if (parent != null && parent.GetComponentInParent<Dropdown>() != null) continue;

                ((RectTransform)selectable.transform).GetWorldCorners(_corners);
                _selectables.Add(selectable);
                _rects.Add(Rect.MinMaxRect(_corners[0].x, _corners[0].y, _corners[2].x, _corners[2].y));
            }
        }

        private void link(int i)
        {
            Selectable selectable = _selectables[i];
            Navigation navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = findVertical(i, up: true);
            navigation.selectOnDown = findVertical(i, up: false);

            // sliders use left/right for their value
            bool slider = selectable is Slider;
            navigation.selectOnLeft = slider ? null : findHorizontal(i, left: true);
            navigation.selectOnRight = slider ? null : findHorizontal(i, left: false);

            if (_tabView != null) linkTabs(selectable, ref navigation);
            selectable.navigation = navigation;
        }

        // tabs go down into their page, and the page goes back up to its tab
        private void linkTabs(Selectable selectable, ref Navigation navigation)
        {
            if (_tabView.TabButtons.Contains(selectable as Button))
            {
                navigation.selectOnDown = _tabView.FirstUsableInCurrentTab();
            }
            else if (_tabView.TabButtons.Contains(navigation.selectOnUp as Button))
            {
                navigation.selectOnUp = _tabView.CurrentTabButton;
            }
        }

        // nearest above/below, preferring the same column
        private Selectable findVertical(int from, bool up)
        {
            Rect rect = _rects[from];
            float tolerance = rect.height * 0.25f;
            Selectable best = null;
            float bestScore = float.MaxValue;
            for (int i = 0; i < _selectables.Count; i++)
            {
                if (i == from) continue;
                Rect other = _rects[i];
                float gap = up ? other.yMin - rect.yMax : rect.yMin - other.yMax;
                if (gap < -tolerance) continue;

                float sideways = horizontalGap(rect, other);
                float score = sideways > 0 ? 10000 + gap + sideways : Mathf.Max(gap, 0) + Mathf.Abs(other.center.x - rect.center.x) * 0.01f;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = _selectables[i];
                }
            }

            return best;
        }

        // nearest to the side on the same row
        private Selectable findHorizontal(int from, bool left)
        {
            Rect rect = _rects[from];
            Selectable best = null;
            float bestScore = float.MaxValue;
            for (int i = 0; i < _selectables.Count; i++)
            {
                if (i == from) continue;
                Rect other = _rects[i];
                float gap = left ? rect.xMin - other.xMax : other.xMin - rect.xMax;
                if (gap < 0 || verticalGap(rect, other) > 0) continue;

                if (gap < bestScore)
                {
                    bestScore = gap;
                    best = _selectables[i];
                }
            }

            return best;
        }

        private static float horizontalGap(Rect a, Rect b) { return Mathf.Max(a.xMin - b.xMax, b.xMin - a.xMax, 0); }

        private static float verticalGap(Rect a, Rect b) { return Mathf.Max(a.yMin - b.yMax, b.yMin - a.yMax, 0); }
    }
}