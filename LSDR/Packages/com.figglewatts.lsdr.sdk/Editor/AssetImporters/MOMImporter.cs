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

            var assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // create meshes for the object
            var meshes = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);
            for (int i = 0; i < meshes.Count; i++)
            {
                var mesh = meshes[i];
                mesh.name = $"{assetName} TMD Object {i} Mesh";
                ctx.AddObjectToAsset($"Mesh{i}", mesh);
            }

            // create a GameObject for the MOM
            GameObject momObj = new GameObject();

            bool hasAnimations = mom.MOS.NumberOfTODs > 0 && mom.MOS.TODs[0].Frames.Length > 0;
            if (!hasAnimations)
            {
                createMomWithoutAnimations(meshes, momObj);
            }
            else
            {
                var clips = createMomWithAnimations(ctx, meshes, momObj, mom);

                // figure out where to put the animator controller
                var fileName = $"{assetName}Animator.controller";
                var dirName = Path.GetDirectoryName(ctx.assetPath);
                if (dirName == null)
                {
                    ctx.LogImportError($"Unable to get asset directory from path: {ctx.assetPath}");
                    return;
                }

                // create it
                var filePath = Path.Combine(dirName, fileName);
                var controller = AnimatorController.CreateAnimatorControllerAtPath(filePath);

                // add the clips to it
                for (int i = 0; i < clips.Length; i++)
                {
                    clips[i].name = $"{assetName}Animation{i}";
                    //ctx.AddObjectToAsset($"Animation {i}", clips[i]);
                    AssetDatabase.AddObjectToAsset(clips[i], controller);
                    controller.AddMotion(clips[i]);
                }

                // add components to the root object
                momObj.AddComponent<AnimatedObject>();
                momObj.AddComponent<Animator>().runtimeAnimatorController = controller;
            }

            ctx.AddObjectToAsset("MOM Object", momObj);
            ctx.SetMainObject(momObj);
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
    }
}
