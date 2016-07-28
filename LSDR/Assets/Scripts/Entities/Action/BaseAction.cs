using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Entities.Action
{
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
