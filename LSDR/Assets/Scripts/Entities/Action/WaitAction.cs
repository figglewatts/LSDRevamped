using UnityEngine;
using System.Collections;
using Types;
using Util;

namespace Entities.Action
{
	public class WaitAction : BaseAction
	{
		public float WaitTime;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			WaitAction actionScript = instantiated.AddComponent<WaitAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);

			actionScript.WaitTime = EntityUtil.TryParseFloat("Wait time", e);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}