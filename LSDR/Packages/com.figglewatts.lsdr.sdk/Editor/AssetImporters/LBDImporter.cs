using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.SDK.Animation;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(version: 1, ext: "lbd")]
    public class LBDImporter : ScriptedImporter
    {
        public Material OpaqueMaterial;
        public Material TransparentMaterial;

        protected MeshCombiner _meshCombiner;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            // read the LBD file
            LBD lbd;
            using (BinaryReader br = new BinaryReader(File.Open(ctx.assetPath, FileMode.Open)))
            {
                lbd = new LBD(br);
            }

            // now create the mesh for the LBD file
            _meshCombiner = new MeshCombiner(new[] {"opaque", "transparent"});
            var lbdMeshes = LibLSDUnity.CreateMeshesFromTMD(lbd.Tiles);
            int tileNo = 0;
            foreach (LBDTile tile in lbd.TileLayout)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileHeight;

                // create an LBD tile if we should draw it
                if (tile.DrawTile)
                {
                    createLBDTile(tile, x, y, lbdMeshes);

                    // now see if there are extra tiles
                    LBDTile curTile = tile;
                    int i = 0;
                    while (curTile.ExtraTileIndex >= 0 && i <= 1)
                    {
                        LBDTile extraTile = lbd.ExtraTiles[curTile.ExtraTileIndex];
                        createLBDTile(extraTile, x, y, lbdMeshes);
                        curTile = extraTile;
                        i++;
                    }
                }

                tileNo++;
            }

            var combinedMesh = _meshCombiner.Combine();
            combinedMesh.name = $"{Path.GetFileNameWithoutExtension(ctx.assetPath)} Mesh";

            // now create a GameObject for the mesh
            GameObject meshObj = new GameObject();
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            mf.sharedMesh = combinedMesh;
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();

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

            // and set the outputs
            ctx.AddObjectToAsset("LBD Object", meshObj);
            ctx.SetMainObject(meshObj);
            ctx.AddObjectToAsset("LBD Mesh", combinedMesh);

            if (lbd.Header.HasMML && lbd.MML.HasValue)
            {
                for (int i = 0; i < lbd.MML?.MOMs.Length; i++)
                {
                    var momObj = createMOM(ctx, lbd.MML.Value.MOMs[i], i);
                    momObj.transform.SetParent(meshObj.transform);
                }
            }

            // clean up unused assets
            foreach (var lbdMesh in lbdMeshes)
            {
                DestroyImmediate(lbdMesh);
            }
        }

        protected GameObject createMOM(AssetImportContext ctx, MOM mom, int index)
        {
            var assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // create meshes for the object
            var meshes = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);
            for (int i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                mesh.name = $"{assetName} MOM {index} TMD Object {i} Mesh";
                ctx.AddObjectToAsset($"MOM{index}Mesh{i}", mesh);
            }

            // create a GameObject for the MOM
            GameObject momObj = new GameObject($"MOM{index}");

            bool hasAnimations = mom.MOS.NumberOfTODs > 0 && mom.MOS.TODs[0].Frames.Length > 0;
            if (!hasAnimations)
            {
                createMomWithoutAnimations(meshes, momObj);
            }
            else
            {
                var clips = createMomWithAnimations(ctx, meshes, momObj, mom);

                // figure out where to put the animator controller
                var fileName = $"{assetName}MOM{index}Animator.controller";
                var dirName = Path.GetDirectoryName(ctx.assetPath);

                // create it
                var filePath = Path.Combine(dirName, fileName);
                var controller = AnimatorController.CreateAnimatorControllerAtPath(filePath);

                // add the clips to it
                for (int i = 0; i < clips.Length; i++)
                {
                    clips[i].name = $"{assetName}MOM{index}Animation{i}";
                    AssetDatabase.AddObjectToAsset(clips[i], controller);
                    controller.AddMotion(clips[i]);
                }

                // add components to the root object
                momObj.AddComponent<AnimatedObject>();
                momObj.AddComponent<Animator>().runtimeAnimatorController = controller;
            }

            return momObj;
        }

        protected AnimationClip[] createMomWithAnimations(AssetImportContext ctx,
            List<Mesh> meshes,
            GameObject momObj,
            MOM mom)
        {
            GameObject animRoot = new GameObject("0");
            animRoot.transform.SetParent(momObj.transform);

            MOMHelper momHelper = new MOMHelper(animRoot, OpaqueMaterial, TransparentMaterial);

            // create the object structure of the animation based on the first frame
            var animFirstFrame = mom.MOS.TODs[0].Frames[0];
            momHelper.CreateAnimationObjectHierarchy(animFirstFrame, meshes);

            // load the animations
            AnimationClip[] clips = new AnimationClip[mom.MOS.TODs.Length];
            for (int i = 0; i < mom.MOS.TODs.Length; i++)
            {
                var animClip = momHelper.TODToClip(mom.MOS.TODs[i]);
                clips[i] = animClip;
            }

            // pose the object in the first frame of its first animation
            momHelper.PoseObjectInFirstFrame(animFirstFrame);

            return clips;
        }

        protected void createMomWithoutAnimations(List<Mesh> meshes, GameObject momObj)
        {
            MOMHelper momHelper = new MOMHelper(momObj, OpaqueMaterial, TransparentMaterial);

            // create GameObjects for the TMD objects
            for (int i = 0; i < meshes.Count; i++)
            {
                var meshObj = momHelper.MakeMeshObject(meshes[i], i);
                meshObj.transform.SetParent(momObj.transform);
            }
        }

        protected void createLBDTile(LBDTile tile, int x, int y, List<Mesh> tileMeshes)
        {
            var mesh = tileMeshes[tile.TileType];

            // create the quaternion describing the tile's orientation from the rotation data
            var tileOrientation = Quaternion.identity;
            switch (tile.TileDirection)
            {
                case LBDTile.TileDirections.Deg90:
                    tileOrientation = Quaternion.AngleAxis(90, Vector3.up);
                    break;
                case LBDTile.TileDirections.Deg180:
                    tileOrientation = Quaternion.AngleAxis(180, Vector3.up);
                    break;
                case LBDTile.TileDirections.Deg270:
                    tileOrientation = Quaternion.AngleAxis(270, Vector3.up);
                    break;
            }

            // create the matrix that will transform the tile to it's position and rotation
            var tileTransformMatrix = Matrix4x4.Translate(new Vector3(x, -tile.TileHeight, y)) *
                                      Matrix4x4.Rotate(tileOrientation);

            // create a combine instance for the mesh
            var combineInstance = new CombineInstance
            {
                mesh = mesh,
                subMeshIndex = 0,
                transform = tileTransformMatrix
            };

            // add it to the mesh combiner
            _meshCombiner.Add(combineInstance, "opaque");

            // if we have a transparent part we'll have to add the transparent part as a combine instance
            if (mesh.subMeshCount > 1)
            {
                var transparentCombineInstance = new CombineInstance
                {
                    mesh = mesh,
                    subMeshIndex = 1,
                    transform = tileTransformMatrix
                };
                _meshCombiner.Add(transparentCombineInstance, "transparent");
            }
        }
    }
}
