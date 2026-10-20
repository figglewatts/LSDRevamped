using System.Linq;
using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Toggle = Torii.UI.Toggle;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Menu used for creating a control scheme.
    /// </summary>
    public class UIControlSchemeCreator : MonoBehaviour
    {
        public enum ControlSchemeCreatorMode
        {
            Create,
            Edit
        }

        public GameObject CreatorObject;
        public GameObject SelectorObject;
        public ControlSchemeLoaderSystem ControlSchemeLoader;

        public Button SchemeEditButton;
        public Button SchemeDeleteButton;
        public Dropdown SchemeDropdown;

        public UIControlSchemeNameVerifier NameVerifier;
        public UIRebindContainerPopulator RebindContainerPopulator;
        public UIControlSchemeDropdownPopulator ControlSchemeDropdownPopulator;

        public UITabView TopButtonsTabView;

        protected ControlScheme _currentlyEditingScheme;
        protected ControlSchemeCreatorMode _mode;

        // don't start typing just from navigating onto it
        public void Awake() { SchemeNameField.shouldActivateOnSelect = false; }

        public void OnDisable() { Hide(); }

        public void ShowCreate() { Show(ControlSchemeCreatorMode.Create); }

        public void ShowEdit() { Show(ControlSchemeCreatorMode.Edit); }

        /// <summary>
        ///     Show the control scheme creator in a certain mode.
        /// </summary>
        /// <param name="mode">Whether we're creating or editing.</param>
        public void Show(ControlSchemeCreatorMode mode)
        {
            CreatorObject.SetActive(value: true);
            SelectorObject.SetActive(value: false);
            _currentlyEditingScheme = ControlSchemeLoader.Current;
            _mode = mode;
            switch (mode)
            {
                case ControlSchemeCreatorMode.Create:
                {
                    // default to a free name so it can be created without typing
                    _currentlyEditingScheme = new ControlScheme(ControlSchemeLoader.Current) { Name = freeSchemeName() };
                    break;
                }
            }

            UpdateView();

            TopButtonsTabView.SetAllButtonsInteractable(state: false);
            SettingsApplyButton.interactable = false;
            SettingsBackButton.interactable = false;

            selectForController(CreatorObject.GetComponentsInChildren<Selectable>().FirstOrDefault(s => s.IsInteractable()));
        }

        public void UpdateView()
        {
            if (_currentlyEditingScheme == null) return;

            RebindContainerPopulator.EditingScheme = _currentlyEditingScheme;
            UseFpsControlsToggle.isOn = _currentlyEditingScheme.FpsControls;

            SchemeEditButton.interactable = _currentlyEditingScheme.Editable;
            SchemeDeleteButton.interactable = _currentlyEditingScheme.Editable;

            SchemeDropdown.value = ControlSchemeLoader.CurrentSchemeIndex;

            switch (_mode)
            {
                case ControlSchemeCreatorMode.Create:
                    SubmitSchemeButtonText.text = "Create";
                    SubmitSchemeButton.interactable = NameVerifier.Validate(_currentlyEditingScheme.Name);
                    SchemeNameField.interactable = true;
                    SchemeNameField.text = _currentlyEditingScheme.Name;
                    NameVerifier.CanHaveSameName = false;
                    break;
                case ControlSchemeCreatorMode.Edit:
                    SubmitSchemeButtonText.text = "Edit";
                    SubmitSchemeButton.interactable = true;
                    NameVerifier.Button.interactable = true;
                    SchemeNameField.interactable = false;
                    SchemeNameField.text = ControlSchemeLoader.Current.Name;
                    NameVerifier.CanHaveSameName = true;
                    break;
            }
        }

        /// <summary>
        ///     Submit a control scheme to the ControlSchemeManager.
        /// </summary>
        public void SubmitScheme()
        {
            switch (_mode)
            {
                case ControlSchemeCreatorMode.Create:
                {
                    ControlSchemeLoader.CreateScheme(_currentlyEditingScheme, @select: true);
                    break;
                }
            }

            ControlSchemeLoader.SaveSchemes();
            ControlSchemeDropdownPopulator.PopulateDropdown();
            SchemeDropdown.value = ControlSchemeLoader.CurrentSchemeIndex;
            Hide();
        }

        public void DeleteScheme()
        {
            ControlSchemeLoader.DeleteScheme(_currentlyEditingScheme);
            ControlSchemeLoader.SaveSchemes();
            ControlSchemeDropdownPopulator.PopulateDropdown();
            SchemeDropdown.value = ControlSchemeLoader.CurrentSchemeIndex;
        }

        /// <summary>
        ///     Hide the creator menu and show the selector menu.
        /// </summary>
        public void Hide()
        {
            CreatorObject.SetActive(value: false);
            SelectorObject.SetActive(value: true);
            TopButtonsTabView.SetAllButtonsInteractable(state: true);
            SettingsApplyButton.interactable = true;
            SettingsBackButton.interactable = true;

            if (gameObject.activeInHierarchy) selectForController(SchemeDropdown);
        }

        protected void selectForController(Selectable selectable)
        {
            if (selectable == null || ControlSchemeLoader.LastUsedDevice is Mouse) return;
            selectable.Select();
        }

        protected string freeSchemeName()
        {
            for (int i = 1;; i++)
            {
                string name = $"Custom {i}";
                if (ControlSchemeLoader.Schemes.All(scheme => scheme.Name != name)) return name;
            }
        }

        public void SchemeDropdownFieldChanged(int idx)
        {
            _currentlyEditingScheme = ControlSchemeLoader.Schemes[idx];
            ControlSchemeLoader.SelectScheme(idx);
            UpdateView();
        }

        public void SchemeNameFieldOnValueChanged(string value)
        {
            if (_currentlyEditingScheme == null) return;
            _currentlyEditingScheme.Name = value;
        }

        public void UseFpsControlsOnValueChanged(bool fpsControls)
        {
            if (_currentlyEditingScheme == null) return;
            _currentlyEditingScheme.FpsControls = fpsControls;
        }

        public void InvertLookYOnValueChanged(bool invertLookY)
        {
            if (_currentlyEditingScheme == null) return;
            _currentlyEditingScheme.InvertLookY = invertLookY;
        }

#region Controls

        public Button SubmitSchemeButton;
        public Text SubmitSchemeButtonText;
        public InputField SchemeNameField;

        public Toggle UseFpsControlsToggle;

        public Button SettingsApplyButton;
        public Button SettingsBackButton;

#endregion
    }
}
