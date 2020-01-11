using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    /// Creates a UI view with 'tabs' enabling different content game objects.
    /// </summary>
    public class UITabView : MonoBehaviour
    {
        public List<Button> TabButtons = new List<Button>();
        public List<RectTransform> TabViews = new List<RectTransform>();

        public int StartOnTab = 0;

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

            TabViews[StartOnTab].gameObject.SetActive(true);
            disableAllExcept(StartOnTab);

            for (int i = 0; i < TabButtons.Count; i++)
            {
                var iCopy = i;
                TabButtons[i].onClick.AddListener(() => onTabButtonClick(iCopy));
            }
        }

        public void OnEnable()
        {
            TabViews[StartOnTab].gameObject.SetActive(true);
            disableAllExcept(StartOnTab);
        }

        /// <summary>
        /// Set all of the tab buttons to a state.
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetAllButtonsInteractable(bool state)
        {
            TabButtons.ForEach(button => button.interactable = state);
        }

        private void onTabButtonClick(int i)
        {
            TabViews[i].gameObject.SetActive(true);
            disableAllExcept(i);
        }

        private void disableAllExcept(int thisOne)
        {
            for (int i = 0; i < TabViews.Count; i++)
            {
                if (i == thisOne) continue;
                else TabViews[i].gameObject.SetActive(false);
            }
        }
    }
}
