using UnityEngine;
using System.Collections.Generic;
using Types;

namespace Entities.WorldObject
{
	public class ToriiObjectAnimator : MonoBehaviour
	{
		public TOBJ ToriiObject;
		public List<GameObject> Objects = new List<GameObject>();

		private int CurrentAnimationHandle;
		public bool IsAnimating;
		public bool LoopAnimations = true;

		public TANIM CurrentAnimation { get { return ToriiObject.Animations[CurrentAnimationHandle]; } }

		public int CurrentKeyframeIndex
		{
			get
			{
				int index = 0;
				for (int i = 0; i < CurrentAnimation.NumberOfKeyframes; i++)
				{
					if (CurrentAnimation.Keyframes[i].Value <= _animationTimer)
					{
						index = i;
					}
				}
				return index;
			}
		}

		public TKEYFRAME CurrentKeyframe { get { return CurrentAnimation.Keyframes[CurrentKeyframeIndex]; } }

		public TKEYFRAME NextKeyframe { get { return CurrentAnimation.Keyframes[CurrentKeyframeIndex + 1]; } }

		private float _animationTimer;
		private OBJSTATE[] _originalObjStates;

		// Use this for initialization
		void Start()
		{
			_animationTimer = 0;
			_originalObjStates = new OBJSTATE[Objects.Count];
			for (int i = 0; i < _originalObjStates.Length; i++)
			{
				OBJSTATE s;
				s.Position = Objects[i].transform.position;
				s.Rotation = Objects[i].transform.rotation;
				s.Scale = Objects[i].transform.localScale;
				_originalObjStates[i] = s;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (IsAnimating)
			{
				_animationTimer += Time.deltaTime*10f;
				if (_animationTimer >= CurrentAnimation.MaxValue)
				{
					if (LoopAnimations)
					{
						_animationTimer = 0;
					}
					else
					{
						IsAnimating = false;
					}
				}
				else
				{
					LerpObjects(CurrentKeyframe, NextKeyframe);
				}
			}
		}

		public void ResetToDefaultStates()
		{
			for (int i = 0; i < Objects.Count; i++)
			{
				GameObject g = Objects[i];
				SetObjectToState(ref g, _originalObjStates[i]);
			}
		}

		private void SetObjectToState(ref GameObject g, OBJSTATE s)
		{
			g.transform.position = s.Position;
			g.transform.rotation = s.Rotation;
			g.transform.localScale = s.Scale;
		}

		private void LerpObjects(TKEYFRAME current, TKEYFRAME next)
		{
			float lowBound = current.Value;
			float highBound = next.Value;
			float valueDifference = highBound - lowBound;
			float timelinePos = _animationTimer - lowBound;
			float t = timelinePos/valueDifference;

			// interpolate between position, scale, and rotation
			for (int i = 0; i < Objects.Count; i++)
			{
				Objects[i].transform.position = Vector3.Lerp(current.ObjStates[i].Position, next.ObjStates[i].Position, t);
				Objects[i].transform.rotation = Quaternion.Lerp(current.ObjStates[i].Rotation, next.ObjStates[i].Rotation, t);
				Objects[i].transform.localScale = Vector3.Lerp(current.ObjStates[i].Scale, next.ObjStates[i].Scale, t);
			}
		}

		public void ChangeAnimation(int animationIndex)
		{
			_animationTimer = 0;
			CurrentAnimationHandle = animationIndex;
		}

		public void ChangeAnimation(string animationName)
		{
			int i = 0;
			foreach (TANIM a in ToriiObject.Animations)
			{
				if (a.Name.Equals(animationName))
				{
					ChangeAnimation(i);
					return;
				}
				i++;
			}

			Debug.LogWarning("Could not find animation with name " + animationName);
		}

		public void Play() { IsAnimating = true; }

		public void Pause() { IsAnimating = false; }

		public void Stop()
		{
			IsAnimating = false;
			_animationTimer = 0;
		}
	}
}