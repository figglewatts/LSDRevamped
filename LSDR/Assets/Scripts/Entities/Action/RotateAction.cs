using Types;
using UnityEngine;
using Util;

namespace Entities.Action
{
	public class RotateAction : BaseAction
	{
		public float Rotation;
		public float RotationSpeed;
		public bool RotateInstantly;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			RotateAction actionScript = instantiated.AddComponent<RotateAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);

			actionScript.Rotation = EntityUtil.TryParseFloat("Rotation", e);

			actionScript.RotationSpeed = EntityUtil.TryParseFloat("Rotation speed", e);

			actionScript.RotateInstantly = e.GetSpawnflagValue(0, 1);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}