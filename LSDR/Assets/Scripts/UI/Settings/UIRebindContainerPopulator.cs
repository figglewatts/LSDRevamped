using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LSDR.InputManagement;
using Torii.UI;
using UnityEngine;
using UnityEngine.Assertions;
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
        public RectTransform BindingChoiceModal;
        public Button BindingChoiceButton;
        public Text BindingChoiceButtonText;
        public RectTransform WaitingForInputModal;
        public Text WaitingForInputText;
        public Text WaitingForInputCancelText;
        public RectTransform Template;
        public Text TemplateActionName;
        public Button TemplateRebindButton;
        public Button TemplateResetButton;
        public Button TemplateDeleteButton;

        protected readonly Dictionary<InputAction, List<IndexedBinding>> _bindingsInternalRepresentation =
            new Dictionary<InputAction, List<IndexedBinding>>();
        protected ControlScheme _editingScheme;
        protected bool _validTemplate = true;


        /// <summary>
        ///     Get/set the scheme we're currently editing.
        /// </summary>
        public ControlScheme EditingScheme
        {
            get => _editingScheme;
            set
            {
                _editingScheme = value;
                populateRebindContainer();
            }
        }

        protected void Awake() { setupRebindRowTemplate(); }

        protected void Start()
        {
            if (_validTemplate) populateRebindContainer();
        }

        [ContextMenu("Testing")]
        public void Testing()
        {
            refreshInternalRepresentation();

            foreach (KeyValuePair<InputAction, List<IndexedBinding>> kvp in _bindingsInternalRepresentation)
            {
                InputAction action = kvp.Key;
                List<IndexedBinding> actionBindings = kvp.Value;

                Debug.Log(action);
                foreach (IndexedBinding indexedBinding in actionBindings)
                {
                    Debug.Log(getBindingDisplayString(indexedBinding));
                }


                // Debug.Log(action);
                // Debug.Log(action.name);
                // Debug.Log(action.GetBindingIndex("Gamepad"));
                // Debug.Log(action.GetBindingIndex("Keyboard and mouse"));
                // Debug.Log(action.bindings[action.GetBindingIndex("Keyboard and mouse")].name);
                // Debug.Log(action.bindings[action.GetBindingIndex("Keyboard and mouse")].ToDisplayString());
                // Debug.Log(action.bindings[action.GetBindingIndex("Keyboard and mouse")].effectivePath);
                // Debug.Log(action.GetBindingDisplayString(InputBinding.DisplayStringOptions.DontUseShortDisplayNames));
                // Debug.Log("-------");
            }
        }

        // populate the container with the rows for each action
        protected void populateRebindContainer()
        {
            refreshInternalRepresentation();

            var population = new List<GameObject>();
            foreach (KeyValuePair<InputAction, List<IndexedBinding>> kvp in _bindingsInternalRepresentation)
            {
                InputAction action = kvp.Key;
                List<IndexedBinding> actionBindings = kvp.Value;
                population.Add(createRebindRow(action, actionBindings));
            }

            Populate(population);
        }

        // the UI is built from an internel representation of the actions/bindings which is
        // built in this function
        // the internal representation helps group actions to bindings in a way that makes
        // hooking things up to the UI easier
        protected void refreshInternalRepresentation()
        {
            string[] excludedActions =
            {
                "Look",
                "Pause"
            };

            // filter the actions to ones we want to be able to rebind
            IEnumerable<InputAction> actions = ControlScheme.InputActions.Game.Get()
                                                            .Where(action => !excludedActions.Contains(action.name));
            _bindingsInternalRepresentation.Clear(); // reset the internal representation
            foreach (InputAction action in actions)
            {
                // create list of bindings
                var bindings = new List<IndexedBinding>();
                _bindingsInternalRepresentation[action] = bindings;
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    // add binding to representation
                    IndexedBinding indexedBinding = new IndexedBinding
                    {
                        Index = i,
                        Binding = action.bindings[i],
                        Action = action
                    };
                    bindings.Add(indexedBinding);

                    // if it's composite, add all of the composite bindings to the representation
                    if (action.bindings[i].isComposite)
                    {
                        var composites = new List<InputBinding>();
                        while (++i < action.bindings.Count && action.bindings[i].isPartOfComposite) // guaranteed to be contiguous
                        {
                            composites.Add(action.bindings[i]);
                        }
                        i--; // reset i as we peeked it in the while loop (i++ would skip ahead 1 too far)
                        Assert.IsTrue(composites.Count > 0);
                        indexedBinding.CompositeBindings = composites;
                    }
                }
            }
        }

        // get the display string for a single binding
        protected string getBindingDisplayString(IndexedBinding binding)
        {
            string displayString = binding.Action.GetBindingDisplayString(binding.Binding, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
            if (string.IsNullOrEmpty(displayString.Trim()))
            {
                displayString = binding.Binding.effectivePath.Split(new[] { '/' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            displayString = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(displayString);
            return displayString;
        }

        // get the display string for the action overall (including a rundown of all bindings)
        protected string getActionDisplayString(InputAction action, List<IndexedBinding> actionBindings)
        {
            string actionName = action.name;
            string bindings = string.Join(", ", actionBindings.Select(getBindingDisplayString));
            return $"{actionName} ({bindings})";
        }

        // create a single rebinding row
        protected GameObject createRebindRow(InputAction action, List<IndexedBinding> actionBindings)
        {
            RebindRow row = Instantiate(Template.gameObject, transform, true).GetComponent<RebindRow>();
            row.gameObject.SetActive(true);
            row.ActionName.text = getActionDisplayString(action, actionBindings);
            row.Bindings = actionBindings;

            if (row.HasMultiple)
            {
                // row has multiple bindings, so we need to choose
                row.DeleteButton.gameObject.SetActive(false);
                row.ResetButton.gameObject.SetActive(false);

                //row.RebindButton.onClick.AddListener();
            }

            //row.RebindButton.onClick.AddListener(Act);
            // row.RebindButton.GetComponentInChildren<Text>().text = getBindingName(action.Bindings, 0);
            // row.RebindButton.onClick.AddListener(() =>
            //     rebindAction(action, row.ActionAButton, getBindingSource(action.Bindings, 0)));
            row.gameObject.SetActive(false);
            return row.gameObject;
        }

        protected void showBindingChoiceModal(List<IndexedBinding> bindingChoices) { }

        protected void showWaitingForBindingModal() { }

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
            templateGo.SetActive(true);
            RebindRow rebindRow = templateGo.AddComponent<RebindRow>();
            rebindRow.ActionName = TemplateActionName;
            rebindRow.RebindButton = TemplateRebindButton;
            rebindRow.ResetButton = TemplateResetButton;
            rebindRow.DeleteButton = TemplateDeleteButton;
            templateGo.SetActive(false);
        }

        /// <summary>
        ///     Used to store a single binding for an action (as well as any composite bindings and other metadata).
        /// </summary>
        protected struct IndexedBinding
        {
            public int Index;
            public InputBinding Binding;
            public List<InputBinding> CompositeBindings;
            public InputAction Action;

            public bool IsComposite => CompositeBindings?.Count > 0;
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

            public List<IndexedBinding> Bindings;

            public bool HasMultiple => Bindings?.Count > 0;
        }
    }
}
