using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Action;
using Entities.Dream;
using Types;
using UnityEngine;
using Util;

namespace Entities.Trigger
{
	public class TriggerSequence : MonoBehaviour
	{
		public string SequenceName;

		private ActionSequence _referencedSequence;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerSequence script = instantiated.AddComponent<TriggerSequence>();

			script.SequenceName = e.GetPropertyValue("Sequence name");

			DreamDirector.PostLoadEvent += script.PostLoad;

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}

		private void PostLoad()
		{
			_referencedSequence = ActionSequence.FindSequence(SequenceName);
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				if (_referencedSequence) _referencedSequence.BeginSequence();
			}
		}
	}
}
