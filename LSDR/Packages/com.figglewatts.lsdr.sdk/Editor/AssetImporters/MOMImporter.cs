using System.IO;
using libLSD.Formats;
using LSDR.SDK.IO;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, ext: "mom")]
    public class MOMImporter : ScriptedImporter
    {
        public Material OpaqueMaterial;
        public Material TransparentMaterial;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // read the MOM file
            MOM mom;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                mom = new MOM(br);
            }

            // create data for combining into single mesh
            var meshes = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);

            // create a GameObject for the MOM
            GameObject momObj = new GameObject();

            // create GameObjects for the TMD objects
            for (int i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                mesh.name = $"{Path.GetFileNameWithoutExtension(ctx.assetPath)} TMD Object {i} Mesh";
                var meshObj = new GameObject($"Object{i}");
                MeshFilter mf = meshObj.AddComponent<MeshFilter>();
                mf.sharedMesh = mesh;
                MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();

                if (mesh.subMeshCount > 1)
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

                meshObj.transform.SetParent(momObj.transform);
                ctx.AddObjectToAsset($"Mesh{i}", mesh);
            }

            ctx.AddObjectToAsset("MOM Object", momObj);
            ctx.SetMainObject(momObj);
        }
    }
}
