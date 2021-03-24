using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using libLSD.Formats.Packets;
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

            // create meshes for the object
            var meshes = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);
            for (int i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                mesh.name = $"{Path.GetFileNameWithoutExtension(ctx.assetPath)} TMD Object {i} Mesh";
                ctx.AddObjectToAsset($"Mesh{i}", mesh);
            }

            // create a GameObject for the MOM
            GameObject momObj = new GameObject();

            bool hasAnimations = mom.MOS.NumberOfTODs > 0 && mom.MOS.TODs[0].Frames.Length > 0;
            if (!hasAnimations)
            {
                createMomWithoutAnimations(ctx, meshes, momObj);
            }
            else
            {
                createMomWithAnimations(ctx, meshes, momObj, mom);
            }

            ctx.AddObjectToAsset("MOM Object", momObj);
            ctx.SetMainObject(momObj);
        }

        protected void createMomWithAnimations(AssetImportContext ctx, List<Mesh> meshes, GameObject momObj, MOM mom)
        {
            GameObject animRoot = new GameObject("0");
            animRoot.transform.SetParent(momObj.transform);

            MOMAnimationHelper animationHelper = new MOMAnimationHelper(animRoot);

            // create the object structure of the animation
            var animFirstFrame = mom.MOS.TODs[0].Frames[0];
            foreach (var packet in animFirstFrame.Packets)
            {
                if (packet.Data is TODObjectControlPacketData objControl)
                {
                    if (objControl.ObjectControl == TODObjectControlPacketData.ObjectControlType.Create)
                    {
                        // create a new object in the transform hierarchy/object table
                        var newObj = new GameObject($"{packet.ObjectID}");
                        animationHelper.CreateObject(packet.ObjectID, newObj.transform);
                    }
                }
                else if (packet.Data is TODObjectIDPacketData objId)
                {
                    if (packet.PacketType == TODPacket.PacketTypes.TMDDataID)
                    {
                        // assign a mesh to an object in the transform hierarchy/object table
                        var meshObj = makeMeshObject(meshes[objId.ObjectID - 1], objId.ObjectID - 1);
                        var parent = animationHelper.GetObject(packet.ObjectID);
                        meshObj.transform.SetParent(parent);
                    }
                    else if (packet.PacketType == TODPacket.PacketTypes.ParentObjectID)
                    {
                        // parent something in the transform hierarchy to something else in the transform hierarchy
                        animationHelper.SetObjectParent(packet.ObjectID, objId.ObjectID);
                    }
                }
            }

            // load the animations
            for (int i = 0; i < mom.MOS.TODs.Length; i++)
            {
                var animClip = animationHelper.TODToClip(mom.MOS.TODs[i]);
                animClip.name = $"Animation{i}";
                ctx.AddObjectToAsset($"Animation {i}", animClip);
            }

            // pose the object in the first frame of its first animation
            foreach (var packet in animFirstFrame.Packets)
            {
                if (packet.Data is TODCoordinatePacketData packetData)
                {
                    var objTransform = animationHelper.GetObject(packet.ObjectID);
                    if (packetData.HasScale)
                    {
                        if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                        {
                            objTransform.localScale = new Vector3(packetData.ScaleX / 4096f,
                                packetData.ScaleY / 4096f,
                                packetData.ScaleZ / 4096f);
                        }
                        else
                        {
                            objTransform.localScale = Vector3.Scale(objTransform.localScale, new Vector3(
                                packetData.ScaleX / 4096f,
                                packetData.ScaleY / 4096f,
                                packetData.ScaleZ / 4096f));
                        }
                    }

                    if (packetData.HasTranslation)
                    {
                        if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                        {
                            Vector3 pos = new Vector3(packetData.TransX, -packetData.TransY, packetData.TransZ) /
                                          2048f;
                            objTransform.localPosition = pos;
                        }
                        else
                        {
                            objTransform.localPosition =
                                objTransform.localPosition +
                                new Vector3(packetData.TransX, -packetData.TransY, packetData.TransZ) /
                                2048f;
                        }
                    }

                    if (packetData.HasRotation)
                    {
                        float pitch = -packetData.RotX / 4096f;
                        float yaw = packetData.RotY / 4096f;
                        float roll = -packetData.RotZ / 4096f;

                        if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                        {
                            var x = Quaternion.AngleAxis(pitch, Vector3.right);
                            var y = Quaternion.AngleAxis(yaw, Vector3.up);
                            var z = Quaternion.AngleAxis(roll, Vector3.forward);
                            objTransform.localRotation = x * y * z;
                        }
                        else
                        {
                            objTransform.Rotate(Vector3.right, pitch);
                            objTransform.Rotate(Vector3.up, yaw);
                            objTransform.Rotate(Vector3.forward, roll);
                        }
                    }
                }
            }
        }

        protected void createMomWithoutAnimations(AssetImportContext ctx, List<Mesh> meshes, GameObject momObj)
        {
            // create GameObjects for the TMD objects
            for (int i = 0; i < meshes.Count; i++)
            {
                var meshObj = makeMeshObject(meshes[i], i);
                meshObj.transform.SetParent(momObj.transform);
            }
        }

        protected GameObject makeMeshObject(Mesh mesh, int id)
        {
            var meshObj = new GameObject($"{id}");
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

            return meshObj;
        }
    }
}
