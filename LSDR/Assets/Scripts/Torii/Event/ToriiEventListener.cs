using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Torii.Event
{
    [AddComponentMenu("Torii/EventListener")]
    public class ToriiEventListener : MonoBehaviour
    {
        public ToriiEvent Event;
        
        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
        
        [MenuItem("GameObject/Torii/EventListener", false, 11)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject eventListenerObject = new GameObject("ToriiEventListener");
            GameObjectUtility.SetParentAndAlign(eventListenerObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(eventListenerObject, "Create " + eventListenerObject.name);
            Selection.activeObject = eventListenerObject;
        }
    }
}