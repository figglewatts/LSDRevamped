using System;
using System.Collections;
using LSDR.Game;
using LSDR.InputManagement;
using Torii.Binding;
using Torii.Coroutine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI.Settings
{
    public class UIVideoSettingsProfile : MonoBehaviour
    {
        [Header("Systems")]
        public ControlSchemeLoaderSystem ControlScheme;
        public SettingsSystem SettingsSystem;

        [Header("View")]
        public Text HintText;
        public Text ProfileNameText;
        public Button DeleteProfileButton;

        public void Start()
        {
            UpdateView();
        }

        public void OnEnable()
        {
            SettingsSystem.Settings.OnPropertyChange += onSettingsChange;
        }

        public void OnDisable()
        {
            SettingsSystem.Settings.OnPropertyChange -= onSettingsChange;
        }

        public void UpdateView()
        {
            HintText.text =
                $"Use '{ControlScheme.InputActions.Game.NextProfile.GetBindingDisplayString()}' in-game " +
                "to switch profiles";

            ProfileNameText.text = SettingsSystem.Settings.CurrentProfile.Name;
            if (!SettingsSystem.Settings.SettingsMatchProfile) ProfileNameText.text += "*";

            // we can only delete profiles when we have more than one remaining
            DeleteProfileButton.interactable = SettingsSystem.Settings.Profiles.Count > 1;
        }

        public void OnNextProfilePressed()
        {
            SettingsSystem.SwitchToNextProfile();
            UpdateView();
        }

        public void OnNewProfilePressed()
        {
            SettingsSystem.Settings.CreateNewProfile();
            UpdateView();
        }

        public void OnRevertProfilePressed()
        {
            SettingsSystem.RevertCurrentProfile();
            UpdateView();
        }

        public void OnDeleteProfilePressed()
        {
            SettingsSystem.DeleteCurrentProfile();
            UpdateView();
        }

        protected void onSettingsChange(string s, IPropertyWatcher watcher)
        {
            StartCoroutine(updateViewNextFrame());
        }

        protected IEnumerator updateViewNextFrame()
        {
            yield return null;
            UpdateView();
        }
    }
}
