using UnityEngine;
using System.Collections;
using Game;
using InputManagement;
using UnityEngine.UI;

namespace UI
{
	public class UIControlSchemeCreator : MonoBehaviour
	{
		public Toggle FpsMovementToggle;
		public Slider MouseSensitivitySlider;
		public InputField SchemeNameInputField;
		public Button CreateControlSchemeButton;
		public UIControlSchemeDropdown ControlSchemeDropdown;
		public UIControlRebindMenu ControlRebindMenu;

		public void OnEnable()
		{
			ControlSchemePicked(GameSettings.CurrentControlSchemeIndex);
			ControlSchemeDropdown.ControlSchemeDropdown.value = GameSettings.CurrentControlSchemeIndex;
		}

		public void SchemeNameFinishEdit()
		{
			// disable button if scheme name field is empty
			CreateControlSchemeButton.interactable = !SchemeNameInputField.text.Equals(string.Empty);
		}

		public void ControlSchemePicked(int i)
		{
			ControlSchemeManager.SwitchToScheme(i);
			FpsMovementToggle.isOn = ControlSchemeManager.CurrentScheme.FPSMovementEnabled;
			MouseSensitivitySlider.value = ControlSchemeManager.CurrentScheme.MouseSensitivity;
			SchemeNameInputField.text = ControlSchemeManager.CurrentScheme.SchemeName;
			ControlRebindMenu.InstantiateInputButtons();
		}

		public void CreateControlScheme()
		{
			ControlScheme scheme = new ControlScheme();
			scheme.SchemeName = SchemeNameInputField.text;
			scheme.FPSMovementEnabled = FpsMovementToggle.isOn;
			scheme.MouseSensitivity = MouseSensitivitySlider.value;
			scheme.Controls.Clear();
			int i = 0;
			foreach (string input in InputHandler.Inputs)
			{
				scheme.Controls.Add(input, InputHandler.Controls[i]);
				i++;
			}
			ControlSchemeManager.AddScheme(scheme);
			ControlSchemeManager.SerializeSchemes();
			ControlSchemeDropdown.UpdateDropdown();
		}
	}
}