using System;
using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.Dream;
using LSDR.Entities.Original;
using LSDR.SDK;
using LSDR.Visual;
using Torii.Graphics;
using Torii.Resource;
using Torii.Util;
using UnityEngine;

namespace LSDR.IO
{
    [CreateAssetMenu(menuName = "System/LBDFastMeshSystem")]
    public class LBDFastMeshSystem : ScriptableObject
    {
        public TextureSetSystem TextureSetSystem;

        public Material LBDDiffuse;
        public Material LBDAlpha;
        public Shader Classic;
        public Shader ClassicAlpha;
        public Shader Revamped;
        public Shader RevampedAlpha;

        [NonSerialized] public GameObject[] LBDColliders;

        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        private static int num = 0;

        public void LoadLBD(string lbdFolder, LegacyTileMode tileMode, int lbdWidth)
        {
            lbdFolder = PathUtil.Combine(Application.streamingAssetsPath, lbdFolder);

            TextureSetSystem.DeregisterMaterial(LBDDiffuse);
            TextureSetSystem.DeregisterMaterial(LBDAlpha);
            TextureSetSystem.RegisterMaterial(LBDDiffuse,
                TextureSetOptions.GetFromLBDPath(lbdFolder, Classic, Revamped));
            TextureSetSystem.RegisterMaterial(LBDAlpha,
                TextureSetOptions.GetFromLBDPath(lbdFolder, ClassicAlpha, RevampedAlpha));

            GameObject lbdRenderer = new GameObject("LBD Renderer");
            LBDTileMap tileMap = lbdRenderer.AddComponent<LBDTileMap>();
            GameObject lbdColliders = new GameObject("LBD Colliders");

            string[] lbdFiles = Directory.GetFiles(lbdFolder, "*.LBD", SearchOption.AllDirectories);
            LBDColliders = new GameObject[lbdFiles.Length];
            for (int i = 0; i < lbdFiles.Length; i++)
            {
                string lbdFile = lbdFiles[i];

                Vector3 posOffset = Vector3.zero;
                if (tileMode == LegacyTileMode.Horizontal)
                {
                    int xPos = i % lbdWidth;
                    int yPos = i / lbdWidth;
                    int xMod = 0;
                    if (yPos % 2 == 1)
                    {
                        xMod = 10;
                    }

                    posOffset = new Vector3(xPos * 20 - xMod, 0, yPos * 20);
                }

                LBD lbd = ResourceManager.Load<LBD>(lbdFile);
                GameObject lbdCollider = createLBDTileMap(lbd, posOffset, tileMap.TileCache);
                lbdCollider.transform.SetParent(lbdColliders.transform);
                LBDColliders[i] = lbdCollider;
            }

            foreach (var m in tileMap.TileCache.Values)
            {
                m.Submit();
            }
        }

        private void createLBDTileMap(LBD lbd, Dictionary<TMDObject, FastMesh> tileCache)
        {
            createLBDTileMap(lbd, Vector3.zero, tileCache);
        }

        private GameObject createLBDTileMap(LBD lbd, Vector3 posOffset, Dictionary<TMDObject, FastMesh> tileCache)
        {
            List<CombineInstance> colliderMeshes = new List<CombineInstance>();

            int tileNo = 0;
            for (int i = 0; i < lbd.TileLayout.Length; i++)
            {
                int x = tileNo % lbd.Header.TileWidth;
                int y = tileNo / lbd.Header.TileWidth;
                LBDTile tile = lbd.TileLayout[x, y];

                // create an LBD tile if we should draw it
                if (tile.DrawTile)
                {
                    FastMesh mesh = createTileMesh(tile, lbd.Tiles, tileCache);
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
                        FastMesh extraTileMesh = createTileMesh(extraTile, lbd.Tiles, tileCache);
                        var extraMatrix =
                            extraTileMesh.AddInstance(new Vector3(x, -extraTile.TileHeight, y) + posOffset,
                                tileRotation(extraTile));
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
                        j++;
                    }
                }

                tileNo++;
            }

            return createLBDCollider(colliderMeshes, posOffset);
        }

        private GameObject createLBDCollider(List<CombineInstance> combineInstances, Vector3 position)
        {
            GameObject colliderObject = new GameObject($"LBD Collider {num}");
            Mesh combined = new Mesh();
            combined.CombineMeshes(combineInstances.ToArray(), true);
            MeshCollider mc = colliderObject.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;
            colliderObject.tag = "Linkable";
            num++;
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

        private FastMesh createTileMesh(LBDTile tile, TMD tilesTmd, Dictionary<TMDObject, FastMesh> tileCache)
        {
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            if (tileCache.ContainsKey(tileObj))
            {
                return tileCache[tileObj];
            }

            Mesh m = LibLSDUnity.MeshFromTMDObject(tileObj);
            FastMesh fm = new FastMesh(m, new[] {LBDDiffuse, LBDAlpha});
            tileCache[tileObj] = fm;
            return fm;
        }
    }
}
