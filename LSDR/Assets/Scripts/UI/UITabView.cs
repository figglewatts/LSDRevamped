using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    ///     Creates a UI view with 'tabs' enabling different content game objects.
    /// </summary>
    public class UITabView : MonoBehaviour
    {
        public List<Button> TabButtons = new List<Button>();
        public List<RectTransform> TabViews = new List<RectTransform>();

        public int StartOnTab;

        private int _currentTab;
        private ColorBlock[] _tabColours;

        public Button CurrentTabButton => TabButtons[_currentTab];

        public Selectable FirstUsableInCurrentTab() { return firstUsable(TabViews[_currentTab]); }

        public void Start()
        {
            if (TabButtons.Count != TabViews.Count)
            {
                Debug.LogError(
                    $"Tab buttons count ({TabButtons.Count}) did not equal tab views count ({TabViews.Count})!");
                return;
            }

            if (TabButtons.Count == 0)
            {
                Debug.LogWarning("Tab buttons count should not be equal to 0!");
                return;
            }

            if (TabViews.Count == 0)
            {
                Debug.LogWarning("Tab views count should not be equal to 0!");
                return;
            }

            if (StartOnTab >= TabViews.Count || StartOnTab >= TabButtons.Count)
            {
                Debug.LogError(
                    $"StartOnTab ({StartOnTab}) cannot be more than TabViews or TabButtons count ({TabViews.Count})!");
                return;
            }

            TabViews[StartOnTab].gameObject.SetActive(value: true);
            disableAllExcept(StartOnTab);

            for (int i = 0; i < TabButtons.Count; i++)
            {
                int iCopy = i;
                TabButtons[i].onClick.AddListener(() => onTabButtonClick(iCopy));
            }

            _tabColours = TabButtons.Select(b => b.colors).ToArray();
            highlightCurrentTab();
        }

        public void OnEnable()
        {
            TabViews[StartOnTab].gameObject.SetActive(value: true);
            disableAllExcept(StartOnTab);
            _currentTab = StartOnTab;
            highlightCurrentTab();
        }

        public void Update()
        {
            GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            if (selected == null || !isInTabView(selected.transform)) return;

            // the shoulder buttons switch tabs
            Gamepad gamepad = Gamepad.current;
            if (gamepad == null) return;
            if (gamepad.leftShoulder.wasPressedThisFrame) switchTab(-1);
            else if (gamepad.rightShoulder.wasPressedThisFrame) switchTab(1);
        }

        /// <summary>
        ///     Set all of the tab buttons to a state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetAllButtonsInteractable(bool state)
        {
            TabButtons.ForEach(button => button.interactable = state);
        }

        private void onTabButtonClick(int i)
        {
            TabViews[i].gameObject.SetActive(value: true);
            disableAllExcept(i);
            _currentTab = i;
            highlightCurrentTab();
        }

        // keep the current tab lit
        private void highlightCurrentTab()
        {
            if (_tabColours == null) return;

            for (int i = 0; i < TabButtons.Count; i++)
            {
                ColorBlock colours = _tabColours[i];
                if (i == _currentTab) colours.normalColor = colours.selectedColor;
                TabButtons[i].colors = colours;
            }
        }

        private void switchTab(int direction)
        {
            for (int step = 1; step < TabButtons.Count; step++)
            {
                int i = (_currentTab + direction * step + TabButtons.Count) % TabButtons.Count;
                if (!TabButtons[i].IsInteractable()) continue;

                TabButtons[i].onClick.Invoke();
                StartCoroutine(selectFirstNextFrame(i));
                return;
            }
        }

        private IEnumerator selectFirstNextFrame(int tab)
        {
            yield return null;
            Selectable first = firstUsable(TabViews[tab]);
            if (first != null) first.Select();
        }

        // not while a modal is up
        private bool isInTabView(Transform selected)
        {
            return TabButtons.Exists(b => b.transform == selected) ||
                   TabViews.Exists(v => v.gameObject.activeInHierarchy && selected.IsChildOf(v));
        }

        private static Selectable firstUsable(RectTransform view)
        {
            return view.GetComponentsInChildren<Selectable>()
                       .FirstOrDefault(s => s.IsInteractable() && s.navigation.mode != Navigation.Mode.None &&
                                            !(s is Scrollbar));
        }

        private void disableAllExcept(int thisOne)
        {
            for (int i = 0; i < TabViews.Count; i++)
            {
                if (i == thisOne) continue;
                TabViews[i].gameObject.SetActive(value: false);
            }
        }
    }
}
