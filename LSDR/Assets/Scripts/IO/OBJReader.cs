using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace IO
{
	public static class OBJReader
	{
		public static GameObject ReadOBJFile(string pathToObj)
		{
			string objString = File.ReadAllText(pathToObj);
			GameObject gameObject = ReadOBJString(objString);
			return gameObject;
		}

		public static GameObject ReadOBJString(string obj)
		{
			GameObject baseGameObject = new GameObject("importedObj");

			// a handle to the current object, used when processing multiple objects
			int objectHandle = 0;

			bool firstObject = true;

			// get ready to see the word "List" like 20 times
			List<GameObject> objObjects = new List<GameObject>();
			List<Mesh> meshes = new List<Mesh>();
			List<Vector3> vertices = new List<Vector3>();
			List<Vector2> UVs = new List<Vector2>();
			List<Vector3> normals = new List<Vector3>();
			List<List<int>> triangles = new List<List<int>>();
			List<List<int?>> uvIndices = new List<List<int?>>();
			List<List<int?>> normalIndices = new List<List<int?>>();
			// all that was so we can support multiple objects within an OBJ file

			objObjects.Add(new GameObject());
			objObjects[objectHandle].transform.SetParent(baseGameObject.transform);
			objObjects[objectHandle].AddComponent<MeshRenderer>();
			objObjects[objectHandle].AddComponent<MeshFilter>();
			meshes.Add(objObjects[objectHandle].GetComponent<MeshFilter>().mesh);
			triangles.Add(new List<int>());
			uvIndices.Add(new List<int?>());
			normalIndices.Add(new List<int?>());

			StringReader sr = new StringReader(obj);
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				string[] words = line.Split(' ');

				if (words[0].Equals("o")) // object
				{
					// initialize the object
					if (!firstObject)
					{
						objectHandle++;
						objObjects.Add(new GameObject(words[1]));
						objObjects[objectHandle].transform.SetParent(baseGameObject.transform);
						objObjects[objectHandle].AddComponent<MeshRenderer>();
						objObjects[objectHandle].AddComponent<MeshFilter>();
						meshes.Add(objObjects[objectHandle].GetComponent<MeshFilter>().mesh);
						triangles.Add(new List<int>());
						uvIndices.Add(new List<int?>());
						normalIndices.Add(new List<int?>());
					}
					else
					{
						objObjects[objectHandle].name = words[1];
						firstObject = false;
					}
					
				}
				if (words[0].Equals("v")) // vertex
				{
					Vector3 vertex = new Vector3(StringToFloat(words[1]), StringToFloat(words[2]), StringToFloat(words[3]));
					vertices.Add(vertex);
				}
				else if (words[0].Equals("vn")) // normal
				{
					Vector3 normal = new Vector3(StringToFloat(words[1]), StringToFloat(words[2]), StringToFloat(words[3]));
					normals.Add(normal);
				}
				else if (words[0].Equals("vt")) // UV
				{
					Vector2 uv = new Vector2(StringToFloat(words[1]), StringToFloat(words[2]));
					UVs.Add(uv);
				}
				else if (words[0].Equals("f"))
				{
					int numVertices = words.Length - 1;
					bool isQuad = (numVertices % 4) == 0;

					int[] verts = new int[numVertices];
					int[] uvs = new int[numVertices];
					int[] norms = new int[numVertices];

					// for each set of indices
					for (int i = 1; i < words.Length; i++) // starts at 1 to account for leading 'f'
					{
						string[] indices = words[i].Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);

						if (words[i].Contains("//") && indices.Length == 2)
						{
							// vertex & vertex normal, no UV data
							// v//vn
							verts[i - 1] = int.Parse(indices[0]);
							norms[i - 1] = int.Parse(indices[1]);
						}
						else if (indices.Length == 2)
						{
							// vertex & UV data, no normals
							// v/vt
							verts[i - 1] = int.Parse(indices[0]);
							uvs[i - 1] = int.Parse(indices[1]);
						}
						else if (indices.Length == 3)
						{
							// all 3 present
							// v/vt/vn
							verts[i - 1] = int.Parse(indices[0]);
							uvs[i - 1] = int.Parse(indices[1]);
							norms[i - 1] = int.Parse(indices[2]);
						}
						else if (indices.Length == 1)
						{
							// it's probably just vertex data
							// v
							verts[i - 1] = (int.Parse(indices[0]));
						}
					}

					if (isQuad)
					{
						// convert everything into tris
						for (int i = 1; i <= numVertices - 2; i++) // watch the less-than-or-equal-to here
						{
							triangles[objectHandle].Add(verts[0] - 1);
							triangles[objectHandle].Add(verts[i] - 1);
							triangles[objectHandle].Add(verts[i + 1] - 1);
							uvIndices[objectHandle].Add(uvs[0] - 1);
							uvIndices[objectHandle].Add(uvs[i] - 1);
							uvIndices[objectHandle].Add(uvs[i + 1] - 1);
							normalIndices[objectHandle].Add(norms[0] - 1);
							normalIndices[objectHandle].Add(norms[i] - 1);
							normalIndices[objectHandle].Add(norms[i + 1] - 1);
						}
					}
					else
					{
						for (int i = 0; i < numVertices; i++)
						{
							triangles[objectHandle].Add(verts[i] - 1);
							uvIndices[objectHandle].Add(uvs[i] - 1);
							normalIndices[objectHandle].Add(norms[i] - 1);
						}
					}
				}
			}

			for (int i = 0; i < objObjects.Count; i++)
			{
				Mesh mesh = meshes[i];
				BuildMesh(vertices.ToArray(), triangles[i].ToArray(), UVs.ToArray(), uvIndices[i].ToArray(), normals.ToArray(), normalIndices[i].ToArray(), ref mesh);
				objObjects[i].transform.localScale = new Vector3(-1, 1, 1);
			}

			return baseGameObject;
		}

		private static void BuildMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs, int?[] uvIndices, Vector3[] normals,
			int?[] normalIndices, ref Mesh m)
		{
			Vector2[] actualUVs = new Vector2[triangles.Length];
			Vector3[] actualVertices = new Vector3[triangles.Length];
			Vector3[] actualNormals = new Vector3[triangles.Length];
			int[] actualTriangles = new int[triangles.Length];
			for (int i = 0; i < triangles.Length; i++)
			{
				actualVertices[i] = vertices[triangles[i]];
				if (uvs.Length > 0)
				{
					actualUVs[i] = uvIndices[i] == null ? Vector2.zero : uvs[uvIndices[i] ?? default(int)];
				}
				if (normals.Length > 0)
				{
					actualNormals[i] = normalIndices[i] == null || normalIndices[i] == -1 ? Vector3.zero : normals[normalIndices[i] ?? default(int)];
				}
				actualTriangles[i] = i;
			}

			m.vertices = actualVertices;
			m.uv = actualUVs;
			m.normals = actualNormals;
			m.triangles = actualTriangles;
			m.RecalculateNormals();
			m.RecalculateBounds();
		}

		private static float StringToFloat(string s)
		{
			return float.Parse(s);
		}

	}
}
