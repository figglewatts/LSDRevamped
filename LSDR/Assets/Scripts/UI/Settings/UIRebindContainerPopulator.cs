﻿using System.Collections.Generic;
using System.Linq;
using LSDR.InputManagement;
using LSDR.UI.Modal;
using Torii.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

namespace LSDR.UI.Settings
{
    /// <summary>
    ///     Populate the control rebind menu container.
    /// </summary>
    public class UIRebindContainerPopulator : ContainerPopulator
    {
        public ControlSchemeLoaderSystem ControlScheme;

        [Header("External stuff")]
        public UIRebinder Rebinder;
        public GameObject BindingChoiceModalPrefab;
        public RectTransform Template;
        public Text TemplateActionName;
        public Button TemplateRebindButton;
        public Button TemplateResetButton;
        public Button TemplateDeleteButton;

        protected ControlScheme _editingScheme;
        protected InputActions _inputActions;
        protected RebindableActions _internalRepresentation;
        protected bool _validTemplate = true;

        /// <summary>
        ///     Get/set the scheme we're currently editing.
        /// </summary>
        public ControlScheme EditingScheme
        {
            get => _editingScheme;
            set
            {
                Debug.Log("Setting editing scheme");
                _editingScheme = value;
                if (gameObject.activeInHierarchy) populateRebindContainer();
            }
        }

        protected void Awake() { setupRebindRowTemplate(); }

        protected void Start()
        {
            if (_validTemplate) populateRebindContainer();
        }

        protected UIBindingChoiceModal createBindingChoiceModal()
        {
            return Instantiate(BindingChoiceModalPrefab).GetComponent<UIBindingChoiceModal>();
        }

        protected void refreshInternalRepresentation()
        {
            if (_editingScheme == null) return;

            if (_inputActions == null) _inputActions = new InputActions();
            _inputActions.LoadBindingOverridesFromJson(_editingScheme.SchemeString);
            _internalRepresentation = new RebindableActions(_inputActions);
        }

        protected void syncBindingsToControlScheme()
        {
            _editingScheme.SyncToInputActions(_inputActions);
            populateRebindContainer();
        }

        // populate the container with the rows for each action
        protected void populateRebindContainer()
        {
            if (_editingScheme == null) return;

            refreshInternalRepresentation();

            var population = new List<GameObject>();
            foreach (RebindableActions.ActionBindings bindingList in _internalRepresentation)
            {
                population.Add(createRebindRow(bindingList));
            }

            Populate(population);
        }


        // get the display string for the action overall (including a rundown of all bindings)
        protected string getActionDisplayString(RebindableActions.ActionBindings actionBindings)
        {
            string actionName = actionBindings.Action.name;
            string bindings = string.Join(", ", actionBindings.IndexedBindings.Select(ab => ab.GetDisplayString()));
            return $"{actionName} ({bindings})";
        }

        // create a single rebinding row
        protected GameObject createRebindRow(RebindableActions.ActionBindings actionBindings)
        {
            RebindRow row = Instantiate(Template.gameObject).GetComponent<RebindRow>();
            row.gameObject.SetActive(value: true);
            row.ActionName.text = getActionDisplayString(actionBindings);
            row.Bindings = actionBindings.IndexedBindings;

            // hook up the rebinding buttons
            if (row.HasMultiple)
            {
                // InputAction has multiple bindings, hook up to present the user with a choice modal
                row.RebindButton.onClick.AddListener(() =>
                {
                    UIModalController.Instance.ShowModal(() =>
                    {
                        UIBindingChoiceModal bindingChoiceModal = createBindingChoiceModal();
                        bindingChoiceModal.ProvideActionBindings(actionBindings,
                            UIBindingChoiceModal.BindingChoiceType.Rebind);
                        bindingChoiceModal.ProvideCancelAction(Rebinder.CancelAction);
                        return bindingChoiceModal.gameObject;
                    }, _ => syncBindingsToControlScheme());
                });
                row.ResetButton.onClick.AddListener(() =>
                {
                    UIModalController.Instance.ShowModal(() =>
                    {
                        UIBindingChoiceModal bindingChoiceModal = createBindingChoiceModal();
                        bindingChoiceModal.ProvideActionBindings(actionBindings,
                            UIBindingChoiceModal.BindingChoiceType.Reset);
                        bindingChoiceModal.ProvideCancelAction(Rebinder.CancelAction);
                        return bindingChoiceModal.gameObject;
                    }, _ => syncBindingsToControlScheme());
                });
                row.DeleteButton.onClick.AddListener(() =>
                {
                    UIModalController.Instance.ShowModal(() =>
                    {
                        UIBindingChoiceModal bindingChoiceModal = createBindingChoiceModal();
                        bindingChoiceModal.ProvideActionBindings(actionBindings,
                            UIBindingChoiceModal.BindingChoiceType.Delete);
                        bindingChoiceModal.ProvideCancelAction(Rebinder.CancelAction);
                        return bindingChoiceModal.gameObject;
                    }, _ => syncBindingsToControlScheme());
                });
            }
            else
            {
                // InputAction has only single binding, perform actions directly without choice modal
                row.RebindButton.onClick.AddListener(() =>
                {
                    Rebinder.InteractiveRebind(actionBindings.IndexedBindings[index: 0], syncBindingsToControlScheme,
                        syncBindingsToControlScheme);
                });
                row.ResetButton.onClick.AddListener(() =>
                {
                    actionBindings.IndexedBindings[index: 0].ResetBinding();
                    syncBindingsToControlScheme();
                });
                row.DeleteButton.onClick.AddListener(() =>
                {
                    actionBindings.IndexedBindings[index: 0].DeleteBinding();
                    syncBindingsToControlScheme();
                });
            }
            return row.gameObject;
        }

        /// <summary>
        ///     Set up the
        /// </summary>
        protected void setupRebindRowTemplate()
        {
            if (!Template)
            {
                Debug.LogError("The template is not assigned. Please give a template for this to work.");
                _validTemplate = false;
                return;
            }

            _validTemplate = true;
            GameObject templateGo = Template.gameObject;
            RebindRow rebindRow = templateGo.AddComponent<RebindRow>();
            rebindRow.ActionName = TemplateActionName;
            rebindRow.RebindButton = TemplateRebindButton;
            rebindRow.ResetButton = TemplateResetButton;
            rebindRow.DeleteButton = TemplateDeleteButton;
            templateGo.SetActive(value: false);
        }


        /// <summary>
        ///     A row in the rebind container. Used for rebinding an entire action (which may have multiple bindings).
        /// </summary>
        protected class RebindRow : MonoBehaviour
        {
            /// <summary>
            ///     The name of the action.
            /// </summary>
            public Text ActionName;

            public Button RebindButton;

            public Button ResetButton;

            public Button DeleteButton;

            public List<IndexedActionBinding> Bindings;

            public bool HasMultiple => Bindings?.Count > 0;
        }
    }
}
