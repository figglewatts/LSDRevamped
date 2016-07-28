using System;
using System.Collections;
using Entities.Dream;
using Entities.WorldObject;
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

			DreamDirector.OnLevelFinishChange += actionScript.PostLoad;

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}

		private void PostLoad()
		{
			ReferencedSequence = ActionSequence.FindSequence(Name);
			AddSelf();
		}

		public override IEnumerator DoAction()
		{
			ToriiObjectAnimator animator = ReferencedSequence.ReferencedGameObject.GetComponentInChildren<ToriiObjectAnimator>();
			animator.ChangeAnimation(AnimationName);
			animator.Play();
			ReferencedSequence.DoNextAction();
			yield return null;
		}
	}
}
