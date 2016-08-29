using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Net;
using Entities;
using Entities.WorldObject;
using Game;
using IO;
using SimpleJSON;
using Types;

namespace Util
{
	public static class IOUtil
	{
		/// <summary>
		/// Writes the contents of a JSON file to disk. Note that this does not use prettyprint.
		/// </summary>
		/// <param name="json">The JSONClass instance to write to disk</param>
		/// <param name="path">The path to write to</param>
		/// <param name="fileName">The name of the json file</param>
		public static void WriteJSONToDisk(JSONClass json, string path)
		{
			// we don't want to be able to create files without a name
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogError("Error writing JSON file to disk: fileName was null or empty");
				return;
			}

			try
			{
				FileInfo file = new FileInfo(path);
				file.Directory.Create();
				File.WriteAllText(path, json.ToString());
			}
			catch (IOException e)
			{
				Debug.LogError("Error writing JSON file to disk: " + path);
				Debug.LogException(e);
			}
		}

		/// <summary>
		/// Reads JSON from disk into JSONClass.
		/// </summary>
		/// <param name="path">Path to the json file.</param>
		/// <returns>Null if error reading.</returns>
		public static JSONClass ReadJSONFromDisk(string path)
		{
			try
			{
				JSONClass json = JSON.Parse(File.ReadAllText(path)).AsObject;
				return json;
			}
			catch (IOException e)
			{
				Debug.LogError("Error reading JSON file from disk: " + path);
				Debug.LogException(e);
			}
			return null;
		}

		/// <summary>
		/// Create a Texture2D from a PNG file.
		/// </summary>
		/// <param name="filePath">The path to the file.</param>
		public static Texture2D LoadPNG(string filePath)
		{		
			Texture2D tex = null;

			string fullFilePath = filePath;
			if (!Path.GetExtension(fullFilePath).Equals(".png"))
			{
				fullFilePath += ".png";
			}

			if (File.Exists(fullFilePath))
			{
				byte[] fileData = File.ReadAllBytes(fullFilePath);
				tex = new Texture2D(2, 2, TextureFormat.ARGB32, false); // (2, 2) is temporary power of 2 size; next line will resize automatically
				tex.LoadImage(fileData);
				tex.filterMode = FilterMode.Point;
				tex.mipMapBias = 0F;
			}
			else
			{
				Debug.LogError("ERROR: Could not find texture at: " + fullFilePath);
			}
			return tex;
		}

		public static Texture2D LoadPNGByteArray(byte[] array)
		{
			Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, false); // (2, 2) temporary POT size, next line resizes automatically
			tex.LoadImage(array);
			tex.filterMode = FilterMode.Point;
			tex.mipMapBias = 0F;
			return tex;
		}

		/// <summary>
		/// Loads an OGG file into the specified AudioSource
		/// </summary>
		public static IEnumerator LoadOGGIntoSource(string filePath, AudioSource source, bool playOnLoad = false, bool absolutePath = false)
		{
			if (!File.Exists(absolutePath ? filePath : PathCombine(Application.streamingAssetsPath, filePath)))
			{
				Debug.LogError("Could not locate OGG at " + filePath);
				yield break;
			}
		
			WWW www = new WWW("file:///" +  (absolutePath ? filePath : PathCombine(Application.streamingAssetsPath, filePath)));
			while (!www.isDone)
			{
				yield return null;
			}
			source.clip = www.GetAudioClip(true);

			if (playOnLoad) source.Play();
		}

		/// <summary>
		/// Loads a Torii object (3D model with animations) and returns a gameobject
		/// </summary>
		public static GameObject LoadObject(string filePath, bool collisionMesh, ResourceLifespan lifespan)
		{
			GameObject g = UnityEngine.Object.Instantiate(ResourceManager.Load<GameObject>(filePath, lifespan));
			TOBJ t = g.GetComponent<ToriiObject>().ToriiObj;

			Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
			foreach (Renderer r in renderers)
			{
				Material m = r.material;

				// load texture
				if (!string.IsNullOrEmpty(t.ObjectTexture))
				{
					if (Path.GetFileNameWithoutExtension(t.ObjectTexture).Contains("["))
					{
						// part of a texture set
						m.shader = Shader.Find(GameSettings.UseClassicShaders ? "LSD/PSX/TransparentSet" : "LSD/TransparentSet");

						string texNameWithoutExtension = Path.GetFileNameWithoutExtension(t.ObjectTexture);
						string baseTexName = texNameWithoutExtension.Substring(1);

						string pathToTextureDir = Path.GetDirectoryName(t.ObjectTexture);

						m.SetTexture("_MainTexA", ResourceManager.Load<Texture2D>(PathCombine(pathToTextureDir, "A" + baseTexName) + ".png", lifespan));
						m.SetTexture("_MainTexB", ResourceManager.Load<Texture2D>(PathCombine(pathToTextureDir, "B" + baseTexName) + ".png", lifespan));
						m.SetTexture("_MainTexC", ResourceManager.Load<Texture2D>(PathCombine(pathToTextureDir, "C" + baseTexName) + ".png", lifespan));
						m.SetTexture("_MainTexD", ResourceManager.Load<Texture2D>(PathCombine(pathToTextureDir, "D" + baseTexName) + ".png", lifespan));
					}
					else
					{
						m.shader = Shader.Find(GameSettings.UseClassicShaders ? "LSD/PSX/Transparent" : "Transparent/Diffuse");
						m.SetTexture("_MainTex", ResourceManager.Load<Texture2D>(t.ObjectTexture, lifespan));
					}
				}
			}

			// load animations
			if (t.NumberOfAnimations > 0)
			{
				ToriiObjectAnimator animator = g.AddComponent<ToriiObjectAnimator>();
				foreach (Transform child in g.transform)
				{
					animator.Objects.Add(child.gameObject);
				}
				animator.ToriiObject = t;
			}

			if (collisionMesh)
			{
				foreach (Transform child in g.transform)
				{
					child.gameObject.AddComponent<MeshCollider>();
				}
			}

			g.transform.localScale = new Vector3(-g.transform.localScale.x, g.transform.localScale.y, g.transform.localScale.z);
			g.SetActive(true);

			return g;
		}

		/// <summary>
		/// Loads MAP file geometry into a mesh and returns a gameobject
		/// </summary>
		public static GameObject LoadMap(string filePath, ResourceLifespan lifespan)
		{
			GameObject map = UnityEngine.Object.Instantiate(ResourceManager.Load<GameObject>(filePath, lifespan));
			map.SetActive(true);
            return map;
		}

		/// <summary>
		/// Loads a Torii map and returns a gameobject with entities in the torii map as child elements
		/// </summary>
		public static GameObject LoadToriiMap(string filePath, ResourceLifespan lifespan, out TMAP tmap)
		{
			// TODO: handle missing files

			tmap = ResourceManager.Load<TMAP>(filePath, lifespan);

			GameObject tmapObject = new GameObject(tmap.Header.Name);

			foreach (ENTITY e in tmap.Content.Entities)
			{
				GameObject entityObject = EntityInstantiator.Instantiate(e);
				entityObject.transform.SetParent(tmapObject.transform);
			}

			return tmapObject;
		}

		public static string GetLevelFromIndex(int index, string journal)
		{
			string[] levels = Directory.GetFiles(PathCombine(Application.streamingAssetsPath, "levels", journal), "*.tmap");
			return levels[index];
		}

		/// <summary>
		/// Combines A and B to form a full path.
		/// </summary>
		public static string PathCombine(string a, string b)
		{
			if (b.StartsWith("\\") || b.StartsWith("/"))
			{
				b = b.Substring(1);
			}
			return Path.Combine(a, b);
		}
		/// <summary>
		/// Combines any number of elements to form a full path.
		/// </summary>
		public static string PathCombine(params string[] componentStrings)
		{
			string path = componentStrings[0];
			for (int i = 1; i < componentStrings.Length; i++)
			{
				path = PathCombine(path, componentStrings[i]);
			}
			return path;
		}

		public static string NormalizePath(string path)
		{
			return Path.GetFullPath(new Uri(path).LocalPath)
				.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				.ToLowerInvariant();
		}
	}
}