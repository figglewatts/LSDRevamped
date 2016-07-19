using Types;
using UnityEngine;
using Util;

namespace Entities.Action
{
	public class AnimateAction : BaseAction
	{
		public string AnimationName;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			AnimateAction actionScript = instantiated.AddComponent<AnimateAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);
			actionScript.AnimationName = e.GetPropertyValue("Animation");

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
