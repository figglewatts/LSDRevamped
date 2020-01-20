using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSDR.UI
{
    public class UISelectableMenu : Selectable
    {
        private Selectable _firstToSelect;
        
        protected override void Start() { _firstToSelect = GetComponentInChildren<Selectable>(); }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
            _firstToSelect.Select();
        }
    }
}
