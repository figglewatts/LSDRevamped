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

		// Use this for initialization
		void Start()
		{
			_source = GetComponent<AudioSource>();
			_linkSound = Resources.Load<AudioClip>("Sound/Dream/linkSound");
		}

		// Update is called once per frame
		void Update() { }

		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!hit.gameObject.tag.Equals("Linkable")) return;
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