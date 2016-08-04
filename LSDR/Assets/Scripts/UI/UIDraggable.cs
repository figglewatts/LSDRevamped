using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public Transform target;
		private bool isMouseDown = false;
		private Vector3 startMousePosition;
		private Vector3 startPosition;
		public bool shouldReturn;

		public Canvas CameraCanvas;

		// Use this for initialization
		void Start()
		{
			if (!target)
			{
				target = this.gameObject.transform;
			}
		}

		public void OnPointerDown(PointerEventData dt)
		{
			isMouseDown = true;

			startPosition = target.position;
			startMousePosition = MousePosToCanvasPos(Input.mousePosition);
		}

		public void OnPointerUp(PointerEventData dt)
		{
			isMouseDown = false;

			if (shouldReturn)
			{
				target.position = startPosition;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (isMouseDown)
			{
				Vector3 currentPosition = MousePosToCanvasPos(Input.mousePosition);

				Vector3 diff = currentPosition - startMousePosition;

				Vector3 pos = startPosition + diff;

				target.position = pos;
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