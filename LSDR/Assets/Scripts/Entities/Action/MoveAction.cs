using UnityEngine;
using Types;
using Util;

namespace Entities.Action
{
	public class MoveAction : BaseAction
	{
		public string TargetName;
		public bool FaceMovementDir;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MoveAction actionScript = instantiated.AddComponent<MoveAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);
			actionScript.TargetName = e.GetPropertyValue("Target name");
			actionScript.FaceMovementDir = e.GetSpawnflagValue(0, 1);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}