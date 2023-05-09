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

        public void InteractiveRebind(IndexedActionBinding indexedBinding, Action onRebindFinish)
        {
            indexedBinding.InteractiveRebind(CancelAction,
                rebindOp =>
                {
                    // onPrepareRebind: show waiting for input modal, provide data
                    UIModalController.Instance.ShowModal(() =>
                    {
                        UIWaitingForInputModal waitingForInputModal = createWaitingForInputModal();
                        waitingForInputModal.ProvideExpectedInput(rebindOp.expectedControlType);
                        waitingForInputModal.ProvideCancelAction(CancelAction);
                        return waitingForInputModal.gameObject;
                    }, onRebindFinish);
                },
                () =>
                {
                    // onRebindSuccess: hide the waiting for input modal
                    UIModalController.Instance.HideModal();
                },
                () =>
                {
                    // onRebindCancel: hide the waiting for input modal
                    UIModalController.Instance.HideModal();
                }
            );
        }

        protected UIWaitingForInputModal createWaitingForInputModal()
        {
            return Instantiate(WaitingForInputModalPrefab).GetComponent<UIWaitingForInputModal>();
        }
    }
}
