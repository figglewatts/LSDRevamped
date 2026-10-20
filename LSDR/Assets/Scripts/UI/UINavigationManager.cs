using System.Linq;
using LSDR.InputManagement;
using LSDR.UI.Modal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace LSDR.UI
{
    // keeps something selected when using a controller or keyboard, and skips disabled controls
    [RequireComponent(typeof(EventSystem), typeof(InputSystemUIInputModule))]
    public class UINavigationManager : MonoBehaviour
    {
        protected const float MOVE_REPEAT_DELAY = 0.35f;
        protected const int MAX_SKIPS = 20;

        public ControlSchemeLoaderSystem ControlScheme;

        protected EventSystem _eventSystem;
        protected InputSystemUIInputModule _inputModule;
        protected GameObject _lastSelected;
        protected Graphic _tintedCheckmark;
        protected GameObject _unhovered;

        // the frame a back button was last pressed by cancel
        public static int LastBackFrame { get; private set; } = -1;

        public void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
            _inputModule = GetComponent<InputSystemUIInputModule>();

            // don't lose the selection on a stray click
            _inputModule.deselectOnBackgroundClick = false;
            _inputModule.moveRepeatDelay = MOVE_REPEAT_DELAY;
        }

        public void OnEnable()
        {
            _inputModule.cancel.action.performed += onCancel;
            _inputModule.move.action.performed += onMove;
        }

        public void OnDisable()
        {
            _inputModule.cancel.action.performed -= onCancel;
            _inputModule.move.action.performed -= onMove;
        }

        // update links before the UI moves
        protected void onMove(InputAction.CallbackContext context)
        {
            UIAutoNavigation.RebuildAll();

            // moving up or down stops typing
            InputField typing = typingIn();
            if (typing != null && context.control.device is Gamepad && Mathf.Abs(context.ReadValue<Vector2>().y) > 0.5f)
            {
                typing.DeactivateInputField();
            }
        }

        // event triggers claim every event, even if just for a hover sound
        protected static bool handlesCancel(GameObject handler)
        {
            if (handler == null) return false;
            foreach (ICancelHandler cancelHandler in handler.GetComponents<ICancelHandler>())
            {
                if (cancelHandler is Dropdown) continue;
                if (cancelHandler is EventTrigger trigger &&
                    !trigger.triggers.Exists(entry => entry.eventID == EventTriggerType.Cancel)) continue;
                return true;
            }

            return false;
        }

        protected InputField typingIn()
        {
            GameObject selected = _eventSystem.currentSelectedGameObject;
            return selected != null && selected.TryGetComponent(out InputField field) && field.isFocused ? field : null;
        }

        // cancel presses the back button, unless the selection handles cancel itself
        protected void onCancel(InputAction.CallbackContext context)
        {
            // stop typing
            InputField typing = typingIn();
            if (typing != null)
            {
                typing.DeactivateInputField();
                return;
            }

            GameObject selected = _eventSystem.currentSelectedGameObject;
            if (selected != null && handlesCancel(ExecuteEvents.GetEventHandler<ICancelHandler>(selected))) return;

            // don't go back behind a modal
            Button back = UIBackButton.Current;
            GameObject modal = UIModalController.Open;
            if (back == null || (modal != null && !back.transform.IsChildOf(modal.transform))) return;
            LastBackFrame = Time.frameCount;
            back.onClick.Invoke();
        }

        public void Update()
        {
            GameObject current = _eventSystem.currentSelectedGameObject;
            if (ControlScheme.LastUsedDevice is Mouse)
            {
                _lastSelected = current;
                _unhovered = null;
                tintCheckmark(current);
                return;
            }

            // keep the selection inside an open modal
            GameObject modal = UIModalController.Open;
            if (modal != null && (current == null || !current.transform.IsChildOf(modal.transform)))
            {
                Selectable first = modal.GetComponentsInChildren<Selectable>()
                                        .FirstOrDefault(s => s.IsInteractable() && !(s is Scrollbar));
                current = first != null ? first.gameObject : null;
                _eventSystem.SetSelectedGameObject(current);
            }
            else if (current != _lastSelected && current != null && !isUsable(current)) current = skipPast(current);

            if (modal == null && !isUsable(current) && UISelectMe.CurrentDefault != null)
            {
                current = UISelectMe.CurrentDefault.gameObject;
            }

            if (current != _eventSystem.currentSelectedGameObject && isUsable(current))
            {
                _eventSystem.SetSelectedGameObject(current);
            }
            _lastSelected = _eventSystem.currentSelectedGameObject;
            tintCheckmark(_lastSelected);
            clearMouseHover();
        }

        // the checkmark covers the box, so tint it too
        protected void tintCheckmark(GameObject selected)
        {
            Toggle toggle = selected != null ? selected.GetComponent<Toggle>() : null;
            Graphic checkmark = toggle != null && toggle.transition == Selectable.Transition.ColorTint ? toggle.graphic : null;
            if (checkmark == _tintedCheckmark) return;

            if (_tintedCheckmark != null) _tintedCheckmark.CrossFadeColor(Color.white, 0, true, false, true);
            if (checkmark != null) checkmark.CrossFadeColor(toggle.colors.selectedColor, 0, true, false, true);
            _tintedCheckmark = checkmark;
        }

        // stop the hidden mouse cursor highlighting things
        protected void clearMouseHover()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null) return;

            GameObject hovered = _inputModule.GetLastRaycastResult(mouse.deviceId).gameObject;
            if (hovered == _unhovered) return;

            _unhovered = hovered;
            Selectable selectable = hovered != null ? hovered.GetComponentInParent<Selectable>() : null;
            if (selectable != null) selectable.OnPointerExit(new PointerEventData(_eventSystem));
        }

        // keep going past disabled controls
        protected GameObject skipPast(GameObject landedOn)
        {
            Vector2 direction = _inputModule.move.action.ReadValue<Vector2>();
            Selectable next = landedOn.GetComponent<Selectable>();
            for (int i = 0; i < MAX_SKIPS && next != null && direction != Vector2.zero; i++)
            {
                next = Mathf.Abs(direction.x) > Mathf.Abs(direction.y)
                    ? direction.x > 0 ? next.FindSelectableOnRight() : next.FindSelectableOnLeft()
                    : direction.y > 0 ? next.FindSelectableOnUp() : next.FindSelectableOnDown();
                if (next != null && isUsable(next.gameObject)) return next.gameObject;
            }

            return isUsable(_lastSelected) ? _lastSelected : null;
        }

        protected static bool isUsable(GameObject selected)
        {
            return selected != null && selected.activeInHierarchy &&
                   selected.TryGetComponent(out Selectable selectable) && selectable.IsInteractable();
        }
    }
}