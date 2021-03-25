using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Formats.Packets;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.AssetImporters
{
    public class MOMHelper
    {
        protected readonly Material _opaque;
        protected readonly Material _transparent;
        protected readonly GameObject _rootObject;
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

        public MOMHelper(GameObject root, Material opaque, Material transparent)
        {
            _rootObject = root;
            root.name = "0";
            _objectTable = new Dictionary<int, AnimationObject> {[0] = new AnimationObject(root.transform)};
            _opaque = opaque;
            _transparent = transparent;
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

        public GameObject MakeMeshObject(Mesh mesh, int id)
        {
            var meshObj = new GameObject($"{id}");
            MeshFilter mf = meshObj.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();

            if (mesh.subMeshCount > 1)
            {
                mr.sharedMaterials = new[]
                {
                    _opaque == null
                        ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                        : _opaque,
                    _transparent == null
                        ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                        : _transparent
                };
            }
            else
            {
                mr.sharedMaterial = _opaque == null
                    ? AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
                    : _opaque;
            }

            return meshObj;
        }

        public void CreateAnimationObjectHierarchy(TODFrame animFirstFrame, List<Mesh> meshes)
        {
            foreach (var packet in animFirstFrame.Packets)
            {
                if (packet.Data is TODObjectControlPacketData objControl)
                {
                    if (objControl.ObjectControl == TODObjectControlPacketData.ObjectControlType.Create)
                    {
                        // create a new object in the transform hierarchy/object table
                        var newObj = new GameObject($"{packet.ObjectID}");
                        CreateObject(packet.ObjectID, newObj.transform);
                    }
                }
                else if (packet.Data is TODObjectIDPacketData objId)
                {
                    if (packet.PacketType == TODPacket.PacketTypes.TMDDataID)
                    {
                        // assign a mesh to an object in the transform hierarchy/object table
                        var meshObj = MakeMeshObject(meshes[objId.ObjectID - 1], objId.ObjectID - 1);
                        var parent = GetObject(packet.ObjectID);
                        meshObj.transform.SetParent(parent);
                    }
                    else if (packet.PacketType == TODPacket.PacketTypes.ParentObjectID)
                    {
                        // parent something in the transform hierarchy to something else in the transform hierarchy
                        SetObjectParent(packet.ObjectID, objId.ObjectID);
                    }
                }
            }
        }

        public void PoseObjectInFirstFrame(TODFrame animFirstFrame)
        {
            foreach (var packet in animFirstFrame.Packets)
            {
                if (packet.Data is TODCoordinatePacketData packetData)
                {
                    var objTransform = GetObject(packet.ObjectID);
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

        public AnimationClip TODToClip(TOD tod)
        {
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
