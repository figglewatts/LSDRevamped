using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputManagement;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class UIControlSchemeNameVerifier : MonoBehaviour
{
    public Button Button;

    public bool CanHaveSameName
    {
        get { return _canHaveSameName; }
        set { _canHaveSameName = value; }
    }

    [SerializeField]
    private bool _canHaveSameName = false;

    [SerializeField]
    private InputField _inputField;

    void Awake()
    {
        _inputField = GetComponent<InputField>();
        _inputField.onValueChanged.AddListener(validateInput);
        Button.interactable = false;
    }

    public bool Validate(string input)
    {
        // scheme can't be empty
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        // scheme can't overwrite another scheme
        if (!CanHaveSameName && ControlSchemeManager.Schemes.Count(scheme => scheme.Name == input) > 0)
        {
            return false;
        }

        return true;
    }

    private void validateInput(string input)
    {
        Button.interactable = Validate(input);
    }
}
