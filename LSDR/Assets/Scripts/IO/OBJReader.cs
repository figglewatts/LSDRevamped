using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace LSDR.IO
{
    /// <summary>
    ///     OBJReader is used for loading OBJ files.
    /// </summary>
    // TODO: this can probably be replaced with a better written parser...
    public static class OBJReader
    {
        /// <summary>
        ///     Read an OBJ from a file and create a GameObject of it.
        /// </summary>
        /// <param name="pathToObj">The path to the OBJ file.</param>
        /// <returns>A GameObject with the created OBJ mesh.</returns>
        public static GameObject ReadOBJFile(string pathToObj)
        {
            string objString = File.ReadAllText(pathToObj);
            GameObject gameObject = ReadOBJString(objString);
            return gameObject;
        }

        /// <summary>
        ///     Read an OBJ string and convert it to a GameObject.
        /// </summary>
        /// <param name="obj">The OBJ string.</param>
        /// <returns>A GameObject containing the stored mesh.</returns>
        public static GameObject ReadOBJString(string obj)
        {
            GameObject baseGameObject = new GameObject("importedObj");

            // a handle to the current object, used when processing multiple objects
            int objectHandle = 0;

            bool firstObject = true;

            // get ready to see the word "List" like 20 times
            var objObjects = new List<GameObject>();
            var meshes = new List<Mesh>();
            var vertices = new List<Vector3>();
            var UVs = new List<Vector2>();
            var normals = new List<Vector3>();
            var triangles = new List<List<int>>();
            var uvIndices = new List<List<int?>>();
            var normalIndices = new List<List<int?>>();
            // all that was so we can support multiple objects within an OBJ file

            // set up the lists before processing
            objObjects.Add(new GameObject());
            objObjects[objectHandle].transform.SetParent(baseGameObject.transform);
            objObjects[objectHandle].AddComponent<MeshRenderer>();
            objObjects[objectHandle].AddComponent<MeshFilter>();
            meshes.Add(objObjects[objectHandle].GetComponent<MeshFilter>().mesh);
            triangles.Add(new List<int>());
            uvIndices.Add(new List<int?>());
            normalIndices.Add(new List<int?>());

            // set up buffered streams to read the OBJ string
            byte[] byteArray = Encoding.ASCII.GetBytes(obj);
            using (MemoryStream ms = new MemoryStream(byteArray))
            using (BufferedStream bs = new BufferedStream(ms))
            using (StreamReader sr = new StreamReader(bs))
            {
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
                        Vector3 vertex = new Vector3(StringToFloat(words[1]), StringToFloat(words[2]),
                            StringToFloat(words[3]));
                        vertices.Add(vertex);
                    }
                    else if (words[0].Equals("vn")) // normal
                    {
                        Vector3 normal = new Vector3(StringToFloat(words[1]), StringToFloat(words[2]),
                            StringToFloat(words[3]));
                        normals.Add(normal);
                    }
                    else if (words[0].Equals("vt")) // UV
                    {
                        Vector2 uv = new Vector2(StringToFloat(words[1]), StringToFloat(words[2]));
                        UVs.Add(uv);
                    }
                    else if (words[0].Equals("f")) // face
                    {
                        int numVertices = words.Length - 1;
                        bool isQuad = numVertices % 4 == 0;

                        int[] verts = new int[numVertices];
                        int[] uvs = new int[numVertices];
                        int[] norms = new int[numVertices];

                        // for each set of indices
                        for (int i = 1; i < words.Length; i++) // starts at 1 to account for leading 'f'
                        {
                            string[] indices = words[i].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                            if (words[i].Contains("//") && indices.Length == 2)
                            {
                                // vertex & vertex normal, no UV data
                                // v//vn
                                verts[i - 1] = int.Parse(indices[0], CultureInfo.InvariantCulture);
                                norms[i - 1] = int.Parse(indices[1], CultureInfo.InvariantCulture);
                            }
                            else if (indices.Length == 2)
                            {
                                // vertex & UV data, no normals
                                // v/vt
                                verts[i - 1] = int.Parse(indices[0], CultureInfo.InvariantCulture);
                                uvs[i - 1] = int.Parse(indices[1], CultureInfo.InvariantCulture);
                            }
                            else if (indices.Length == 3)
                            {
                                // all 3 present
                                // v/vt/vn
                                verts[i - 1] = int.Parse(indices[0], CultureInfo.InvariantCulture);
                                uvs[i - 1] = int.Parse(indices[1], CultureInfo.InvariantCulture);
                                norms[i - 1] = int.Parse(indices[2], CultureInfo.InvariantCulture);
                            }
                            else if (indices.Length == 1)
                            {
                                // it's probably just vertex data
                                // v
                                verts[i - 1] = int.Parse(indices[0], CultureInfo.InvariantCulture);
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
            }

            // build the mesh for each object
            for (int i = 0; i < objObjects.Count; i++)
            {
                Mesh mesh = meshes[i];
                BuildMesh(vertices.ToArray(), triangles[i].ToArray(), UVs.ToArray(), uvIndices[i].ToArray(),
                    normals.ToArray(), normalIndices[i].ToArray(), ref mesh);
                objObjects[i].transform.localScale = new Vector3(x: -1, y: 1, z: 1);
            }

            return baseGameObject;
        }

        /// <summary>
        ///     Build a mesh from a given set of verts, tries, UVs, indices, and normals.
        /// </summary>
        /// <param name="vertices">An array of vertices.</param>
        /// <param name="triangles">An array of indices.</param>
        /// <param name="uvs">An array of UVs.</param>
        /// <param name="uvIndices">An array of indices into the UV array.</param>
        /// <param name="normals">An array of normals.</param>
        /// <param name="normalIndices">An array of indices into the normals array.</param>
        /// <param name="m">The Mesh we're using to build this mesh.</param>
        private static void BuildMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs, int?[] uvIndices,
            Vector3[] normals,
            int?[] normalIndices, ref Mesh m)
        {
            var actualUVs = new Vector2[triangles.Length];
            var actualVertices = new Vector3[triangles.Length];
            var actualNormals = new Vector3[triangles.Length];
            int[] actualTriangles = new int[triangles.Length];

            // for each index, populate the arrays corresponding to it
            for (int i = 0; i < triangles.Length; i++)
            {
                actualVertices[i] = vertices[triangles[i]];
                if (uvs.Length > 0)
                {
                    actualUVs[i] = uvIndices[i] == null ? Vector2.zero : uvs[uvIndices[i] ?? default(int)];
                }
                if (normals.Length > 0)
                {
                    actualNormals[i] = normalIndices[i] == null || normalIndices[i] == -1
                        ? Vector3.zero
                        : normals[normalIndices[i] ?? default(int)];
                }
                actualTriangles[i] = i;
            }

            // assign these new arrays to the mesh
            m.vertices = actualVertices;
            m.uv = actualUVs;
            m.normals = actualNormals;
            m.triangles = actualTriangles;
            m.RecalculateNormals();
            m.RecalculateBounds();
        }

        // convert a string to a float
        private static float StringToFloat(string s)
        {
            float returnVal;
            float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out returnVal);
            return returnVal;
        }
    }
}
