using LSDR.SDK.DreamControl;
using LSDR.SDK.Visual;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Text")]
    public class TextSpecialDay : AbstractSpecialDay
    {
        public Texture2D OriginalTexture;

        [TextArea]
        public string TextJa;

        public override void HandleDay(int dayNumber)
        {
            SceneManager.LoadScene("text_dream");
            FadeManager.Managed.FadeOut(Color.black, 3f, () =>
            {
                TextSpecialDayControl control = FindObjectOfType<TextSpecialDayControl>();
                control.BeginTextDay(this);
            }, 1);
        }
    }
}
