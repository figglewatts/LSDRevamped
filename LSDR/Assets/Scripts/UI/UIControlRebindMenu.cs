using UnityEngine;
using System.Collections;
using InputManagement;

namespace UI
{
	public class UIControlRebindMenu : MonoBehaviour
	{
		public GameObject RebindButtonContainer;

		// Use this for initialization
		void Start()
		{
			for (int i = 0; i < InputHandler.NumberOfInputs; i++)
			{
				InstantiateInputButton(InputHandler.Inputs[i], InputHandler.Controls[i].ToString());
			}
		}

		private void InstantiateInputButton(string controlName, string inputName)
		{
			GameObject button = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InputRebindButton"));
			button.transform.SetParent(RebindButtonContainer.transform, false);
			UIControlRebindElement script = button.GetComponent<UIControlRebindElement>();
			script.ControlName.text = controlName;
			script.InputName.text = inputName;
		}
	}
}