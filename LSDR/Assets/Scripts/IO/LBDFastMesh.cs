using System;
using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.IO.ResourceHandlers;
using Torii.Graphics;
using Torii.Resource;
using Torii.UnityEditor;
using Torii.Util;
using UnityEngine;

namespace LSDR.IO
{
    public class LBDFastMesh : MonoBehaviour
    {
        public Material LBDDiffuse;
        public Material LBDAlpha;

        public GameObject[] LBDColliders;

        [BrowseFileSystem(BrowseType.Directory, name: "LBD")]
        public string LBDDirectoryPath;

        [BrowseFileSystem(BrowseType.File, new []{"TIX files", "TIX"}, "TIX")]
        public string TIXFilePath;
        
        public LegacyTileMode Mode;
        public int LBDWidth = 1;

        private Dictionary<TMDObject, FastMesh> _tileCache;
        private Material[] _materials;
        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        public void Awake()
        {
            _tileCache = new Dictionary<TMDObject, FastMesh>(new TMDObjectEqualityComparer());
            _materials = new[] {LBDDiffuse, LBDAlpha};
            
            ResourceManager.RegisterHandler(new LBDHandler());
            ResourceManager.RegisterHandler(new TIXHandler());
        }

        public void Start()
        {
            LoadLBD(LBDDirectoryPath);
            
            TIX tix = ResourceManager.Load<TIX>(PathUtil.Combine(Application.streamingAssetsPath, TIXFilePath));
            UseTIX(tix);
        }

        public void LoadLBD(string lbdFolder)
        {
            GameObject lbdColliders = new GameObject("LBD Colliders");

            string[] lbdFiles = Directory.GetFiles(PathUtil.Combine(Application.streamingAssetsPath, lbdFolder),
                "*.LBD", SearchOption.AllDirectories);
            LBDColliders = new GameObject[lbdFiles.Length];
            for (int i = 0; i < lbdFiles.Length; i++)
            {
                string lbdFile = lbdFiles[i];
                
                Vector3 posOffset = Vector3.zero;
                if (Mode == LegacyTileMode.Horizontal)
                {
                    int xPos = i % LBDWidth;
                    int yPos = i / LBDWidth;
                    int xMod = 0;
                    if (yPos % 2 == 1)
                    {
                        xMod = 10;
                    }
                    posOffset = new Vector3(xPos * 20 - xMod, 0, yPos * 20);
                }
                
                LBD lbd = ResourceManager.Load<LBD>(lbdFile);
                GameObject lbdCollider = CreateLBDTileMap(lbd, posOffset);
                lbdCollider.transform.SetParent(lbdColliders.transform);
                LBDColliders[i] = lbdCollider;
            }
        }
        
        public void UseTIX(TIX tix)
        {
            var tex = LibLSDUnity.GetTextureFromTIX(tix);
            LBDDiffuse.SetTexture(_mainTex, tex);
            LBDAlpha.SetTexture(_mainTex, tex);
        }

        public void CreateLBDTileMap(LBD lbd)
        {
            CreateLBDTileMap(lbd, Vector3.zero);
        }
        
        public GameObject CreateLBDTileMap(LBD lbd, Vector3 posOffset)
        {
            List<CombineInstance> colliderMeshes = new List<CombineInstance>();
            
            int tileNo = 0;
            for (int i = 0; i < lbd.TileLayout.Length; i++)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileWidth;
                LBDTile tile = lbd.TileLayout[x, y];
                
                // create an LBD tile if we should draw it
                if (tile.DrawTile)
                {
                    FastMesh mesh = createTileMesh(tile, lbd.Tiles);
                    var matrix = mesh.AddInstance(new Vector3(x, -tile.TileHeight, y) + posOffset, tileRotation(tile));
                    colliderMeshes.Add(new CombineInstance
                    {
                        mesh = mesh.Mesh,
                        transform = matrix,
                        subMeshIndex = 0
                    });
                    if (mesh.Mesh.subMeshCount > 1)
                    {
                        colliderMeshes.Add(new CombineInstance
                        {
                            mesh = mesh.Mesh,
                            transform = matrix,
                            subMeshIndex = 1
                        });
                    }
                    
                    // now do extra tiles
                    LBDTile curTile = tile;
                    int j = 0;
                    while (curTile.ExtraTileIndex >= 0 && j <= 1)
                    {
                        LBDTile extraTile = lbd.ExtraTiles[curTile.ExtraTileIndex];
                        FastMesh extraTileMesh = createTileMesh(extraTile, lbd.Tiles);
                        var extraMatrix = extraTileMesh.AddInstance(new Vector3(x, -extraTile.TileHeight, y) + posOffset, tileRotation(extraTile));
                        colliderMeshes.Add(new CombineInstance
                        {
                            mesh = extraTileMesh.Mesh,
                            transform = extraMatrix,
                            subMeshIndex = 0
                        });
                        if (extraTileMesh.Mesh.subMeshCount > 1)
                        {
                            colliderMeshes.Add(new CombineInstance
                            {
                                mesh = extraTileMesh.Mesh,
                                transform = extraMatrix,
                                subMeshIndex = 1
                            });
                        }
                        curTile = extraTile;
                        i++;
                    }
                }

                tileNo++;
            }

            return createLBDCollider(colliderMeshes, posOffset);
        }

        private GameObject createLBDCollider(List<CombineInstance> combineInstances, Vector3 position)
        {
            GameObject colliderObject = new GameObject("LBD Collider");
            Mesh combined = new Mesh();
            combined.CombineMeshes(combineInstances.ToArray(), true);
            MeshCollider mc = colliderObject.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;
            colliderObject.tag = "Linkable";
            return colliderObject;
        }

        private Quaternion tileRotation(LBDTile tile)
        {
            switch (tile.TileDirection)
            {
                case LBDTile.TileDirections.Deg90:
                {
                    return Quaternion.AngleAxis(90, Vector3.up);
                }

                case LBDTile.TileDirections.Deg180:
                {
                    return Quaternion.AngleAxis(180, Vector3.up);
                }

                case LBDTile.TileDirections.Deg270:
                {
                    return Quaternion.AngleAxis(270, Vector3.up);
                }

                default:
                {
                    return Quaternion.identity;
                }
            }
        }

        private FastMesh createTileMesh(LBDTile tile, TMD tilesTmd)
        {
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            if (_tileCache.ContainsKey(tileObj))
            {
                return _tileCache[tileObj];
            }

            Mesh m = LibLSDUnity.MeshFromTMDObject(tileObj);
            FastMesh fm = new FastMesh(m, _materials);
            _tileCache[tileObj] = fm;
            return fm;
        }

        public void Update()
        {
            foreach (var fm in _tileCache.Values)
            {
                fm.Draw();
            }
        }
    }
}
