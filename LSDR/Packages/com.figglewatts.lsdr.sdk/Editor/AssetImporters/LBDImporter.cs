using System.Collections.Generic;
using System.IO;
using libLSD.Formats;
using LSDR.SDK.IO;
using UnityEditor;
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

            // clean up unused assets
            foreach (var lbdMesh in lbdMeshes)
            {
                DestroyImmediate(lbdMesh);
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
