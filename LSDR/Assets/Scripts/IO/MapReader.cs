using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Game;
using MapParse;
using MapParse.Types;
using UnityEngine;
using Util;

namespace IO
{
	// TODO: MapReader is now obsolete
	public static class MapReader
	{
		public static float MapScaleFactor = 0.025F;
	
		private static readonly Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
		private static readonly List<string> _wadsUsed = new List<string>();

		private static string _pathToMapTextures = "";
		private static Shader _diffuseShader = null;
		private static Shader _transparentShader = null;
		private static bool _generateCollisionMesh = true;

		public static GameObject LoadMap(string path, string pathToTextures, Shader diffuse = null, Shader transparent = null,
			bool generateCollisionMesh = true)
		{
			if (Mathf.Approximately(MapScaleFactor, 0))
			{
				Debug.LogError("MapScaleFactor cannot be 0!");
				return null;
			}
			
			_materialCache.Clear();
			_wadsUsed.Clear();
			
			_pathToMapTextures = pathToTextures;
			_diffuseShader = diffuse;
			_transparentShader = transparent;
			_generateCollisionMesh = generateCollisionMesh;
		
			MapFile map = MapParser.ParseMap(path);
			return BuildMapMesh(map);
		}

		private static GameObject BuildMapMesh(MapFile map)
		{
			GameObject mapObject = new GameObject("Imported Map");

			for (int i = 0; i < map.Entities.Count; i++)
			{
				GameObject entityObject = BuildEntity(map.Entities[i]);
				entityObject.transform.SetParent(mapObject.transform);
			}

			mapObject.transform.localScale = new Vector3(MapScaleFactor, MapScaleFactor, MapScaleFactor);

			return mapObject;
		}

		private static GameObject BuildEntity(Entity e)
		{
			GameObject entityObject = new GameObject(e.Properties["classname"]);

			// populate the list of wads used if this entity has the wad property
			if (e.GetPropertyValue("classname").Equals("worldspawn"))
			{
				string wadString = e.GetPropertyValue("wad");
				foreach (string wad in wadString.Split(';'))
				{
					_wadsUsed.Add(Path.GetFileNameWithoutExtension(wad));
				}
			}

			// generate gameobjects for entity's brushes
			for (int i = 0; i < e.NumberOfBrushes; i++)
			{
				GameObject brushObject = BuildBrush(e.Brushes[i]);
				brushObject.transform.SetParent(entityObject.transform);
			}

			if (_generateCollisionMesh && e.NumberOfBrushes > 0)
			{
				// combine brushes into single mesh for collision
				MeshFilter[] meshFilters = entityObject.GetComponentsInChildren<MeshFilter>();
				CombineInstance[] combine = new CombineInstance[meshFilters.Length];

				// set up the CombineInstances, transform local coords to world coords
				for (int filterIndex = 0; filterIndex < meshFilters.Length; filterIndex++)
				{
					combine[filterIndex].mesh = meshFilters[filterIndex].sharedMesh;
					combine[filterIndex].transform = meshFilters[filterIndex].transform.localToWorldMatrix;
				}

				// create the collision mesh
				Mesh collisionMesh = new Mesh();
				collisionMesh.CombineMeshes(combine);

				// add and setup the mesh collider
				entityObject.AddComponent<MeshCollider>().sharedMesh = collisionMesh;
			}

			return entityObject;
		}

		private static GameObject BuildBrush(Brush b)
		{
			GameObject brushObject = new GameObject("Brush");

			for (int i = 0; i < b.NumberOfFaces; i++)
			{
				GameObject faceObject = BuildFace(b.Faces[i]);
				faceObject.transform.SetParent(brushObject.transform);
			}

			return brushObject;
		}

		private static GameObject BuildFace(Face f)
		{
			// check to see if we need to apply a transparent shader to the face
			bool isTransparent = f.Texture.Contains('{');

			Texture2D[] faceTextures = TextureLoad(f.Texture);

			// check to see if we loaded multiple textures
			bool inTextureSet = faceTextures.Length >= 4;

			// cache the path for debug logging
			string pathToTex = IOUtil.PathCombine(_pathToMapTextures, f.Texture);

			// check to see if we correctly loaded the other texture sets, if not default to A
			if (inTextureSet)
			{
				if (faceTextures[(int)TextureSetIndex.B] == null)
				{
					Debug.LogWarning("WARNING: Could not load B texture for: " + pathToTex);
					Debug.LogWarning("Defaulting to texture A");
					faceTextures[(int)TextureSetIndex.B] = faceTextures[(int)TextureSetIndex.A];
				}
				if (faceTextures[(int)TextureSetIndex.C] == null)
				{
					Debug.LogWarning("WARNING: Could not load C texture for: " + pathToTex);
					Debug.LogWarning("Defaulting to texture A");
					faceTextures[(int)TextureSetIndex.C] = faceTextures[(int)TextureSetIndex.A];
				}
				if (faceTextures[(int)TextureSetIndex.D] == null)
				{
					Debug.LogWarning("WARNING: Could not load D texture for: " + pathToTex);
					Debug.LogWarning("Defaulting to texture A");
					faceTextures[(int)TextureSetIndex.D] = faceTextures[(int)TextureSetIndex.A];
				}
			}

			// create the material to apply to the face
			Material material;
			if (!TextureExistsInMaterialCache(f.Texture, out material)) // note if the texture exists in the cache, material will be assigned
			{
				// if the material has not already been created, create one
				// with either the transparent or diffuse shader, depending upon the check done earlier
				material = new Material(isTransparent ? _transparentShader : _diffuseShader);

				material.SetTexture("_MainTexA", faceTextures[(int)TextureSetIndex.A]);
				material.SetTextureScale("_MainTexA", new Vector2(1, -1));
				material.SetTexture("_MainTexB", inTextureSet ? faceTextures[(int)TextureSetIndex.B] : faceTextures[(int)TextureSetIndex.A]);
				material.SetTextureScale("_MainTexB", new Vector2(1, -1));
				material.SetTexture("_MainTexC", inTextureSet ? faceTextures[(int)TextureSetIndex.C] : faceTextures[(int)TextureSetIndex.A]);
				material.SetTextureScale("_MainTexC", new Vector2(1, -1));
				material.SetTexture("_MainTexD", inTextureSet ? faceTextures[(int)TextureSetIndex.D] : faceTextures[(int)TextureSetIndex.A]);
				material.SetTextureScale("_MainTexD", new Vector2(1, -1));
				_materialCache.Add(f.Texture, material);
			}

			GameObject faceObject = new GameObject("Face");
			CreatePolys(ref faceObject, f, material);
			
			return faceObject;
		}

		private static void CreatePolys(ref GameObject faceObject, Face f, Material m)
		{
			List<Vector3> verts = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> tris = new List<int>();

			faceObject.AddComponent<MeshFilter>();
			faceObject.AddComponent<MeshRenderer>();

			Mesh faceMesh = faceObject.GetComponent<MeshFilter>().mesh;
			faceMesh.Clear();

			// go through and generate the mesh vertex by vertex
			for (int polyIndex = 0; polyIndex < f.Polys.Length; polyIndex++)
			{
				Poly p = f.Polys[polyIndex];

				for (int vertIndex = 0; vertIndex < p.NumberOfVertices; vertIndex++)
				{
					Vertex v = p.Verts[vertIndex];
					verts.Add(Vec3ToUnity(v.P));
				}

				// generate triangles
				for (int triIndex = 0; triIndex < p.NumberOfVertices - 2; triIndex++)
				{
					tris.Add(0);
					tris.Add(triIndex + 1);
					tris.Add(triIndex + 2);
				}

				p.CalculateTextureCoordinates(m.GetTexture("_MainTexA").width, m.GetTexture("_MainTexA").height, f.TexAxis, f.TexScale);

				// generate UVs
				for (int uvIndex = 0; uvIndex < p.NumberOfVertices; uvIndex++)
				{
					Vector2 uv = new Vector2(p.Verts[uvIndex].Tex[0], p.Verts[uvIndex].Tex[1]);
					uvs.Add(uv);
				}
			}

			// set up the polygons for the mesh and calculate normals
			faceMesh.vertices = verts.ToArray();
			faceMesh.triangles = tris.ToArray();
			faceMesh.uv = uvs.ToArray();
			faceMesh.RecalculateNormals();

			faceObject.GetComponent<Renderer>().sharedMaterial = m;
		}

		private static Vector3 Vec3ToUnity(Vec3 v)
		{
			return new Vector3(v.X, v.Y, v.Z);
		}

		private static bool TextureExistsInMaterialCache(string texName, out Material m)
		{
			bool exists = _materialCache.ContainsKey(texName);
			m = exists ? _materialCache[texName] : null;
			return exists;
		}

		private static Texture2D[] TextureLoad(string textureName)
		{
			// check to see if we need to load different texture sets
			bool isInTextureSet = textureName.Contains("[");

			string tempTextureName = textureName;
			if (isInTextureSet)
			{
				// remove the A, B, C, or D from the texture
				tempTextureName = textureName.Substring(1);
			}
			tempTextureName += ".png";

			string mapTexturesPath = IOUtil.PathCombine(Application.streamingAssetsPath, _pathToMapTextures);

			// perform a lookup for which wad the texture is in
			string wadName = GetTextureWad(textureName, mapTexturesPath);

			Texture2D[] textures = new Texture2D[isInTextureSet ? 4 : 1];
			for (int i = 0; i < textures.Length; i++)
			{
				TextureSetIndex tsi = (TextureSetIndex)i;

				string nameOfTextureToLoad = (isInTextureSet ? tsi.ToString() : string.Empty) + tempTextureName;

				textures[i] = ResourceManager.Load<Texture2D>(IOUtil.PathCombine(mapTexturesPath, wadName, nameOfTextureToLoad), ResourceLifespan.LEVEL);
			}

			return textures;
		}

		private static string GetTextureWad(string textureName, string mapTexturesPath)
		{
			string wadName = string.Empty;
			foreach (string wad in _wadsUsed)
			{
				string texPath = IOUtil.PathCombine(mapTexturesPath, wad, textureName + ".png");
				if (File.Exists(texPath))
				{
					wadName = wad;
				}
			}
			if (wadName.Equals(string.Empty))
			{
				Debug.LogError("ERROR: Could not find texture \"" + textureName + "\" in any WAD");
				return null;
			}
			return wadName;
		}

		private enum TextureSetIndex
		{
			A,
			B,
			C,
			D
		}
	}
}
