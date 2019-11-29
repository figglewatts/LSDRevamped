using UnityEngine;
using LSDR.Entities.Dream;
using LSDR.Entities.WorldObject;
using LSDR.Game;
using LSDR.InputManagement;
using LSDR.UI;
using UnityEngine.Audio;
using LSDR.Util;

namespace LSDR.Entities.Player
{
	// TODO: refactor PlayerLinker in DreamDirector refactor
	public class PlayerLinker : MonoBehaviour
	{
		public SettingsSystem Settings;

		public float LinkDelay = 0.7F;

		private bool _canLink = true;

		private float _linkTimer = 0F;

		private AudioSource _source;
		private AudioClip _linkSound;
		private AudioMixer _masterMixer;

		private bool _touchingFloor;

		private const int ChangeTextureSetChance = 100;

		// Use this for initialization
		void Start()
		{
			_masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
			_source = GetComponent<AudioSource>();
			_source.spatialBlend = 0; // 2D audio
			_source.outputAudioMixerGroup = _masterMixer.FindMatchingGroups("SFX")[0];
			_linkSound = Resources.Load<AudioClip>("Sound/Dream/linkSound");
		}

		void Update() { }

		public void OnControllerColliderHit(ControllerColliderHit hit)
		{
			// TODO
			//if (DreamDirector.Payload.DreamEnded) return;
		
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
				LinkableObject o = hit.gameObject.transform.parent.transform.parent.GetComponent<LinkableObject>();

				Color linkCol = o.ForceFadeColor ? o.FadeColor : RandUtil.RandColor();
				string linkLevel = "";//o.LinkToSpecificLevel ? IOUtil.PathCombine("levels", o.LinkedLevel) : RandUtil.RandomLevelFromDir(DreamJournalManager.CurrentJournal);

				_linkTimer = 0;
				Link(linkLevel, linkCol);
			}
		}

		public void Link(string dreamFilePath, Color color, bool playSound = true, string spawnName = "")
		{
			Settings.CanControlPlayer = false;
			_canLink = false;

			int shouldChangeTextureSetChance = RandUtil.Int(100);
			bool shouldChangeTextureSet = shouldChangeTextureSetChance < ChangeTextureSetChance;

			if (playSound) _source.PlayOneShot(_linkSound);
			Fader.FadeIn(color, 1F, () =>
			{
				DreamDirector.SwitchDreamLevel(dreamFilePath, spawnName);
				if (shouldChangeTextureSet) DreamDirector.RefreshTextureSet(false);
				Settings.CanControlPlayer = true;
				Fader.FadeOut(color, 1F, () =>
				{
					_canLink = true;
				});
			});
		}

		public void Link(string dreamFilePath) { Link(dreamFilePath, RandUtil.RandColor()); }

		public void Link(bool playSound = true)
		{
			//Link(RandUtil.RandomLevelFromDir(DreamJournalManager.CurrentJournal), RandUtil.RandColor(), playSound);
		}
	}
}