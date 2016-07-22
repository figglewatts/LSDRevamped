using System;
using UnityEngine;
using System.Collections;
using Entities.Dream;
using Entities.WorldObject;
using Game;
using UI;
using Util;

namespace Entities.Player
{
	public class PlayerLinker : MonoBehaviour
	{
		public float LinkDelay = 0.7F;

		private bool _canLink = true;

		private float _linkTimer = 0F;

		// TODO: hook this up to global SFX volume
		private AudioSource _source;
		private AudioClip _linkSound;

		private bool _touchingFloor;

		// Use this for initialization
		void Start()
		{
			_source = GetComponent<AudioSource>();
			_linkSound = Resources.Load<AudioClip>("Sound/Dream/linkSound");
		}

		void Update()
		{
			//Debug.Log(_touchingFloor);
		}

		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!hit.gameObject.tag.Equals("Linkable")) return;
			
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
				_source.PlayOneShot(_linkSound);
				// TODO: link to specific level
				// TODO: link with forced color

				LinkableObject o = hit.gameObject.transform.parent.transform.parent.GetComponent<LinkableObject>();

				Color linkCol = o.ForceFadeColor ? o.FadeColor : RandUtil.RandColor();
				string linkLevel = o.LinkToSpecificLevel ? IOUtil.PathCombine("levels", o.LinkedLevel) : RandUtil.RandomLevelFromDir(GameSettings.CurrentJournalDir);

				_linkTimer = 0;
				Link(linkLevel, linkCol);
			}
		}

		public void Link(string dreamFilePath, Color color, string spawnName = "")
		{
			GameSettings.CanControlPlayer = false;
			_canLink = false;
			Fader.FadeIn(color, 1F, () =>
			{
				DreamDirector.SwitchDreamLevel(dreamFilePath, spawnName);
				GameSettings.CanControlPlayer = true;
				Fader.FadeOut(color, 1F, () =>
				{
					Debug.Log("Finished linking");
					_canLink = true;
				});
			});
		}

		public void Link(string dreamFilePath) { Link(dreamFilePath, RandUtil.RandColor()); }

		public void Link()
		{
			// TODO: random dream selection from current journal
			throw new NotImplementedException();
		}
	}
}