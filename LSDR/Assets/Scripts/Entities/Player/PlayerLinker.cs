using LSDR.Dream;
using UnityEngine;
using LSDR.Util;
using Torii.Util;

namespace LSDR.Entities.Player
{
	public class PlayerLinker : MonoBehaviour
	{
		public DreamSystem DreamSystem;

		public float LinkDelay = 0.7F;

		private bool _canLink = true;

		private float _linkTimer = 0F;

		private bool _touchingFloor;

		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!hit.gameObject.CompareTag("Linkable")) return;
			
			// if we're not touching a wall, reset the link delay timer
			// (this works because when you touch a wall and the floor, the collisions alternate)
			// basically, if we are touching the floor for 2 collisions in a row, we can reasonably
			// assume we are not also touching a wall
			if (_touchingFloor && hit.normal == Vector3.up) _linkTimer = 0;

			// remember if we were touching the floor on the last collision
			_touchingFloor = hit.normal == Vector3.up;

			// make sure we are facing the collision
			if (Vector3.Dot(transform.forward, hit.moveDirection) <= 0.75F) return;

			if (!_canLink) return;
			_linkTimer += Time.deltaTime;
			if (_linkTimer > LinkDelay)
			{
				_canLink = false;
				DreamSystem.Transition(RandUtil.RandColor());
			}
		}
	}
}