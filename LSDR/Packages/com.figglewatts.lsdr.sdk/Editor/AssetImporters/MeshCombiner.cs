using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    /// <summary>
    /// Utility class for combining meshes with submeshes. Should be used in editor only.
    ///
    /// Usage:
    /// <code>
    /// var mc = new MeshCombiner(new[] {"opaque", "transparent"});
    /// mc.SetSubmeshSettings("transparent", mergeSubMeshes: false, useMatrices: false);
    ///
    /// foreach (var mesh in meshes)
    /// {
    ///     mc.Add(new CombineInstance
    ///     {
    ///         mesh = mesh,
    ///         subMeshIndex = 0,
    ///         transform = Matrix4x4.Identity
    ///     }, "opaque");
    ///
    ///     if (mesh.subMeshCount > 1)
    ///     {
    ///         mc.Add(new CombineInstance
    ///         {
    ///             mesh = mesh,
    ///             subMeshIndex = 1,
    ///             transform = Matrix4x4.Identity
    ///         }, "transparent");
    ///     }
    /// }
    ///
    /// var combinedMesh = mc.Combine();
    /// </code>
    /// </summary>
    public class MeshCombiner
    {
        protected readonly Dictionary<string, SubmeshType> _combineStorage;

        protected class SubmeshType
        {
            public bool MergeSubMeshes = true;
            public bool UseMatrices = true;
            public bool HasLightmapData = false;
            public readonly List<CombineInstance> CombineInstances;

            public SubmeshType() { CombineInstances = new List<CombineInstance>(); }
        }

        public MeshCombiner()
        {
            _combineStorage = new Dictionary<string, SubmeshType>();
        }

        public void Add(CombineInstance instance, string submeshType = "default")
        {
            ensureSubmeshExists(submeshType);
            _combineStorage[submeshType].CombineInstances.Add(instance);
        }

        public void SetSubmeshSettings(string submeshType = "default",
            bool mergeSubMeshes = true,
            bool useMatrices = true,
            bool hasLightmapData = false)
        {
            ensureSubmeshExists(submeshType);
            var submesh = _combineStorage[submeshType];
            submesh.UseMatrices = useMatrices;
            submesh.MergeSubMeshes = mergeSubMeshes;
            submesh.HasLightmapData = hasLightmapData;
        }

        public Mesh Combine()
        {
            // create temporary meshes for each submesh type, to be later combined
            // this is because Unity does not let us combine and preserve submeshes, so we create
            // and combine them separately, then combine at the end with mergeSubMeshes false
            var createdMeshes = new List<Mesh>();
            foreach (var kv in _combineStorage)
            {
                var (submeshName, submeshType) = (kv.Key, kv.Value);
                int vertsRunningCount = 0;
                Mesh submesh = new Mesh
                {
                    subMeshCount = 1,
                    vertices = submeshType.CombineInstances
                                          .SelectMany(
                                               i => i.mesh.vertices.Select(v => i.transform.MultiplyPoint3x4(v)))
                                          .ToArray(),
                    normals = submeshType.CombineInstances.SelectMany(i => i.mesh.normals).ToArray(),
                    colors32 = submeshType.CombineInstances.SelectMany(i => i.mesh.colors32).ToArray(),
                    uv = submeshType.CombineInstances.SelectMany(i => i.mesh.uv).ToArray(),
                    triangles = submeshType.CombineInstances.SelectMany(i =>
                                            {
                                                int count = vertsRunningCount;
                                                var indices = i.mesh.GetTriangles(i.subMeshIndex)
                                                               .Select(idx => count + idx);
                                                vertsRunningCount += i.mesh.vertexCount;
                                                return indices;
                                            })
                                           .ToArray(), // need to add prior numverts
                    name = submeshName
                };

                createdMeshes.Add(submesh);
            }

            // combine the temporary meshes into the final mesh
            Mesh combinedMesh = new Mesh
            {
                subMeshCount = createdMeshes.Count,
                vertices = createdMeshes.SelectMany(m => m.vertices).ToArray(),
                normals = createdMeshes.SelectMany(m => m.normals).ToArray(),
                colors32 = createdMeshes.SelectMany(m => m.colors32).ToArray(),
                uv = createdMeshes.SelectMany(m => m.uv).ToArray()
            };

            int submeshNum = 0;
            int runningIndexTotal = 0;
            foreach (var mesh in createdMeshes)
            {
                if (mesh.vertexCount > 0)
                {
                    combinedMesh.SetTriangles(mesh.triangles, submeshNum, calculateBounds: true);
                    runningIndexTotal += mesh.vertices.Length;
                }
                submeshNum++;
            }
            combinedMesh.RecalculateNormals();
            combinedMesh.RecalculateBounds();
            combinedMesh.RecalculateTangents();
            combinedMesh.Optimize();

            // clean up the temporary meshes we made
            foreach (var mesh in createdMeshes)
            {
                Object.DestroyImmediate(mesh);
            }

            return combinedMesh;
        }

        protected void ensureSubmeshExists(string submeshType)
        {
            var exists = _combineStorage.ContainsKey(submeshType);
            if (!exists)
            {
                _combineStorage[submeshType] = new SubmeshType();
            }
        }
    }
}
