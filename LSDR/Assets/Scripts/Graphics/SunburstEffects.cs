// Sunburst effects.
// By Keijiro Takahashi, 2013
// https://github.com/keijiro/unity-sunburst-mesh-fx
using UnityEngine;
using System.Collections;

namespace Graphics
{
	[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
	public class SunburstEffects : MonoBehaviour
	{
		#region Public variables

		public int beamCount = 100;
		[Range(0.01f, 0.5f)] public float beamWidth = 0.1f;
		[Range(0.1f, 10.0f)] public float speed = 0.4f;
		[Range(1.0f, 10.0f)] public float scalePower = 1.0f;

		#endregion

		#region Beam vectors

		Vector3[] beamDir;
		Vector3[] beamExt;

		#endregion

		#region Mesh data

		Mesh mesh;
		Vector3[] vertices;

		#endregion

		#region Animation parameters

		const float indexToNoise = 0.77f;
		float time;

		#endregion

		#region Private functions

		void ResetBeams()
		{
			// Allocate arrays.
			beamDir = new Vector3[beamCount];
			beamExt = new Vector3[beamCount];
			vertices = new Vector3[beamCount*3];
			var normals = new Vector3[beamCount*3];

			// Initialize the beam vectors.
			var normalIndex = 0;
			for (var i = 0; i < beamCount; i++)
			{
				// Make a beam in a completely random way.
				var dir = Random.onUnitSphere;
				var ext = Random.onUnitSphere;
				beamDir[i] = dir;
				beamExt[i] = ext;

				// Use a slightly modified vector on the first vertex to make a gradation.
				var normal = Vector3.Cross(dir, ext).normalized;
				normals[normalIndex++] = Vector3.Lerp(dir, normal, 0.5f).normalized;
				normals[normalIndex++] = normal;
				normals[normalIndex++] = normal;
			}

			// Initialize the triangle set.
			var indices = new int[beamCount*3];
			for (var i = 0; i < indices.Length; i++)
			{
				indices[i] = i;
			}

			// Initialize the mesh.
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = indices;
		}

		void UpdateVertices()
		{
			var vertexIndex = 0;
			for (var i = 0; i < beamCount; i++)
			{
				// Use 2D Perlin noise to animate the beam.
				var scale = Mathf.Pow(Mathf.PerlinNoise(time, i*indexToNoise), scalePower);

				// Never modify the first vertex.
				vertexIndex++;

				// Update the 2nd and 3rd vertices.
				var tip = beamDir[i]*scale;
				var ext = beamExt[i]*beamWidth*scale;
				vertices[vertexIndex++] = tip - ext;
				vertices[vertexIndex++] = tip + ext;
			}
		}

		#endregion

		#region Monobehaviour functions

		void Awake()
		{
			// Initialize the mesh instance.
			mesh = new Mesh();
			mesh.MarkDynamic();
			GetComponent<MeshFilter>().sharedMesh = mesh;

			// Initialize the beam array.
			ResetBeams();
		}

		void Update()
		{
			// Reset the beam array if the number was changed.
			if (beamCount != beamDir.Length)
			{
				ResetBeams();
			}

			// Do animation.
			UpdateVertices();

			// Update the vertex array.
			mesh.vertices = vertices;

			// Advance the time count.
			time += Time.deltaTime*speed;
		}

		#endregion
	}
}