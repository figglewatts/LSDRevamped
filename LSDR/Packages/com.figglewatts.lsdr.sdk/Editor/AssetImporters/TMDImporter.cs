using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.SDK.Visual;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, "tmd")]
    public class TMDImporter : ScriptedImporter
    {
        public Material OpaqueMaterial;
        public Material TransparentMaterial;

        protected MeshCombiner _meshCombiner;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // read the TMD file
            TMD tmd;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                tmd = new TMD(br);
            }

            // create data for combining into single mesh
            List<Mesh> meshes = LibLSDUnity.CreateMeshesFromTMD(tmd);
            _meshCombiner = new MeshCombiner();
            _meshCombiner.SetSubmeshSettings("opaque", useMatrices: false);
            _meshCombiner.SetSubmeshSettings("transparent", useMatrices: false);
            foreach (Mesh tmdObj in meshes)
            {
                _meshCombiner.Add(new CombineInstance
                {
                    mesh = tmdObj,
                    subMeshIndex = 0
                }, "opaque");

                // if it has a transparent part, we need to add it
                if (tmdObj.subMeshCount > 1)
                {
                    _meshCombiner.Add(new CombineInstance
                    {
                        mesh = tmdObj,
                        subMeshIndex = 1
                    }, "transparent");
                }
            }

            // combine the temporary meshes into the final mesh
            Mesh combinedMesh = _meshCombiner.Combine();
            combinedMesh.name = $"{Path.GetFileNameWithoutExtension(ctx.assetPath)} Mesh";

            // create GameObject for the mesh
            GameObject meshObj = new GameObject();
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            mf.sharedMesh = combinedMesh;
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
            meshObj.AddComponent<ShaderSetter>();

            if (combinedMesh.subMeshCount > 1)
            {
                mr.sharedMaterials = new[]
                {
                    OpaqueMaterial == null
                        ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                        : OpaqueMaterial,
                    TransparentMaterial == null
                        ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                        : TransparentMaterial
                };
            }
            else
            {
                mr.sharedMaterial = OpaqueMaterial == null
                    ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                    : OpaqueMaterial;
            }

            // set the outputs
            ctx.AddObjectToAsset("TMD Object", meshObj);
            ctx.SetMainObject(meshObj);
            ctx.AddObjectToAsset("TMD Mesh", combinedMesh);

            // clean up unused assets
            foreach (Mesh tmdMesh in meshes)
            {
                DestroyImmediate(tmdMesh);
            }
        }
    }
}
