using System.IO;
using LSDR.Types;

namespace LSDR.IO
{
	public static class ToriiMapReader
	{
		// TODO: ToriiMapReader is probably obsolete
		public static TMAP ReadFromFile(string filePath)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException("Could not find TMAP file!");
		
			TMAP tmap;
			using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
			{
				TMAPHEADER header;
				header.Signature = reader.ReadChars(8);
				header.Version = reader.ReadInt32();
				header.Name = reader.ReadString();
				header.Author = reader.ReadString();
				header.PreviewSize = reader.ReadInt32();
				header.Preview = reader.ReadBytes(header.PreviewSize);
				tmap.Header = header;

				TMAPCONTENT content;
				content.NumberOfEntities = reader.ReadInt32();
				content.Entities = new ENTITY[content.NumberOfEntities];
				for (int i = 0; i < content.NumberOfEntities; i++)
				{
					content.Entities[i] = ReadEntity(reader);
				}
				tmap.Content = content;
			}
			return tmap;
		}

		private static ENTITY ReadEntity(BinaryReader r)
		{
			ENTITY e;
			e.Classname = r.ReadString();
			e.Position = ReadVec3(r);
			e.Rotation = ReadAxisAngle(r);
			e.Scale = ReadVec3(r);
			e.NumberOfProperties = r.ReadInt32();
			e.Properties = new PROPERTY[e.NumberOfProperties];
			for (int i = 0; i < e.NumberOfProperties; i++)
			{
				e.Properties[i] = ReadProperty(r);
			}
			e.Spawnflags = r.ReadUInt32();
			return e;
		}

		private static PROPERTY ReadProperty(BinaryReader r)
		{
			PROPERTY p;
			p.Name = r.ReadString();
			p.Value = r.ReadString();
			return p;
		}

		private static AXISANGLE ReadAxisAngle(BinaryReader r)
		{
			AXISANGLE aa;
			aa.Axis = ReadVec3(r);
			aa.Angle = r.ReadSingle();
			return aa;
		}

		private static VEC3 ReadVec3(BinaryReader r)
		{
			VEC3 v;
			v.X = r.ReadSingle();
			v.Y = r.ReadSingle();
			v.Z = r.ReadSingle();
			return v;
		}

	}
}
