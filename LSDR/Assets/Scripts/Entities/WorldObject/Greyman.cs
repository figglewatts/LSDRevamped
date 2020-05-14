using System;
using System.Collections.Generic;
using System.Linq;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.IO;
using LSDR.IO.ResourceHandlers;
using LSDR.UI;
using Torii.Resource;
using Torii.UI;
using Torii.Util;
using UnityEngine;

namespace LSDR.Entities.WorldObject
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Greyman : MonoBehaviour
    {
        public Material GreymanMaterial;
        public float MoveSpeed = 0.5f;
        public float FlashDistance = 4;
        public DreamSystem DreamSystem;
        
        public const int HAPPINESS_PENALTY = -2;

        private bool _playerEncountered = false;
        private MOM _greymanMom;
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        
        private const string PATH_TO_GREYMAN_MOM = "original/ETC/SYMSPY.MOM";
        
        public void Awake()
        {
            ResourceManager.RegisterHandler(new MOMHandler());
            
            _greymanMom =
                ResourceManager.Load<MOM>(PathUtil.Combine(Application.streamingAssetsPath, PATH_TO_GREYMAN_MOM));

            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = GreymanMaterial;
            
            populateMesh(_greymanMom);
        }

        public void Update()
        {
            if (_playerEncountered) return;
            
            var t = transform;
            t.position += t.forward * (MoveSpeed * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(t.position, DreamSystem.Player.position);

            if (distanceToPlayer < FlashDistance)
            {
                _playerEncountered = true;
                playerEncountered();
            }
        }
        
        private void playerEncountered()
        {
            DreamSystem.CurrentSequence.UpperModifier += HAPPINESS_PENALTY;
            ToriiFader.Instance.FadeIn(Color.white, 0.1F, () =>
            {
                _meshRenderer.enabled = false;
                ToriiFader.Instance.FadeOut(Color.white, 3F, () =>
                {
                    if (gameObject != null) Destroy(gameObject);
                });
            });
        }

        private void populateMesh(MOM mom)
        {
            Mesh m = LibLSDUnity.CreateMeshesFromTMD(mom.TMD).First();
            rotateUpright(m);
            _meshFilter.sharedMesh = m;
        }

        private void rotateUpright(Mesh m)
        {
            List<Vector3> verts = new List<Vector3>();
            m.GetVertices(verts);

            Quaternion rot = Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(90, Vector3.right);
            
            for (int i = 0; i < verts.Count; i++)
            {
                verts[i] = rot * verts[i];
            }
            
            m.SetVertices(verts);
            m.RecalculateBounds();
        }
    }
}
