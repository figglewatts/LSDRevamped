using Types;
using UnityEngine;
using Util;

namespace Entities.Action
{
	public class ActionSequence : MonoBehaviour
	{
		public string SequenceName;
		public string NextSequence;
		public int TimesToLoop;
		public bool BeginOnLevelLoad;
		public bool LoopSequence;
		public bool HasNextSequence;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			ActionSequence actionSequence = instantiated.AddComponent<ActionSequence>();

			actionSequence.SequenceName = e.GetPropertyValue("Sequence name");
			actionSequence.NextSequence = e.GetPropertyValue("Next sequence");

			actionSequence.TimesToLoop = EntityUtil.TryParseInt("Times to loop", e);

			actionSequence.BeginOnLevelLoad = e.GetSpawnflagValue(0, 3);
			actionSequence.LoopSequence = e.GetSpawnflagValue(1, 3);
			actionSequence.HasNextSequence = e.GetSpawnflagValue(2, 3);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}
	}
}
