using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Formats.Packets;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    public class MOMAnimationHelper
    {
        protected GameObject _rootObject;
        protected readonly Dictionary<int, AnimationObject> _objectTable;

        protected class AnimationObject
        {
            public Transform Transform { get; }
            public AnimationObject Parent { get; protected set; }

            public AnimationObject(Transform transform)
            {
                Transform = transform;
                Parent = null;
            }

            public void SetParent(AnimationObject parent)
            {
                Parent = parent;
                Transform.SetParent(parent.Transform);
            }

            public string GetPath()
            {
                return Parent == null ? Transform.gameObject.name : $"{Parent.GetPath()}/{Transform.gameObject.name}";
            }
        }

        protected struct FrameTransformation
        {
            public TODCoordinatePacketData CoordPacket;
            public int FrameNumber;

            public TransformationResult PerformTransformation(float frameTime, Transform currentTransform)
            {
                var result = new TransformationResult
                    {Keyframes = new Keyframe[10], CurrentTransform = currentTransform};
                if (CoordPacket.HasScale)
                {
                    var scale = new Vector3(CoordPacket.ScaleX / 4096f, CoordPacket.ScaleY / 4096f,
                        CoordPacket.ScaleZ / 4096f);
                    if (CoordPacket.MatrixType == TODPacketData.PacketDataType.Absolute)
                    {
                        result.CurrentTransform.localScale = scale;
                    }
                    else
                    {
                        result.CurrentTransform.localScale = Vector3.Scale(result.CurrentTransform.localScale, scale);
                    }
                }

                if (CoordPacket.HasTranslation)
                {
                    var translation = new Vector3(CoordPacket.TransX, -CoordPacket.TransY, CoordPacket.TransZ) / 2048f;
                    if (CoordPacket.MatrixType == TODPacketData.PacketDataType.Absolute)
                    {
                        result.CurrentTransform.localPosition = translation;
                    }
                    else
                    {
                        result.CurrentTransform.localPosition += translation;
                    }
                }

                if (CoordPacket.HasRotation)
                {
                    float pitch = -CoordPacket.RotX / 4096f;
                    float yaw = CoordPacket.RotY / 4096f;
                    float roll = -CoordPacket.RotZ / 4096f;

                    if (CoordPacket.MatrixType == TODPacketData.PacketDataType.Absolute)
                    {
                        var x = Quaternion.AngleAxis(pitch, Vector3.right);
                        var y = Quaternion.AngleAxis(yaw, Vector3.up);
                        var z = Quaternion.AngleAxis(roll, Vector3.forward);
                        result.CurrentTransform.localRotation = x * y * z;
                    }
                    else
                    {
                        result.CurrentTransform.Rotate(Vector3.right, pitch);
                        result.CurrentTransform.Rotate(Vector3.up, yaw);
                        result.CurrentTransform.Rotate(Vector3.forward, roll);
                    }
                }

                // now fill the keyframes in with transform information
                float time = FrameNumber * frameTime;
                result.Keyframes[0] = new Keyframe(time, result.CurrentTransform.localPosition.x);
                result.Keyframes[1] = new Keyframe(time, result.CurrentTransform.localPosition.y);
                result.Keyframes[2] = new Keyframe(time, result.CurrentTransform.localPosition.z);
                result.Keyframes[3] = new Keyframe(time, result.CurrentTransform.localRotation.x);
                result.Keyframes[4] = new Keyframe(time, result.CurrentTransform.localRotation.y);
                result.Keyframes[5] = new Keyframe(time, result.CurrentTransform.localRotation.z);
                result.Keyframes[6] = new Keyframe(time, result.CurrentTransform.localRotation.w);
                result.Keyframes[7] = new Keyframe(time, result.CurrentTransform.localScale.x);
                result.Keyframes[8] = new Keyframe(time, result.CurrentTransform.localScale.y);
                result.Keyframes[9] = new Keyframe(time, result.CurrentTransform.localScale.z);

                return result;
            }
        }

        protected struct TransformationResult
        {
            public Keyframe[] Keyframes;
            public Transform CurrentTransform;
        }

        public MOMAnimationHelper(GameObject root)
        {
            _rootObject = root;
            root.name = "0";
            _objectTable = new Dictionary<int, AnimationObject> {[0] = new AnimationObject(root.transform)};
        }

        public Transform GetObject(int id) { return _objectTable[id].Transform; }

        public void SetObjectParent(int id, int parentId) { _objectTable[id].SetParent(_objectTable[parentId]); }

        public void CreateObject(int id, Transform obj)
        {
            var animObj = new AnimationObject(obj);
            animObj.Transform.SetParent(_rootObject.transform);
            _objectTable[id] = animObj;
        }

        public void PrintAllPaths()
        {
            foreach (var animObj in _objectTable.Values)
            {
                Debug.Log(animObj.GetPath());
            }
        }

        public AnimationClip TODToClip(TOD tod)
        {
            Debug.Log($"Frames1: {tod.Header.NumberOfFrames}, Frames2: {tod.Frames.Length}");

            // scan the packets and store transformations and their frames with the animation objects
            var animObjTransforms = new Dictionary<AnimationObject, List<FrameTransformation>>(_objectTable.Count);
            for (int frameNo = 0; frameNo < tod.Frames.Length; frameNo++)
            {
                var frame = tod.Frames[frameNo];
                foreach (var packet in frame.Packets)
                {
                    if (packet.Data is TODCoordinatePacketData coordPacket)
                    {
                        var animObj = _objectTable[packet.ObjectID];
                        var frameTransformation = new FrameTransformation
                        {
                            CoordPacket = coordPacket,
                            FrameNumber = frameNo
                        };
                        addToAnimObjectTransformsMap(animObjTransforms, animObj, frameTransformation);
                    }
                }
            }

            var frameTime = tod.Header.Resolution / 60f;
            AnimationClip clip = new AnimationClip {frameRate = 1f / frameTime};
            foreach (AnimationObject animObj in _objectTable.Values)
            {
                if (!animObjTransforms.ContainsKey(animObj)) continue;

                var transforms = animObjTransforms[animObj];
                var path = animObj.GetPath();
                Dictionary<string, List<Keyframe>> curveMapping = new Dictionary<string, List<Keyframe>>
                {
                    ["localPosition.x"] = new List<Keyframe>(),
                    ["localPosition.y"] = new List<Keyframe>(),
                    ["localPosition.z"] = new List<Keyframe>(),
                    ["localRotation.x"] = new List<Keyframe>(),
                    ["localRotation.y"] = new List<Keyframe>(),
                    ["localRotation.z"] = new List<Keyframe>(),
                    ["localRotation.w"] = new List<Keyframe>(),
                    ["localScale.x"] = new List<Keyframe>(),
                    ["localScale.y"] = new List<Keyframe>(),
                    ["localScale.z"] = new List<Keyframe>()
                };
                foreach (var frameTransform in transforms)
                {
                    var result = frameTransform.PerformTransformation(frameTime, animObj.Transform);
                    curveMapping["localPosition.x"].Add(result.Keyframes[0]);
                    curveMapping["localPosition.y"].Add(result.Keyframes[1]);
                    curveMapping["localPosition.z"].Add(result.Keyframes[2]);
                    curveMapping["localRotation.x"].Add(result.Keyframes[3]);
                    curveMapping["localRotation.y"].Add(result.Keyframes[4]);
                    curveMapping["localRotation.z"].Add(result.Keyframes[5]);
                    curveMapping["localRotation.w"].Add(result.Keyframes[6]);
                    curveMapping["localScale.x"].Add(result.Keyframes[7]);
                    curveMapping["localScale.y"].Add(result.Keyframes[8]);
                    curveMapping["localScale.z"].Add(result.Keyframes[9]);
                }

                foreach (var kv in curveMapping)
                {
                    var propertyName = kv.Key;
                    var keyframes = kv.Value;

                    // add the first frame at the end, to ensure good looping
                    var firstFrame = keyframes[0];
                    keyframes.Add(new Keyframe(tod.Frames.Length * frameTime, firstFrame.value));
                    var curve = new AnimationCurve(keyframes.ToArray());

                    for (int i = 0; i < keyframes.Count; i++)
                    {
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    }

                    AnimationUtility.SetEditorCurve(clip,
                        new EditorCurveBinding {path = path, propertyName = propertyName, type = typeof(Transform)},
                        curve);
                }
            }

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            clip.EnsureQuaternionContinuity();

            return clip;
        }

        protected void addToAnimObjectTransformsMap(Dictionary<AnimationObject, List<FrameTransformation>> map,
            AnimationObject animObject,
            FrameTransformation transformation)
        {
            if (!map.ContainsKey(animObject))
            {
                map[animObject] = new List<FrameTransformation>();
            }

            map[animObject].Add(transformation);
        }
    }
}
