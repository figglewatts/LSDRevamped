﻿using LSDR.Game;
using Torii.Binding;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Menu for setting gameplay settings.
    /// </summary>
    public class UIGameplaySettings : MonoBehaviour
    {
        public SettingsSystem Settings;
        public GameSaveSystem GameSave;

        public Toggle EnableHeadBobToggle;
        public Toggle SmoothHeadBobToggle;
        public Toggle EnableFootstepSoundsToggle;
        public Dropdown CurrentJournalDropdown;
        public Dropdown CurrentModDropdown;
        public Slider HeadbobIntensitySlider;
        public UIJournalDropdownPopulator JournalDropdownPopulator;
        public Toggle SpecialDaysEnabledToggle;

        public void Start()
        {
            Settings.SettingsBindBroker.RegisterData(EnableHeadBobToggle);
            Settings.SettingsBindBroker.RegisterData(EnableFootstepSoundsToggle);
            Settings.SettingsBindBroker.RegisterData(CurrentJournalDropdown);
            Settings.SettingsBindBroker.RegisterData(CurrentModDropdown);
            Settings.SettingsBindBroker.RegisterData(HeadbobIntensitySlider);
            Settings.SettingsBindBroker.RegisterData(SmoothHeadBobToggle);
            Settings.SettingsBindBroker.RegisterData(SpecialDaysEnabledToggle);

            EnableHeadBobToggle.isOn = Settings.Settings.HeadBobEnabled;
            EnableFootstepSoundsToggle.isOn = Settings.Settings.EnableFootstepSounds;
            CurrentJournalDropdown.value = Settings.Settings.CurrentJournalIndex;
            CurrentModDropdown.value = Settings.Settings.CurrentModIndex;
            HeadbobIntensitySlider.value = Settings.Settings.HeadBobIntensity;
            SmoothHeadBobToggle.isOn = Settings.Settings.SmoothHeadBob;
            SpecialDaysEnabledToggle.isOn = Settings.Settings.SpecialDaysEnabled;

            CurrentModDropdown.onValueChanged.AddListener(_ =>
            {
                updateMod();
                updateJournal();
            });

            Settings.SettingsBindBroker.Bind(() => EnableHeadBobToggle.isOn,
                () => Settings.Settings.HeadBobEnabled, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => EnableFootstepSoundsToggle.isOn,
                () => Settings.Settings.EnableFootstepSounds, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => CurrentJournalDropdown.value,
                () => Settings.Settings.CurrentJournalIndex, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => CurrentModDropdown.value, () => Settings.Settings.CurrentModIndex,
                BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => HeadbobIntensitySlider.value,
                () => Settings.Settings.HeadBobIntensity, BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => SmoothHeadBobToggle.isOn, () => Settings.Settings.SmoothHeadBob,
                BindingType.TwoWay);
            Settings.SettingsBindBroker.Bind(() => SpecialDaysEnabledToggle.isOn,
                () => Settings.Settings.SpecialDaysEnabled, BindingType.TwoWay);
        }

        protected void updateMod()
        {
            GameSave.Load();
        }

        protected void updateJournal()
        {
            CurrentJournalDropdown.value = 0;
            JournalDropdownPopulator.PopulateDropdownWithJournals();
        }
    }
}
