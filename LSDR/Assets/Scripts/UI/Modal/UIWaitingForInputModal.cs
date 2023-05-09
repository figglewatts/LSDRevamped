using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI.Modal
{
    public class UIWaitingForInputModal : MonoBehaviour
    {
        [Header("View")]
        public Text WaitingForInputText;
        public Text CancelText;

        public void ProvideExpectedInput(string expectedInput)
        {
            WaitingForInputText.text = $"Waiting for {expectedInput}";
        }

        public void ProvideCancelAction(InputAction cancelAction)
        {
            CancelText.text = $"Press {cancelAction.GetBindingDisplayString()} to cancel";
        }
    }
}
