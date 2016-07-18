using System;
using IO;

namespace Types
{
	public struct TMAP
	{
		public TMAPHEADER Header;
		public TMAPCONTENT Content;
	}

	public struct TMAPHEADER
	{
		public char[] Signature;
		public int Version;
		public string Name;
		public string Author;
		public int PreviewSize;
		public byte[] Preview;
	}

	public struct TMAPCONTENT
	{
		public int NumberOfEntities;
		public ENTITY[] Entities;
	}

	public struct ENTITY
	{
		public string Classname;
		public VEC3 Position;
		public AXISANGLE Rotation;
		public VEC3 Scale;
		public int NumberOfProperties;
		public PROPERTY[] Properties;
		public uint Spawnflags;

		public string GetPropertyValue(string name)
		{
			foreach (PROPERTY p in Properties)
			{
				if (p.Name.Equals(name))
				{
					return p.Value;
				}
			}
			return null;
		}
	}

	public struct PROPERTY
	{
		public string Name;
		public string Value;
	}

	public struct AXISANGLE
	{
		public VEC3 Axis;
		public float Angle;
	}

	public struct VEC3
	{
		public float X;
		public float Y;
		public float Z;
	}
}