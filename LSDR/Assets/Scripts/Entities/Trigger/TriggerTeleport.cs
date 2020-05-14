using LSDR.Entities.WorldObject;
using LSDR.Types;
using LSDR.UI;
using UnityEngine;
using LSDR.Util;
using Torii.UI;

namespace LSDR.Entities.Trigger
{
	// TODO: TriggerTeleport is obsolete
	public class TriggerTeleport : MonoBehaviour
	{
		public string TargetName;
		public Color FadeColor;
		public float FadeInTime;
		public float FadeOutTime;

		public bool FadeOnTeleport;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			TriggerTeleport script = instantiated.AddComponent<TriggerTeleport>();

			script.TargetName = e.GetPropertyValue("Target name");
			if (script.TargetName.Equals(string.Empty))
			{
				Debug.LogWarning("Trigger_teleport has no target, please fix in Torii!");
			}

			script.FadeOnTeleport = e.GetSpawnflagValue(0, 1);

			if (script.FadeOnTeleport)
			{
				script.FadeColor = EntityUtil.TryParseColor("Fade color", e);
				script.FadeInTime = EntityUtil.TryParseFloat("Fade in time", e);
				script.FadeOutTime = EntityUtil.TryParseFloat("Fade out time", e);
			}

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			instantiated.AddComponent<BoxCollider>().isTrigger = true;

			return instantiated;
		}

		public void OnTriggerEnter(Collider other)
		{
			if (TargetName.Equals(string.Empty)) return;
			if (other.CompareTag("Player"))
			{
				Transform teleportTarget = Target.GetTargetTransform(TargetName);
				if (FadeOnTeleport)
				{
					ToriiFader.Instance.FadeIn(FadeColor, FadeInTime, () =>
					{
						other.transform.position = teleportTarget.position;
						other.transform.rotation = Quaternion.Euler(other.transform.rotation.eulerAngles.x,
							teleportTarget.transform.rotation.eulerAngles.y, other.transform.rotation.eulerAngles.z);
						ToriiFader.Instance.FadeOut(FadeOutTime);
					});
				}
				else
				{
					other.transform.position = teleportTarget.position;
					other.transform.rotation = Quaternion.Euler(other.transform.rotation.eulerAngles.x,
						teleportTarget.transform.rotation.eulerAngles.y, other.transform.rotation.eulerAngles.z);
				}
			}
		}
	}
}
