using System.Collections;
using UnityEngine;

namespace LSDR.Entities.Action
{
	// TODO: BaseAction is obsolete
	public abstract class BaseAction : MonoBehaviour
	{
		public string Name;
		public int SequencePosition;

		protected ActionSequence ReferencedSequence;

		public abstract IEnumerator DoAction();

		public void AddSelf()
		{
			ReferencedSequence.AddAction(this, SequencePosition);
		}
	}
}
