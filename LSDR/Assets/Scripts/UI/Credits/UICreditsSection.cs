using System.Collections.Generic;
using System.Linq;
using Torii.Resource;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Credits
{
    public class UICreditsSection : MonoBehaviour
    {
        public RectTransform NamesContainer;
        public Text Title;
        public RawImage Image;
        public GameObject UICreditsSectionNamePrefab;
        public LayoutElement ImageLayoutElement;

        public void SetImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) return;

            Image.texture = ResourceManager.UnityLoad<Texture2D>(imagePath);
            Image.color = Color.white;
            float resizeRatio = ImageLayoutElement.minWidth / Image.texture.width;
            ImageLayoutElement.minHeight = Image.texture.height * resizeRatio;
        }

        public void SetTitle(string title) { Title.text = title; }

        public void PopulateNames(IEnumerable<string> names, bool sortNames)
        {
            if (sortNames) names = names.OrderBy(s => s);

            foreach (string n in names)
            {
                Text nameText = Instantiate(UICreditsSectionNamePrefab, NamesContainer).GetComponent<Text>();
                nameText.text = n;
            }
        }
    }
}
