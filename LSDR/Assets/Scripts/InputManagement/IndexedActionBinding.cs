using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

namespace LSDR.InputManagement
{
    /// <summary>
    ///     Used to store a single binding for an action (as well as any composite bindings and other metadata).
    ///     More generally used as internal representation of a rebindable binding on an action in our scaffolding around
    ///     the Unity input system.
    ///     Contains methods to perform various rebind actions on the binding.
    /// </summary>
    public struct IndexedActionBinding
    {
        public int Index;
        public InputBinding Binding;
        public List<InputBinding> CompositeBindings;
        public InputAction InputAction;

        public bool IsComposite => CompositeBindings?.Count > 0;

        public IEnumerable<int> CompositeBindingIndexes
        {
            get
            {
                for (int i = Index + 1;
                     i < InputAction.bindings.Count && InputAction.bindings[i].isPartOfComposite;
                     i++)
                {
                    yield return i;
                }
            }
        }

        public string GetDisplayString()
        {
            return InputAction.GetBindingDisplayStringNotEmpty(Binding);
        }

        public void InteractiveRebind(InputAction cancelAction,
            Action<InputActionRebindingExtensions.RebindingOperation, InputBinding> onPrepareRebinding,
            Action onRebindSuccess,
            Action onRebindCancel,
            bool gamepad)
        {
            InputControl cancelControl =
                cancelAction.controls.FirstOrDefault(c => gamepad ? c.device is Gamepad : c.device is Keyboard);

            // perform cleanup related to a rebinding operation
            void cleanup(InputActionRebindingExtensions.RebindingOperation op)
            {
                op.Dispose();
            }

            // perform a rebinding on an enumerator of binding indexes
            // hasComposite controls whether we recursively consume the enumerator to rebind composites
            //   if false, only one element from the enumerator will be consumed
            void performRebind(InputAction action, IEnumerator<int> bindingIndexes, bool hasComposite)
            {
                if (!bindingIndexes.MoveNext()) // if we have no more elements
                {
                    return;
                }
                InputActionRebindingExtensions.RebindingOperation rebindOp =
                    action.PerformInteractiveRebinding(bindingIndexes.Current)
                          .WithCancelingThrough(cancelControl)
                          .OnCancel(op =>
                           {
                               cleanup(op);
                               onRebindCancel?.Invoke();
                           })
                          .OnComplete(op =>
                           {
                               cleanup(op);
                               onRebindSuccess?.Invoke();
                               if (hasComposite) performRebind(action, bindingIndexes, hasComposite: true);
                           });
                onPrepareRebinding?.Invoke(rebindOp, action.bindings[bindingIndexes.Current]);
                rebindOp.Start();
            }

            IEnumerator<int> indexesToRebind = IsComposite
                ? CompositeBindingIndexes.GetEnumerator()
                : new List<int> { Index }.GetEnumerator();
            performRebind(InputAction, indexesToRebind, IsComposite);
        }

        public void ResetBinding()
        {
            if (IsComposite)
            {
                foreach (int compositeIndex in CompositeBindingIndexes)
                {
                    InputAction.RemoveBindingOverride(compositeIndex);
                }
            }

            InputAction.RemoveBindingOverride(Index);
        }

        public void DeleteBinding()
        {
            if (IsComposite)
            {
                foreach (int compositeIndex in CompositeBindingIndexes)
                {
                    InputAction.ApplyBindingOverride(compositeIndex, string.Empty);
                }
            }

            InputAction.ApplyBindingOverride(Index, string.Empty); // empty override disables this binding
        }
    }
}
