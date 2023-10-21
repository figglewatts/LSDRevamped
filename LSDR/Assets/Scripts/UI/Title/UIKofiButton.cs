using UnityEngine;
using UnityEngine.EventSystems;

namespace LSDR.UI.Title
{
    public class UIKofiButton : MonoBehaviour, IPointerClickHandler
    {
        private readonly string KOFI_URL = "https://ko-fi.com/figglewatts";

        public void OnPointerClick(PointerEventData eventData)
        {
            Application.OpenURL(KOFI_URL);
        }
    }
}
