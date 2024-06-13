using UnityEngine;
using UnityEngine.EventSystems;

namespace LSDR.UI.Title
{
    public class UISDKButton : MonoBehaviour, IPointerClickHandler
    {
        private readonly string KOFI_URL = "https://sdk.lsdrevamped.net";

        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL(KOFI_URL);
        }
    }
}
