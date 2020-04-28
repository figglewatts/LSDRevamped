using UnityEngine;

namespace LSDR.UI.Title
{
    public class UIPatreonButton : MonoBehaviour
    {
        private readonly string PATREON_URL = "https://www.patreon.com/figglewatts";
        
        public void OnClick()
        {
            Application.OpenURL(PATREON_URL);
        }
    }
}
