using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace LSDR.InputManagement
{
    public class RebindableActions : IEnumerable<RebindableActions.ActionBindings>
    {
        protected readonly Dictionary<InputAction, List<IndexedActionBinding>> _store =
            new Dictionary<InputAction, List<IndexedActionBinding>>();

        public RebindableActions(InputActions inputActions)
        {
            refreshInternalRepresentation(inputActions);
        }

        public IEnumerator<ActionBindings> GetEnumerator()
        {
            return _store.Select(kvp => new ActionBindings { Action = kvp.Key, IndexedBindings = kvp.Value })
                         .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // the UI is built from an internel representation of the actions/bindings which is
        // built in this function
        // the internal representation helps group actions to bindings in a way that makes
        // hooking things up to the UI easier
        protected void refreshInternalRepresentation(InputActions inputActions)
        {
            string[] excludedActions =
            {
                "Look",
                "Pause"
            };

            // filter the actions to ones we want to be able to rebind
            IEnumerable<InputAction> actions = inputActions.Game.Get()
                                                           .Where(action => !excludedActions.Contains(action.name));
            _store.Clear(); // reset the internal representation
            foreach (InputAction action in actions)
            {
                // create list of bindings
                var bindings = new List<IndexedActionBinding>();
                _store[action] = bindings;
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    // add binding to representation
                    IndexedActionBinding indexedActionBinding = new IndexedActionBinding
                    {
                        Index = i,
                        Binding = action.bindings[i],
                        InputAction = action
                    };

                    // if it's composite, add all of the composite bindings to the representation
                    if (action.bindings[i].isComposite)
                    {
                        var composites = new List<InputBinding>();
                        while (++i < action.bindings.Count &&
                               action.bindings[i].isPartOfComposite) // guaranteed to be contiguous
                        {
                            composites.Add(action.bindings[i]);
                        }
                        i--; // reset i as we peeked it in the while loop (i++ would skip ahead 1 too far)
                        Assert.IsTrue(composites.Count > 0);
                        indexedActionBinding.CompositeBindings = composites;
                    }
                    bindings.Add(indexedActionBinding);
                }
            }
        }

        public struct ActionBindings
        {
            public InputAction Action;
            public List<IndexedActionBinding> IndexedBindings;
        }
    }
}
