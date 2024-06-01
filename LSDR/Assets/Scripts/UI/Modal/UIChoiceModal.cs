using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Modal
{
    public class UIChoiceModal : MonoBehaviour
    {
        [Header("View")]
        public Text TitleText;
        public Text BodyText;

        public enum Result
        {
            Cancel = 0,
            Yes = 1,
        }

        public void SetText(string title, string body)
        {
            TitleText.text = title;
            BodyText.text = body;
        }

        public void OnYesPressed()
        {
            UIModalController.Instance.HideModal((int)Result.Yes);
        }

        public void OnCancelPressed()
        {
            UIModalController.Instance.HideModal((int)Result.Cancel);
        }
    }
}
