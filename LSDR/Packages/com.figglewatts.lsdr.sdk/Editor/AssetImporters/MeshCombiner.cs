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
        protected List<Mesh> _createdMeshes;

        protected class SubmeshType
        {
            public bool MergeSubMeshes = true;
            public bool UseMatrices = true;
            public bool HasLightmapData = false;
            public readonly List<CombineInstance> CombineInstances;

            public SubmeshType() { CombineInstances = new List<CombineInstance>(); }
        }

        public MeshCombiner(string[] submeshes = null)
        {
            _combineStorage = new Dictionary<string, SubmeshType>();
            _createdMeshes = new List<Mesh>();

            if (submeshes == null)
            {
                submeshes = new[] {"default"};
            }

            foreach (var submeshType in submeshes)
            {
                _combineStorage[submeshType] = new SubmeshType();
            }
        }

        public void Add(CombineInstance instance, string submeshType = "default")
        {
            if (!checkSubmeshExists(submeshType)) return;
            _combineStorage[submeshType].CombineInstances.Add(instance);
        }

        public void SetSubmeshSettings(string submeshType = "default",
            bool mergeSubMeshes = true,
            bool useMatrices = true,
            bool hasLightmapData = false)
        {
            if (!checkSubmeshExists(submeshType)) return;
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
            _createdMeshes.Clear();
            foreach (var kv in _combineStorage)
            {
                var (submeshName, submeshType) = (kv.Key, kv.Value);
                Mesh submesh = new Mesh();
                submesh.CombineMeshes(submeshType.CombineInstances.ToArray(), submeshType.MergeSubMeshes,
                    submeshType.UseMatrices, submeshType.HasLightmapData);
                submesh.name = submeshName;
                _createdMeshes.Add(submesh);
            }

            // combine the temporary meshes into the final mesh
            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(
                _createdMeshes.Select(m => new CombineInstance {mesh = m}).ToArray(),
                mergeSubMeshes: false, useMatrices: false);
            combinedMesh.RecalculateNormals();
            combinedMesh.RecalculateBounds();
            combinedMesh.RecalculateTangents();
            combinedMesh.Optimize();

            // clean up the temporary meshes we made
            foreach (var mesh in _createdMeshes)
            {
                Object.DestroyImmediate(mesh);
            }

            return combinedMesh;
        }

        protected bool checkSubmeshExists(string submeshType)
        {
            var exists = _combineStorage.ContainsKey(submeshType);
            if (!exists)
            {
                Debug.LogError($"Unable to add mesh with submesh type '{submeshType}' to the combine. " +
                               "Submesh type did not exist.");
            }

            return exists;
        }
    }
}
