using UnityEngine;
using InControl;
using LSDR.InputManagement;
using Torii.UI;

namespace LSDR.UI.Settings
{
	/// <summary>
	/// Menu used for creating a control scheme.
	/// </summary>
	public class UIControlSchemeCreator : MonoBehaviour
	{
	    public UnityEngine.UI.Button SubmitSchemeButton;
	    public UnityEngine.UI.Text SubmitSchemeButtonText;
	    public UnityEngine.UI.InputField SchemeNameField;
	    public UnityEngine.UI.Button CancelSchemeButton;

	    public UnityEngine.UI.Button CreateNewSchemeButton;
	    public UnityEngine.UI.Button EditExistingSchemeButton;

	    public Slider MouseSensitivitySlider;
	    public Toggle UseFpsControlsToggle;

	    public GameObject CreatorObject;
	    public GameObject SelectorObject;

	    public UIControlSchemeNameVerifier NameVerifier;
	    public UIRebindContainerPopulator RebindContainerPopulator;

	    public UITabView TopButtonsTabView;
	    public UnityEngine.UI.Button SettingsApplyButton;
	    public UnityEngine.UI.Button SettingsBackButton;

	    public UIControlSchemeDropdownPopulator ControlSchemeDropdownPopulator;

	    private ControlScheme _currentlyEditingScheme;

	    public enum ControlSchemeCreatorMode
	    {
            Create,
            Edit
	    }

        [HideInInspector]
	    public ControlSchemeCreatorMode Mode;

	    public void Start()
	    {
            MouseSensitivitySlider.onValueChanged.AddListener(mouseSensitivityOnValueChanged);
            UseFpsControlsToggle.onValueChanged.AddListener(useFpsControlsOnValueChanged);
            CreateNewSchemeButton.onClick.AddListener(() => Show(ControlSchemeCreatorMode.Create));
            EditExistingSchemeButton.onClick.AddListener(() => Show(ControlSchemeCreatorMode.Edit));
            SchemeNameField.onValueChanged.AddListener(schemeNameFieldOnValueChanged);
            SubmitSchemeButton.onClick.AddListener(SubmitScheme);
            CancelSchemeButton.onClick.AddListener(Hide);
	    }

	    /// <summary>
	    /// Show the control scheme creator in a certain mode.
	    /// </summary>
	    /// <param name="mode">Whether we're creating or editing.</param>
	    public void Show(ControlSchemeCreatorMode mode)
	    {
		    CreatorObject.SetActive(true);
            SelectorObject.SetActive(false);
	        Mode = mode;
	        switch (mode)
	        {
	            case ControlSchemeCreatorMode.Create:
	            {
		            _currentlyEditingScheme = new ControlScheme(ControlActions.CreateDefaultTank(), "NewScheme", false);
	                SubmitSchemeButtonText.text = "Create";
	                NameVerifier.CanHaveSameName = false;
	                SubmitSchemeButton.interactable = NameVerifier.Validate(_currentlyEditingScheme.Name);
	                SchemeNameField.interactable = true;
	                SchemeNameField.text = _currentlyEditingScheme.Name;
	                break;
	            }
	            case ControlSchemeCreatorMode.Edit:
	            {
	                _currentlyEditingScheme = ControlSchemeManager.Current;
	                SubmitSchemeButtonText.text = "Edit";
	                SubmitSchemeButton.interactable = true;
	                SchemeNameField.interactable = false;
	                SchemeNameField.text = ControlSchemeManager.Current.Name;
	                NameVerifier.CanHaveSameName = true;
	                break;
	            }
	        }
	        RebindContainerPopulator.EditingScheme = _currentlyEditingScheme;
	        UseFpsControlsToggle.isOn = _currentlyEditingScheme.FpsControls;
	        MouseSensitivitySlider.value = _currentlyEditingScheme.MouseSensitivity;
            TopButtonsTabView.SetAllButtonsInteractable(false);
	        SettingsApplyButton.interactable = false;
	        SettingsBackButton.interactable = false;
	    }

	    /// <summary>
	    /// Submit a control scheme to the ControlSchemeManager.
	    /// </summary>
	    public void SubmitScheme()
	    {
	        switch (Mode)
	        {
	            case ControlSchemeCreatorMode.Create:
	            {
	                ControlSchemeManager.Schemes.Add(_currentlyEditingScheme);
                    ControlSchemeManager.UseScheme(ControlSchemeManager.Schemes.Count - 1);
	                break;
	                
	            }
	            case ControlSchemeCreatorMode.Edit:
	            {
	                ControlSchemeManager.Schemes[ControlSchemeManager.CurrentSchemeIndex] = _currentlyEditingScheme;
	                break;
	            }
	        }
	        ControlSchemeManager.SerializeControlSchemes(ControlSchemeManager.Schemes);
            ControlSchemeManager.ReloadSchemes();
            ControlSchemeDropdownPopulator.PopulateDropdown();
	        Hide();
	    }

	    /// <summary>
	    /// Hide the creator menu and show the selector menu.
	    /// </summary>
	    public void Hide()
	    {
	        CreatorObject.SetActive(false);
	        SelectorObject.SetActive(true);
            TopButtonsTabView.SetAllButtonsInteractable(true);
	        SettingsApplyButton.interactable = true;
	        SettingsBackButton.interactable = true;
	    }

	    private void schemeNameFieldOnValueChanged(string value) { _currentlyEditingScheme.Name = value; }

	    private void mouseSensitivityOnValueChanged(float sensitivity)
	    {
	        _currentlyEditingScheme.MouseSensitivity = sensitivity;
	    }

	    private void useFpsControlsOnValueChanged(bool fpsControls)
	    {
	        // based on whether we're using FPS controls or not, we want the bindings to be different
	        // to support mouselook and strafing etc.
		    _currentlyEditingScheme.FpsControls = fpsControls;
	        if (fpsControls)
	        {
	            _currentlyEditingScheme.Actions.LookUp.ClearBindings();
	            _currentlyEditingScheme.Actions.LookUp.AddDefaultBinding(Mouse.PositiveY);
	            _currentlyEditingScheme.Actions.LookUp.AddDefaultBinding(InputControlType.RightStickUp);

	            _currentlyEditingScheme.Actions.LookDown.ClearBindings();
	            _currentlyEditingScheme.Actions.LookDown.AddDefaultBinding(Mouse.NegativeY);
	            _currentlyEditingScheme.Actions.LookDown.AddDefaultBinding(InputControlType.RightStickDown);

	            _currentlyEditingScheme.Actions.LookLeft.ClearBindings();
	            _currentlyEditingScheme.Actions.LookLeft.AddDefaultBinding(Mouse.NegativeX);
	            _currentlyEditingScheme.Actions.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);

	            _currentlyEditingScheme.Actions.LookRight.ClearBindings();
	            _currentlyEditingScheme.Actions.LookRight.AddDefaultBinding(Mouse.PositiveX);
	            _currentlyEditingScheme.Actions.LookRight.AddDefaultBinding(InputControlType.RightStickRight);
	        }
	        else
	        {
	            _currentlyEditingScheme.Actions.LookUp.ClearBindings();
	            _currentlyEditingScheme.Actions.LookUp.AddDefaultBinding(Key.E);
	            _currentlyEditingScheme.Actions.LookUp.AddDefaultBinding(InputControlType.Action4);

	            _currentlyEditingScheme.Actions.LookDown.ClearBindings();
	            _currentlyEditingScheme.Actions.LookDown.AddDefaultBinding(Key.Q);
	            _currentlyEditingScheme.Actions.LookDown.AddDefaultBinding(InputControlType.Action3);

                _currentlyEditingScheme.Actions.LookLeft.ClearBindings();
                _currentlyEditingScheme.Actions.LookRight.ClearBindings();
	        }
            RebindContainerPopulator.PopulateRebindContainer();
	    }
	}
}