using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Types;
using UnityEngine;

namespace Util
{
	public static class EntityUtil
	{
		public static float TryParseFloat(string propertyName, ENTITY e)
		{
			float result;
			if (!float.TryParse(e.GetPropertyValue("Happiness"), out result))
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
				                 e.GetPropertyValue(propertyName));
				return 0;
			}
			return result;
		}

		public static int TryParseInt(string propertyName, ENTITY e)
		{
			int result;
			if (!int.TryParse(e.GetPropertyValue("Happiness"), out result))
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
								 e.GetPropertyValue(propertyName));
				return 0;
			}
			return result;
		}

		public static Color TryParseColor(string propertyName, ENTITY e)
		{
			string color = e.GetPropertyValue(propertyName);
			string[] colorParts = color.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			if (colorParts.Length != 3)
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
								 e.GetPropertyValue(propertyName) + ", colorParts was not equal to 3");
				return Color.white;
			}

			float r;
			if (!float.TryParse(colorParts[0], out r))
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
								 e.GetPropertyValue(propertyName) + ", unable to parse red component: " + colorParts[0]);
				r = 1;
			}

			float g;
			if (!float.TryParse(colorParts[1], out g))
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
								 e.GetPropertyValue(propertyName) + ", unable to parse green component: " + colorParts[1]);
				g = 1;
			}

			float b;
			if (!float.TryParse(colorParts[2], out b))
			{
				Debug.LogWarning("Unable to parse property \"" + propertyName + "in " + e.Classname + ": " +
								 e.GetPropertyValue(propertyName) + ", unable to parse blue component: " + colorParts[2]);
				b = 1;
			}

			return new Color(r, g, b);
		}

		public static void SetInstantiatedObjectTransform(ENTITY e, ref GameObject g)
		{
			g.transform.position = Vec3ToUnityVector3(e.Position);
			g.transform.rotation = AxisAngleToQuaternion(e.Rotation);
			g.transform.localScale = Vec3ToUnityVector3(e.Scale);
		}

		public static Vector3 Vec3ToUnityVector3(VEC3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		public static Quaternion AxisAngleToQuaternion(AXISANGLE a)
		{
			return Quaternion.AngleAxis(a.Angle, Vec3ToUnityVector3(a.Axis));
		}
	}
}
