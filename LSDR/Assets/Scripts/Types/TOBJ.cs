using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Types
{
	// TODO: TOBJ might be obsolete now
	
	[Serializable]
	public struct TOBJ
	{
		public string ObjectFile;
		public string ObjectTexture;
		public int NumberOfObjects;
		public int MaxAnimationID;
		public int NumberOfAnimations;
		public TANIM[] Animations;
	}

	[Serializable]
	public struct TANIM
	{
		public string Name;
		public int ID;
		public float MaxValue;
		public int MaxKeyframeID;
		public int NumberOfKeyframes;
		public TKEYFRAME[] Keyframes;
	}

	[Serializable]
	public struct TKEYFRAME
	{
		public int ID;
		public float Value;
		public OBJSTATE[] ObjStates;
	}

	[Serializable]
	public struct OBJSTATE
	{
		public Vector3 Position;
		public Quaternion Rotation;
		public Vector3 Scale;
	}
}