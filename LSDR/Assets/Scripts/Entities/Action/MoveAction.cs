using System.Collections;
using LSDR.Entities.Dream;
using LSDR.Entities.WorldObject;
using UnityEngine;
using LSDR.Types;
using LSDR.Util;

namespace LSDR.Entities.Action
{
	// TODO: MoveAction is obsolete
	public class MoveAction : BaseAction
	{
		public string TargetName;
		public bool FaceMovementDir;
		public float MoveSpeed;

		private Transform _targetTransform;
		private Vector3 _initialPosition;
		private float _distanceCovered = 0;
		private float _startTime;
		private bool _canMove = true;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			MoveAction actionScript = instantiated.AddComponent<MoveAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);
			actionScript.TargetName = e.GetPropertyValue("Target name");
			actionScript.MoveSpeed = EntityUtil.TryParseFloat("Move speed", e);
			actionScript.FaceMovementDir = e.GetSpawnflagValue(0, 1);

			DreamDirector.PostLoadEvent += actionScript.PostLoad;

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}

		private void PostLoad()
		{
			_targetTransform = Target.GetTargetTransform(TargetName);
			ReferencedSequence = ActionSequence.FindSequence(Name);
			AddSelf();
		}

		public override IEnumerator DoAction()
		{
			_startTime = Time.time;
			_initialPosition = ReferencedSequence.ReferencedGameObject.transform.position;
			float distanceToTarget = Vector3.Distance(_initialPosition, _targetTransform.position);

			if (FaceMovementDir) ReferencedSequence.ReferencedGameObject.transform.LookAt(_targetTransform);

			while (_canMove)
			{
				float fracJourney = _distanceCovered/distanceToTarget;

				if (fracJourney >= 1) _canMove = false;

				ReferencedSequence.ReferencedGameObject.transform.position = Vector3.Lerp(_initialPosition,
					_targetTransform.position, fracJourney);
				_distanceCovered = (Time.time - _startTime)*MoveSpeed;
				yield return null;
			}

			ReferencedSequence.DoNextAction();
			_canMove = true;
			_distanceCovered = 0;
		}
	}
}