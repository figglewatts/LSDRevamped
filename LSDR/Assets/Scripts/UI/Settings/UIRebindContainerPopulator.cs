using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InControl;
using InputManagement;
using Torii.UI;
using UnityEngine;
using UnityEngine.UI;
using Text = UnityEngine.UI.Text;

/// <summary>
/// Populate the control rebind menu container.
/// </summary>
public class UIRebindContainerPopulator : ContainerPopulator
{
    /// <summary>
    /// A single item in the Rebind container.
    /// </summary>
    protected internal class RebindItem : MonoBehaviour
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public Text ActionName;
        
        /// <summary>
        /// The primary button to rebind.
        /// </summary>
        public Button ActionAButton;
        
        /// <summary>
        /// The secondary button to rebind.
        /// </summary>
        public Button ActionBButton;
    }

    public RectTransform Template;
    public Text TemplateActionName;
    public Button TemplateActionAButton;
    public Button TemplateActionBButton;

    /// <summary>
    /// Get/set the scheme we're currently editing.
    /// </summary>
    public ControlScheme EditingScheme
    {
        get { return _editingScheme; }
        set
        {
            _editingScheme = value;
            PopulateRebindContainer();
        }
    }

    [SerializeField] private ControlScheme _editingScheme;

    private bool _validTemplate = true;

    void Awake()
    {
        setupTemplate();
    }

    void Start()
    {
        if (_validTemplate) PopulateRebindContainer();
    }

    /// <summary>
    /// Populate the container with rebind actions.
    /// </summary>
    public void PopulateRebindContainer()
    {
        List<GameObject> population = new List<GameObject>();
        foreach (var action in _editingScheme.Actions.Actions)
        {
            population.Add(createRebindRow(action));
        }
        Populate(population);
    }

    // create a single rebinding row
    private GameObject createRebindRow(PlayerAction action)
    {
        RebindItem row = Instantiate(Template.gameObject, transform, true).GetComponent<RebindItem>();
        row.gameObject.SetActive(true);
        row.ActionName.text = action.Name;
        row.ActionAButton.GetComponentInChildren<Text>().text = getBindingName(action.Bindings, 0);
        row.ActionBButton.GetComponentInChildren<Text>().text = getBindingName(action.Bindings, 1);
        row.ActionAButton.onClick.AddListener(() =>
            rebindAction(action, row.ActionAButton, getBindingSource(action.Bindings, 0)));
        row.ActionBButton.onClick.AddListener(() =>
            rebindAction(action, row.ActionBButton, getBindingSource(action.Bindings, 1)));
        row.gameObject.SetActive(false);
        return row.gameObject;
    }

    // perform the rebind action
    private void rebindAction(PlayerAction action, Button rebindButton, BindingSource binding)
    {
        action.ListenOptions = ControlActions.DefaultListenOptions;
        action.ListenOptions.OnBindingAdded += (playerAction, source) => PopulateRebindContainer();
        action.ListenOptions.OnBindingRejected += (playerAction, source, rejectionType) =>
        {
            rebindButton.GetComponentInChildren<Text>().text = source.Name;
        };
        action.ListenOptions.OnBindingFound += (playerAction, source) =>
        {
            if (source == new KeyBindingSource(Key.Escape) ||
                source == new DeviceBindingSource(InputControlType.Select))
            {
                playerAction.StopListeningForBinding();
                rebindButton.GetComponentInChildren<Text>().text = binding != null ? binding.Name : "<unbound>";
                return false;
            }

            return true;
        };
        
        if (binding != null)
        {
            action.ListenForBindingReplacing(binding);
        }
        else
        {
            action.ListenForBinding();
        }

        rebindButton.GetComponentInChildren<Text>().text = "<rebinding>";
        Debug.Log($"Rebinding action: {action.Name}");
    }

    private BindingSource getBindingSource(ReadOnlyCollection<BindingSource> bindings, int index)
    {
        return bindings.Count > index ? bindings[index] : null;
    }

    private string getBindingName(ReadOnlyCollection<BindingSource> bindings, int index)
    {
        return bindings.Count > index ? bindings[index].Name : "<unbound>";
    }

    private void setupTemplate()
    {
        if (!Template)
        {
            Debug.LogError("The template is not assigned. Please give a template for this to work.");
            _validTemplate = false;
            return;
        }

        GameObject templateGo = Template.gameObject;
        templateGo.SetActive(true);
        RebindItem rebindItem = templateGo.AddComponent<RebindItem>();
        rebindItem.ActionName = TemplateActionName;
        rebindItem.ActionAButton = TemplateActionAButton;
        rebindItem.ActionBButton = TemplateActionBButton;
        templateGo.SetActive(false);
    }
}
