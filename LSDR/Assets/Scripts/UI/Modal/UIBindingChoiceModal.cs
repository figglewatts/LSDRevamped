using System;
using LSDR.InputManagement;
using LSDR.UI.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LSDR.UI.Modal
{
    public class UIBindingChoiceModal : MonoBehaviour
    {
        public enum BindingChoiceType
        {
            Rebind,
            Reset,
            Delete
        }

        [Header("View")]
        public Text TitleText;
        public Text CancelText;
        public RectTransform ButtonContainer;

        [Header("Other")]
        public GameObject BindingChoiceButtonPrefab;
        public UIRebinder Rebinder;

        protected InputAction _cancelAction;
        protected bool _canHideModal = true;

        protected void Update()
        {
            if (_canHideModal && _cancelAction != null && _cancelAction.WasReleasedThisFrame())
            {
                UIModalController.Instance.HideModal();
            }

        }

        public void ProvideCancelAction(InputAction cancelAction)
        {
            _cancelAction = cancelAction;
            CancelText.text = $"Press {cancelAction.GetBindingDisplayString()} to cancel";
        }

        public void ProvideActionBindings(RebindableActions.ActionBindings actionBindings,
            BindingChoiceType choiceType)
        {
            TitleText.text = $"Choose binding to {choiceTypeToString(choiceType)}";

            foreach (IndexedActionBinding binding in actionBindings.IndexedBindings)
            {
                createChoiceButton(binding, choiceType);
            }
        }

        protected GameObject createChoiceButton(IndexedActionBinding binding, BindingChoiceType choiceType)
        {
            GameObject buttonObj = Instantiate(BindingChoiceButtonPrefab, ButtonContainer, worldPositionStays: false);
            buttonObj.SetActive(value: true);
            Button button = buttonObj.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                switch (choiceType)
                {
                    case BindingChoiceType.Rebind:
                        _canHideModal = false;
                        Rebinder.InteractiveRebind(binding, () => UIModalController.Instance.HideModal(), () =>
                        {
                            _canHideModal = true;
                        });
                        break;
                    case BindingChoiceType.Reset:
                        binding.ResetBinding();
                        UIModalController.Instance.HideModal();
                        break;
                    case BindingChoiceType.Delete:
                        binding.DeleteBinding();
                        UIModalController.Instance.HideModal();
                        break;
                }
            });

            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            buttonText.text = binding.GetDisplayString();

            return buttonObj;
        }

        protected string choiceTypeToString(BindingChoiceType choiceType)
        {
            switch (choiceType)
            {
                case BindingChoiceType.Rebind:
                    return "change";
                case BindingChoiceType.Reset:
                    return "reset";
                case BindingChoiceType.Delete:
                    return "clear";
                default:
                    throw new ArgumentOutOfRangeException(nameof(choiceType), choiceType, message: null);
            }
        }
    }
}
