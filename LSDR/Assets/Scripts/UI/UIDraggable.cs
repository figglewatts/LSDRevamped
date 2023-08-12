using UnityEngine;
using UnityEngine.EventSystems;

namespace LSDR.UI
{
    /// <summary>
    ///     When attached to a gameobject, make a given transform draggable with the mouse.
    /// </summary>
    public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        ///     The target of this draggable.
        /// </summary>
        public Transform Target;

        /// <summary>
        ///     Should the target return?
        /// </summary>
        public bool ShouldReturn;

        /// <summary>
        ///     The camera's canvas.
        /// </summary>
        public Canvas CameraCanvas;

        private bool isMouseDown;
        private Vector3 startMousePosition;
        private Vector3 startPosition;

        private void Start()
        {
            if (!Target)
            {
                Target = gameObject.transform;
            }
        }

        private void Update()
        {
            if (isMouseDown)
            {
                Vector3 currentPosition = MousePosToCanvasPos(Input.mousePosition);

                Vector3 diff = currentPosition - startMousePosition;

                Vector3 pos = startPosition + diff;

                Target.position = pos;
            }
        }

        public void OnPointerDown(PointerEventData dt)
        {
            isMouseDown = true;

            startPosition = Target.position;
            startMousePosition = MousePosToCanvasPos(Input.mousePosition);
        }

        public void OnPointerUp(PointerEventData dt)
        {
            isMouseDown = false;

            if (ShouldReturn)
            {
                Target.position = startPosition;
            }
        }

        private Vector3 MousePosToCanvasPos(Vector3 mousePos)
        {
            Vector3 mouse = mousePos;
            mouse.z = CameraCanvas.planeDistance;
            return CameraCanvas.worldCamera.ScreenToWorldPoint(mouse);
        }
    }
}
