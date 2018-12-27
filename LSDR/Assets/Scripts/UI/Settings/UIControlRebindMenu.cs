using UnityEngine;
using System.Collections.Generic;
using InputManagement;

namespace UI
{
	public class UIControlRebindMenu : MonoBehaviour
	{
        /*
        public GameObject RebindButtonContainer;

		private List<GameObject> _instantiatedButtons = new List<GameObject>();

		// Use this for initialization
		void Start()
		{
			InstantiateInputButtons();
		}

		void OnEnable()
		{
			InstantiateInputButtons();
		}

		public void InstantiateInputButtons()
		{
			DestroyInstantiatedButtons();
			for (int i = 0; i < InputHandler.NumberOfInputs; i++)
			{
				InstantiateInputButton(InputHandler.Inputs[i], InputHandler.Controls[i].ToString());
			}
		}

		private void DestroyInstantiatedButtons()
		{
			foreach (GameObject button in _instantiatedButtons)
			{
				Destroy(button);
			}
		}

		private void InstantiateInputButton(string controlName, string inputName)
		{
			GameObject button = Instantiate(Resources.Load<GameObject>("Prefabs/UI/InputRebindButton"));
			button.transform.SetParent(RebindButtonContainer.transform, false);
			UIControlRebindElement script = button.GetComponent<UIControlRebindElement>();
			script.ControlName.text = controlName;
			script.InputName.text = inputName;
			_instantiatedButtons.Add(button);
		}*/
	}
}