using Types;
using UnityEngine;
using Util;

namespace Entities.Trigger
{
	public class TriggerTeleport : MonoBehaviour
	{
		public string TargetName;
		public Color FadeColor;
		public float FadeInTime;
		public float FadeHoldTime;
		public float FadeOutTime;

		public bool FadeOnTeleport;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerTeleport script = instantiated.AddComponent<TriggerTeleport>();

			script.TargetName = e.GetPropertyValue("Target name");

			script.FadeOnTeleport = e.GetSpawnflagValue(0, 1);

			if (script.FadeOnTeleport)
			{
				script.FadeColor = EntityUtil.TryParseColor("Fade color", e);
				script.FadeInTime = EntityUtil.TryParseFloat("Fade in time", e);
				script.FadeHoldTime = EntityUtil.TryParseFloat("Fade hold time", e);
				script.FadeOutTime = EntityUtil.TryParseFloat("Fade out time", e);
			}

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}
	}
}
