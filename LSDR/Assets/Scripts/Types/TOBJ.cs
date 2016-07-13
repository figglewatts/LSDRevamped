using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Types
{
	public struct TOBJ
	{
		public string ObjectFile;
		public string ObjectTexture;
		public int NumberOfObjects;
		public int MaxAnimationID;
		public int NumberOfAnimations;
		public TANIM[] Animations;
	}

	public struct TANIM
	{
		public string Name;
		public int ID;
		public float MaxValue;
		public int MaxKeyframeID;
		public int NumberOfKeyframes;
		public TKEYFRAME[] Keyframes;
	}

	public struct TKEYFRAME
	{
		public int ID;
		public float Value;
		public OBJSTATE[] ObjStates;
	}

	public struct OBJSTATE
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
	}
}