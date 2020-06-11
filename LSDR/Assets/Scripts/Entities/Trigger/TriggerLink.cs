using System;
using ProtoBuf;
using Torii.UnityEditor;
using Torii.Util;
using UnityEngine;

namespace LSDR.Entities.Trigger
{
	public class TriggerLink : BaseEntity
	{
		[BrowseFileSystem(BrowseType.File, new [] { "Dream JSON", "json"}, "dream")]
		public string LinkedLevel;
		public Color ForcedLinkColor;
		public string SpawnPointEntityID;

		public bool ForceFadeColor;
		public bool PlayLinkSound;

		private BoxCollider _collider;

		public void Start()
		{
			// create the collider
			_collider = gameObject.AddComponent<BoxCollider>();
			_collider.size = transform.localScale;
			_collider.isTrigger = true;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (!other.gameObject.CompareTag("Player")) return;

			if (!ForceFadeColor)
			{
				ForcedLinkColor = RandUtil.RandColor();
			}
			DreamSystem.Transition(ForcedLinkColor, LinkedLevel, PlayLinkSound, SpawnPointEntityID);
		}

		public void OnDrawGizmos()
		{
			var position = transform.position;
			Gizmos.DrawIcon(position, "TriggerLink.png");
			Gizmos.color = Color.green;
			var localScale = transform.localScale;
			Gizmos.DrawWireCube(position, localScale);
			Gizmos.color = new Color(0, 1, 0, 0.5f);
			Gizmos.DrawCube(position, localScale);
		}

		public override EntityMemento Save() { return new TriggerLinkMemento(this); }
		
		public override void Restore(EntityMemento memento, LevelEntities entities)
		{
			base.Restore(memento, entities);

			var triggerLinkMemento = (TriggerLinkMemento)memento;
			LinkedLevel = triggerLinkMemento.LinkedLevel;
			ForcedLinkColor = triggerLinkMemento.ForcedLinkColor;
			SpawnPointEntityID = triggerLinkMemento.SpawnPointEntityID;
			ForceFadeColor = triggerLinkMemento.ForceFadeColor;
			PlayLinkSound = triggerLinkMemento.PlayLinkSound;
			entities.Register(this);
		}
	}
	
	[ProtoContract(ImplicitFields = ImplicitFields.AllPublic, SkipConstructor = true)]
	public class TriggerLinkMemento : EntityMemento
	{
		public string LinkedLevel;
		public Color ForcedLinkColor;
		public string SpawnPointEntityID;

		public bool ForceFadeColor;
		public bool PlayLinkSound;

		protected override Type EntityType => typeof(TriggerLink);

		public TriggerLinkMemento(TriggerLink state) : base(state)
		{
			LinkedLevel = state.LinkedLevel;
			ForcedLinkColor = state.ForcedLinkColor;
			SpawnPointEntityID = state.SpawnPointEntityID;
			ForceFadeColor = state.ForceFadeColor;
			PlayLinkSound = state.PlayLinkSound;
		}
	}
}
