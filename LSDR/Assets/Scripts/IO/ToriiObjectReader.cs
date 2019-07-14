using System.IO;
using UnityEngine;
using Types;

namespace IO
{
	public static class ToriiObjectReader
	{
		// TODO: ToriiObjectReader is probably obsolete
		public static bool Read(string location, ref TOBJ tobj)
		{
			using (BinaryReader reader = new BinaryReader(File.Open(location, FileMode.Open)))
			{
				char[] signature = reader.ReadChars(8);
				//if (signature != SIGNATURE)
				//{
				//	throw new Exception("Invalid signature in .tobj file.");
				//}

				int version = reader.ReadInt32();
				// check version later?

				tobj.ObjectFile = reader.ReadString();
				tobj.ObjectTexture = reader.ReadString();
				tobj.NumberOfObjects = reader.ReadInt32();
				tobj.MaxAnimationID = reader.ReadInt32();
				tobj.NumberOfAnimations = reader.ReadInt32();

				tobj.Animations = new TANIM[tobj.NumberOfAnimations];
				for (int i = 0; i < tobj.NumberOfAnimations; i++)
				{
					TANIM anim;
					anim.Name = reader.ReadString();
					anim.ID = reader.ReadInt32();
					anim.MaxValue = reader.ReadSingle();
					anim.MaxKeyframeID = reader.ReadInt32();
					anim.NumberOfKeyframes = reader.ReadInt32();

					anim.Keyframes = new TKEYFRAME[anim.NumberOfKeyframes];
					for (int j = 0; j < anim.NumberOfKeyframes; j++)
					{
						TKEYFRAME kf;
						kf.ID = reader.ReadInt32();
						kf.Value = reader.ReadSingle();

						kf.ObjStates = new OBJSTATE[tobj.NumberOfObjects];
						for (int k = 0; k < tobj.NumberOfObjects; k++)
						{
							OBJSTATE state;
							state.Position = ReadVector3(reader);
							state.Rotation = ReadQuaternion(reader);
							state.Scale = ReadVector3(reader);

							kf.ObjStates[k] = state;
						}

						anim.Keyframes[j] = kf;
					}

					tobj.Animations[i] = anim;
				}
			}

			return true;
		}

		private static Vector3 ReadVector3(BinaryReader reader)
		{
			Vector3 vec3;
			vec3.x = reader.ReadSingle();
			vec3.y = reader.ReadSingle();
			vec3.z = reader.ReadSingle();
			return vec3;
		}

		private static Quaternion ReadQuaternion(BinaryReader reader)
		{
			Quaternion quat;
			quat.x = reader.ReadSingle();
			quat.y = reader.ReadSingle();
			quat.z = reader.ReadSingle();
			quat.w = reader.ReadSingle();
			return quat;
		}
	}
}
