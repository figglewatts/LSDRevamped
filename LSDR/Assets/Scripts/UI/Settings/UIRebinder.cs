using System;
using LSDR.InputManagement;
using LSDR.UI.Modal;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LSDR.UI.Settings
{
    public class UIRebinder : MonoBehaviour
    {
        public InputActionReference CancelAction;
        public GameObject WaitingForInputModalPrefab;
        public ControlSchemeLoaderSystem ControlScheme;

        public void Awake()
        {
            CancelAction.action.Enable();
        }

        public void InteractiveRebind(IndexedActionBinding indexedBinding, Action onRebindSuccess,
            Action onRebindCancel)
        {
            int bindingsCompleted = 0;
            indexedBinding.InteractiveRebind(CancelAction,
                (rebindOp, binding) =>
                {
                    // onPrepareRebind: show waiting for input modal, provide data
                    UIModalController.Instance.ShowModal(() =>
                    {
                        UIWaitingForInputModal waitingForInputModal = createWaitingForInputModal();
                        waitingForInputModal.ProvideTitleText(binding.name);
                        waitingForInputModal.ProvideExpectedInput(rebindOp.expectedControlType);
                        waitingForInputModal.ProvideCancelAction(CancelAction);
                        return waitingForInputModal.gameObject;
                    });
                },
                () =>
                {
                    // onRebindSuccess: hide the waiting for input modal
                    UIModalController.Instance.HideModal();
                    bindingsCompleted++;
                    if (!indexedBinding.IsComposite || bindingsCompleted >= indexedBinding.CompositeBindings.Count)
                    {
                        onRebindSuccess?.Invoke();
                    }
                },
                () =>
                {
                    // onRebindCancel: hide the waiting for input modal
                    UIModalController.Instance.HideModal();
                    onRebindCancel?.Invoke();
                },
                ControlScheme.LastUsedGamepad
            );
        }

        protected UIWaitingForInputModal createWaitingForInputModal()
        {
            return Instantiate(WaitingForInputModalPrefab).GetComponent<UIWaitingForInputModal>();
        }
    }
}
