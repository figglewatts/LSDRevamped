using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Torii.Graphics
{
    [Serializable]
    public class FastMesh
    {
        private const int MAX_INSTANCES_PER_CALL = 1023;
        public Mesh Mesh;
        public Material[] Materials;
        private readonly List<Transform> _transforms;

        private Matrix4x4[][] _cachedMatrices;

        // do we need to regenerate the cached matrices?
        private bool _dirty = true;
        public Options RenderOptions;

        public FastMesh(Mesh mesh, Material[] materials, Options options = new Options())
        {
            Mesh = mesh;
            Materials = materials;
            _transforms = new List<Transform>();
            RenderOptions = options;
        }

        public Transform AddInstance(Vector3 pos)
        {
            return AddInstance(pos, Quaternion.identity);
        }

        public Transform AddInstance(Vector3 pos, Quaternion rot)
        {
            return AddInstance(pos, rot, Vector3.one);
        }

        public Transform AddInstance(Vector3 pos, Quaternion rot, Vector3 scale)
        {
            Transform t = new Transform(pos, rot, scale);
            _transforms.Add(t);
            _dirty = true;
            return t;
        }

        public void Submit()
        {
            _cachedMatrices = generateCache(_transforms);
            _dirty = false;
        }

        public void Draw()
        {
            if (_dirty)
            {
                Submit();
            }

            for (int i = 0; i < Mesh.subMeshCount; i++)
            {
                for (int j = 0; j < _cachedMatrices.Length; j++)
                {
                    UnityEngine.Graphics.DrawMeshInstanced(Mesh, i, Materials[i], _cachedMatrices[j],
                        _cachedMatrices[j].Length,
                        properties: null, RenderOptions.CastShadows, RenderOptions.ReceiveShadows, RenderOptions.Layer,
                        RenderOptions.Camera);
                }
            }
        }

        private Matrix4x4[][] generateCache(IReadOnlyList<Transform> transforms)
        {
            int numberOfArrays = (int)Mathf.Ceil(transforms.Count / (float)MAX_INSTANCES_PER_CALL);
            var cache = new Matrix4x4[numberOfArrays][];

            var matrices = new Matrix4x4[transforms.Count];
            for (int i = 0; i < cache.Length; i++)
            {
                int batchStart = i * MAX_INSTANCES_PER_CALL;
                int batchLength = transforms.Count - batchStart > MAX_INSTANCES_PER_CALL - 1
                    ? MAX_INSTANCES_PER_CALL
                    : transforms.Count - batchStart;
                cache[i] = new Matrix4x4[batchLength];
                for (int j = 0; j < batchLength; j++)
                {
                    cache[i][j] = transforms[batchStart + j];
                }
            }

            return cache;
        }

        public struct Transform
        {
            private Vector3 position { get; }
            private Quaternion rotation { get; }
            private Vector3 scale { get; }

            public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
            {
                this.position = position;
                this.rotation = rotation;
                this.scale = scale;
            }

            public static implicit operator Matrix4x4(Transform t)
            {
                return Matrix4x4.Translate(t.position) * Matrix4x4.Rotate(t.rotation) * Matrix4x4.Scale(t.scale);
            }
        }

        public struct Options
        {
            public ShadowCastingMode CastShadows { get; }
            public bool ReceiveShadows { get; }
            public int Layer { get; }
            public Camera Camera { get; }

            public Options(ShadowCastingMode castShadows, bool receiveShadows, int layer, Camera camera)
            {
                CastShadows = castShadows;
                ReceiveShadows = receiveShadows;
                Layer = layer;
                Camera = camera;
            }
        }
    }
}
