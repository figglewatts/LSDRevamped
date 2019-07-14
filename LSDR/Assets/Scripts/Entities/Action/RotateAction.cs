using System.Collections;
using LSDR.Entities.Dream;
using LSDR.Types;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Entities.Action
{
	// TODO: RotateAction is obsolete
	public class RotateAction : BaseAction
	{
		public float Rotation;
		public float RotationSpeed;
		public bool RotateInstantly;

		private Quaternion _originalRotation;
		private Quaternion _targetRotation;
		private bool _canRotate;
		private float _rotationProgress;

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			RotateAction actionScript = instantiated.AddComponent<RotateAction>();
			actionScript.Name = e.GetPropertyValue("Sequence name");
			actionScript.SequencePosition = EntityUtil.TryParseInt("Sequence position", e);

			actionScript.Rotation = EntityUtil.TryParseFloat("Rotation", e);

			actionScript.RotationSpeed = EntityUtil.TryParseFloat("Rotation speed", e);

			actionScript.RotateInstantly = e.GetSpawnflagValue(0, 1);

			DreamDirector.PostLoadEvent += actionScript.PostLoad;

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
			_rotationProgress = 0;
			_originalRotation = ReferencedSequence.ReferencedGameObject.transform.rotation;
			_targetRotation = Quaternion.Euler(_originalRotation.eulerAngles.x, Rotation, _originalRotation.eulerAngles.z);

			if (RotateInstantly)
			{
				ReferencedSequence.ReferencedGameObject.transform.rotation = _targetRotation;
				ReferencedSequence.DoNextAction();
				yield break;
			}

			_canRotate = true;

			while (_canRotate)
			{
				_rotationProgress += Time.deltaTime*RotationSpeed;

				if (_rotationProgress >= 1) _canRotate = false;

				ReferencedSequence.ReferencedGameObject.transform.rotation = Quaternion.Lerp(_originalRotation, _targetRotation,
					_rotationProgress);

				yield return null;
			}

			ReferencedSequence.DoNextAction();
		}
	}
}