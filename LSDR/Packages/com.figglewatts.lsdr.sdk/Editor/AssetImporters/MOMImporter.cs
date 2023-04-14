using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.SDK.Animation;
using UnityEditor.Animations;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    [ScriptedImporter(1, "mom")]
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

            GameObject momObj = ImportMOMAsset(ctx, mom, OpaqueMaterial, TransparentMaterial);
            ctx.SetMainObject(momObj);
        }

        public static GameObject ImportMOMAsset(AssetImportContext ctx,
            MOM mom,
            Material opaqueMaterial,
            Material transparentMaterial,
            string assetPrefix = "")
        {
            string assetName = Path.GetFileNameWithoutExtension(ctx.assetPath);

            // create meshes for the object
            List<Mesh> meshes = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);
            for (int i = 0; i < meshes.Count; i++)
            {
                Mesh mesh = meshes[i];
                mesh.name = $"{assetPrefix}{assetName}TMDObject{i}Mesh";
                ctx.AddObjectToAsset($"{assetPrefix}Mesh{i}", mesh);
            }

            // create a GameObject for the MOM
            GameObject momObj = new GameObject($"{assetPrefix}MOM");

            bool hasAnimations = mom.MOS.NumberOfTODs > 0 && mom.MOS.TODs[0].Frames.Length > 0;
            if (!hasAnimations)
                createMomWithoutAnimations(meshes, momObj, opaqueMaterial, transparentMaterial);
            else
            {
                AnimationClip[] clips =
                    createMomWithAnimations(ctx, meshes, momObj, mom, opaqueMaterial, transparentMaterial, assetPrefix);

                // create it
                AnimatorController controller = new AnimatorController
                {
                    name = $"{assetPrefix}AnimatorController"
                };
                string layerName = controller.MakeUniqueLayerName("Base Layer");
                AnimatorControllerLayer layer = new AnimatorControllerLayer
                {
                    name = layerName,
                    stateMachine = new AnimatorStateMachine
                    {
                        name = layerName,
                        hideFlags = HideFlags.HideInHierarchy
                    }
                };
                ctx.AddObjectToAsset($"{assetPrefix}AnimatorControllerStateMachine", layer.stateMachine);
                controller.AddLayer(layer);
                ctx.AddObjectToAsset($"{assetPrefix}AnimatorController", controller);

                // add the clips to it
                foreach (AnimationClip clip in clips)
                {
                    AnimatorState state = layer.stateMachine.AddState(clip.name);
                    state.hideFlags = HideFlags.HideInHierarchy;
                    state.motion = clip;
                    ctx.AddObjectToAsset($"{assetPrefix}AnimatorControllerState{clip.name}", state);
                }

                // add components to the root object
                momObj.AddComponent<AnimatedObject>();
                momObj.GetComponent<Animator>().runtimeAnimatorController = controller;
            }

            ctx.AddObjectToAsset($"{assetPrefix}MOM", momObj);
            return momObj;
        }

        protected static AnimationClip[] createMomWithAnimations(AssetImportContext ctx,
            List<Mesh> meshes,
            GameObject momObj,
            MOM mom,
            Material opaqueMaterial,
            Material transparentMaterial,
            string assetPrefix = "")
        {
            GameObject animRoot = new GameObject("0");
            animRoot.transform.SetParent(momObj.transform);

            MOMHelper momHelper = new MOMHelper(animRoot, opaqueMaterial, transparentMaterial);

            // create the object structure of the animation based on the first frame
            TODFrame animFirstFrame = mom.MOS.TODs[0].Frames[0];
            momHelper.CreateAnimationObjectHierarchy(animFirstFrame, meshes);

            // load the animations
            AnimationClip[] clips = new AnimationClip[mom.MOS.TODs.Length];
            for (int i = 0; i < mom.MOS.TODs.Length; i++)
            {
                AnimationClip animClip = momHelper.TODToClip(mom.MOS.TODs[i]);
                clips[i] = animClip;
                clips[i].name = $"{assetPrefix}MOMAnimation{i}";
                ctx.AddObjectToAsset(clips[i].name, animClip);
            }

            // pose the object in the first frame of its first animation
            momHelper.PoseObjectInFirstFrame(animFirstFrame);

            return clips;
        }

        protected static void createMomWithoutAnimations(List<Mesh> meshes,
            GameObject momObj,
            Material opaqueMaterial,
            Material transparentMaterial)
        {
            MOMHelper momHelper = new MOMHelper(momObj, opaqueMaterial, transparentMaterial);

            // create GameObjects for the TMD objects
            for (int i = 0; i < meshes.Count; i++)
            {
                GameObject meshObj = momHelper.MakeMeshObject(meshes[i], i);
                meshObj.transform.SetParent(momObj.transform);
            }
        }
    }
}
