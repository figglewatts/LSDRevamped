using System.Collections.Generic;
using libLSD.Formats;
using LSDR.SDK;
using Torii.Pooling;
using UnityEngine;

namespace LSDR.IO
{
    [CreateAssetMenu(menuName = "System/LBDReaderSystem")]
    public class LBDReaderSystem : ScriptableObject
    {
        public const int MAX_POSSIBLE_TILES = 54930;
        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

        public Material LBDDiffuse;
        public Material LBDAlpha;

        /// <summary>
        ///     The PrefabPool to use for LBD tiles.
        /// </summary>
        public PrefabPool LBDTilePool;

        private TMDReader _tmdReader;

        public void OnEnable() { _tmdReader = new TMDReader(); }

        public void UseTIX(TIX tix)
        {
            Texture2D tex = LibLSDUnity.GetTextureFromTIX(tix);
            LBDDiffuse.SetTexture(_mainTex, tex);
            LBDAlpha.SetTexture(_mainTex, tex);
        }

        /// <summary>
        ///     Create an LBD tilemap GameObject from an LSD level tileset.
        /// </summary>
        /// <param name="lbd">The loaded LBD file.</param>
        /// <returns>A GameObject containing loaded meshes for all tiles in their layout.</returns>
        public GameObject CreateLBDTileMap(LBD lbd, Dictionary<TMDObject, Mesh> cache)
        {
            GameObject lbdTilemap = new GameObject("LBD TileMap");
            var
                meshesCreated = new List<CombineInstance>(); // we're combining meshes into a collision mesh

            // for each tile in the tilemap
            int tileNo = 0;
            foreach (LBDTile tile in lbd.TileLayout)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileWidth;

                // create an LBD tile if we should draw it
                if (tile.DrawTile)
                {
                    GameObject lbdTile = createLBDTile(tile, lbd.ExtraTiles, x, y, lbd.Tiles, meshesCreated, cache);
                    lbdTile.transform.SetParent(lbdTilemap.transform);
                }

                tileNo++;
            }

            // combine all tiles into mesh for efficient collision
            Mesh combined = new Mesh();
            combined.CombineMeshes(meshesCreated.ToArray(), mergeSubMeshes: true);
            MeshCollider mc = lbdTilemap.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;

            lbdTilemap.tag = "Linkable";

            return lbdTilemap;
        }

        // create an LBD tile GameObject
        private GameObject createLBDTile(LBDTile tile,
            LBDTile[] extraTiles,
            int x,
            int y,
            TMD tilesTmd,
            List<CombineInstance> meshesCreated,
            Dictionary<TMDObject, Mesh> cache)
        {
            // create the GameObject for the base tile
            GameObject lbdTile = createSingleLBDTile(tile, x, y, tilesTmd, meshesCreated, cache);

            // now see if it has any extra tiles, and create those
            LBDTile curTile = tile;
            int i = 0;
            while (curTile.ExtraTileIndex >= 0 && i <= 1)
            {
                LBDTile extraTile = extraTiles[curTile.ExtraTileIndex];
                GameObject extraTileObj = createSingleLBDTile(extraTile, x, y, tilesTmd, meshesCreated, cache);
                extraTileObj.transform.SetParent(lbdTile.transform,
                    worldPositionStays: true); // parent them to original tile
                curTile = extraTile;
                i++;
            }

            return lbdTile;
        }

        // create a single LBD tile GameObject (not including extra tiles)
        private GameObject createSingleLBDTile(LBDTile tile,
            int x,
            int y,
            TMD tilesTmd,
            List<CombineInstance> meshesCreated,
            Dictionary<TMDObject, Mesh> cache)
        {
            // rotate the tile based on its direction
            Quaternion tileRot = Quaternion.identity;
            switch (tile.TileDirection)
            {
                case LBDTile.TileDirections.Deg90:
                {
                    tileRot = Quaternion.AngleAxis(angle: 90, Vector3.up);
                    break;
                }
                case LBDTile.TileDirections.Deg180:
                {
                    tileRot = Quaternion.AngleAxis(angle: 180, Vector3.up);
                    break;
                }
                case LBDTile.TileDirections.Deg270:
                {
                    tileRot = Quaternion.AngleAxis(angle: 270, Vector3.up);
                    break;
                }
            }

            // create the GameObject and add/setup necessary components
            GameObject lbdTile = LBDTilePool.Summon(new Vector3(x, -tile.TileHeight, y), tileRot);
            MeshFilter mf = lbdTile.GetComponent<MeshFilter>();
            MeshRenderer mr = lbdTile.GetComponent<MeshRenderer>();
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            Mesh tileMesh;
            if (cache.ContainsKey(tileObj))
            {
                tileMesh = cache[tileObj];
            }
            else
            {
                tileMesh = LibLSDUnity.MeshFromTMDObject(tileObj);
                cache[tileObj] = tileMesh;
            }

            mf.sharedMesh = tileMesh;

            // the renderer needs to use virtual PSX Vram as its materials
            mr.sharedMaterials = new[] { LBDDiffuse, LBDAlpha };

            // set the tile's height
            lbdTile.transform.position = new Vector3(x, -tile.TileHeight, y);

            // make a CombineInstance for combining all tiles into one mesh later on
            Matrix4x4 localToWorldMatrix = lbdTile.transform.localToWorldMatrix;
            CombineInstance combine = new CombineInstance
            {
                mesh = tileMesh,
                transform = localToWorldMatrix,
                subMeshIndex = 0
            };
            meshesCreated.Add(combine);

            // if tile has transparent part, do the same for the transparent mesh
            if (tileMesh.subMeshCount > 1)
            {
                CombineInstance combineTrans = new CombineInstance
                {
                    mesh = tileMesh,
                    transform = localToWorldMatrix,
                    subMeshIndex = 1
                };
                meshesCreated.Add(combineTrans);
            }

            return lbdTile;
        }
    }
}
