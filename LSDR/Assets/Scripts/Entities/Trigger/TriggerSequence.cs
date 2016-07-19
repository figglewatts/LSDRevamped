using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;
using Util;

namespace Entities.Trigger
{
	public class TriggerSequence : MonoBehaviour
	{
		public string SequenceName;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerSequence script = instantiated.AddComponent<TriggerSequence>();

			script.SequenceName = e.GetPropertyValue("Sequence name");

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}
	}
}
