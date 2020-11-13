using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Formats.Packets;
using UnityEngine;

namespace LSDR.Entities.Original
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class TODAnimator : MonoBehaviour
    {
        public bool Playing = false;

        public int CurrentFrame { get; set; }

        public const double TICK = 1d / 60;

        private double time = 0;
        private TODAnimation _currentAnimation;
        private readonly Dictionary<int, MeshFilter> _objectTable = new Dictionary<int, MeshFilter>();

        private bool _shouldDestroyLastObjTable = false;
        private List<GameObject> _lastObjTable = new List<GameObject>();

        public void Play(TODAnimation anim)
        {
            _currentAnimation = anim;
            CurrentFrame = 0;
            time = 0;
            Playing = true;
            resetObjectTable();
            processFrame(0);
        }

        public void SetAnimation(TODAnimation anim)
        {
            _currentAnimation = anim;
            time = 0;
            Playing = false;
            FirstFrame(anim);
        }

        public void FirstFrame(TODAnimation anim)
        {
            _currentAnimation = anim;
            resetObjectTable();
            processFrame(0);
            Playing = false;
            time = 0;
        }

        public void Resume() { Playing = true; }

        public void Pause() { Playing = false; }

        public void Stop()
        {
            _currentAnimation = null;
            CurrentFrame = 0;
            time = 0;
            Playing = false;
            resetObjectTable();
        }

        public void Start() { CurrentFrame = 0; }

        public void Update()
        {
            if (_currentAnimation == null || !Playing || !Application.isPlaying) return;

            double tickRate = _currentAnimation.Tod.Header.Resolution * TICK;

            time += Time.deltaTime;
            if (time > tickRate)
            {
                time = 0;
                processFrame(CurrentFrame);
                CurrentFrame++;
            }
        }

        private void resetObjectTable()
        {
            foreach (var obj in _objectTable)
            {
                // check to see if we're playing to destroy - as we may be running in the editor (in the level editor)
                if (Application.isPlaying)
                {
                    _lastObjTable.Add(obj.Value.gameObject);
                }
                else
                {
                    DestroyImmediate(obj.Value.gameObject);
                }
            }

            if (Application.isPlaying)
            {
                _shouldDestroyLastObjTable = true;
            }

            _objectTable.Clear();
            var rootObj = new GameObject("Animation Root Object");
            rootObj.transform.SetParent(transform, false);
            _objectTable[0] = rootObj.AddComponent<MeshFilter>();
        }

        private void cleanupLastObjectTable()
        {
            foreach (var obj in _lastObjTable)
            {
                // check to see if we're playing to destroy - as we may be running in the editor (in the level editor)
                if (Application.isPlaying)
                {
                    Destroy(obj);
                }
            }

            _lastObjTable.Clear();
            _shouldDestroyLastObjTable = false;
        }

        private void createObjectMapping(int objectId)
        {
            if (_objectTable.ContainsKey(objectId)) return;

            GameObject obj = new GameObject($"Animation Object {objectId}");
            obj.transform.SetParent(transform, false);
            obj.AddComponent<MeshRenderer>().sharedMaterial = _currentAnimation.Material;
            MeshFilter mf = obj.AddComponent<MeshFilter>();
            _objectTable[objectId] = mf;
        }

        private void processFrame(int frameNumber)
        {
            if (frameNumber >= _currentAnimation.Tod.Header.NumberOfFrames - 1)
            {
                frameNumber = 0;
                CurrentFrame = 0;
            }

            if (_currentAnimation.Tod.Frames.Length <= 0) return;

            if (_shouldDestroyLastObjTable)
            {
                cleanupLastObjectTable();
            }

            TODFrame frame = _currentAnimation.Tod.Frames[frameNumber];

            if (frame.Packets == null)
                return;

            foreach (var packet in frame.Packets)
            {
                if (packet.Data is TODObjectControlPacketData)
                {
                    handleObjectControlPacket(packet);
                }
                else if (packet.Data is TODObjectIDPacketData)
                {
                    handleObjectIDPacket(packet);
                }
                else if (packet.Data is TODCoordinatePacketData)
                {
                    handleCoordinatePacket(packet);
                }
            }
        }

        private void handleObjectControlPacket(TODPacket packet)
        {
            TODObjectControlPacketData packetData = packet.Data as TODObjectControlPacketData;
            if (packetData.ObjectControl == TODObjectControlPacketData.ObjectControlType.Create)
            {
                createObjectMapping(packet.ObjectID);
            }
            else if (packetData.ObjectControl == TODObjectControlPacketData.ObjectControlType.Kill)
            {
                Destroy(_objectTable[packet.ObjectID]);
            }
        }

        private void handleObjectIDPacket(TODPacket packet)
        {
            TODObjectIDPacketData packetData = packet.Data as TODObjectIDPacketData;
            if (packet.PacketType == TODPacket.PacketTypes.TMDDataID)
            {
                // create mapping in object table
                var obj = _objectTable[packet.ObjectID];
                obj.sharedMesh = _currentAnimation.ObjectTable[packetData.ObjectID - 1];
            }
            else if (packet.PacketType == TODPacket.PacketTypes.ParentObjectID)
            {
                // set object parent
                Transform parentTransform = _objectTable[packetData.ObjectID].transform;
                _objectTable[packet.ObjectID].transform.SetParent(parentTransform, false);
            }
        }

        private void handleCoordinatePacket(TODPacket packet)
        {
            TODCoordinatePacketData packetData = packet.Data as TODCoordinatePacketData;

            Transform objTransform = _objectTable[packet.ObjectID].transform;
            if (packetData.HasScale)
            {
                if (packetData.MatrixType == TODPacketData.PacketDataType.Absolute)
                {
                    objTransform.localScale = new Vector3(packetData.ScaleX / 4096f, packetData.ScaleY / 4096f,
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
                    Vector3 pos = new Vector3(packetData.TransX, -packetData.TransY, packetData.TransZ) / 2048f;
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
