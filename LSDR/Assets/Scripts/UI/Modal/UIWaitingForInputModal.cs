using Torii.Event;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI.Modal
{
    public class UIWaitingForInputModal : MonoBehaviour
    {
        [Header("View")]
        public Text TitleText;
        public Text WaitingForInputText;
        public Text CancelText;

        protected InputAction _cancelAction;

        public void Update()
        {
            if (_cancelAction != null && _cancelAction.WasReleasedThisFrame()) UIModalController.Instance.HideModal();
        }

        public void HideSelf()
        {
            UIModalController.Instance.HideModal();
        }

        public void ProvideTitleText(string titleText)
        {
            if (string.IsNullOrWhiteSpace(titleText)) return;

            TitleText.text = $"Rebinding control for '{titleText}'";
        }

        public void ProvideExpectedInput(string expectedInput)
        {
            WaitingForInputText.text = $"Waiting for {expectedInput}";
        }

        public void ProvideCancelAction(InputAction cancelAction)
        {
            CancelText.text = $"Press {cancelAction.GetBindingDisplayString()} to cancel";
            _cancelAction = cancelAction;
        }
    }
}
