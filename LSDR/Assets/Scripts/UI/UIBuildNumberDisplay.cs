using UnityEngine;
using System.Collections;
using AutoUpdate;
using UnityEngine.UI;

namespace UI
{
	public class UIBuildNumberDisplay : MonoBehaviour
	{
		public Text BuildNumberText;

        void Start()
        {
            BuildNumberText.text = typeof(UIBuildNumberDisplay).Assembly.GetName().Version.ToString();
        }
    }
}
