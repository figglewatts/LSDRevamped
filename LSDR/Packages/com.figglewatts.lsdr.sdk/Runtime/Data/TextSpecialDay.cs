using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Text")]
    public class TextSpecialDay : AbstractSpecialDay
    {
        public Texture2D OriginalTexture;
        public string TextJa;

        public override void HandleDay(int dayNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
