using System.Collections.Generic;
using UnityEngine;
using libLSD.Formats;
using libLSD.Types;
using Visual;

namespace IO
{
    public static class LibLSDUnity
    {
        public struct TimData
        {
            public IColor[,] Colors;
            public int Width;
            public int Height;
        }
        
        public static List<Mesh> CreateMeshesFromTMD(TMD tmd)
        {
            List<Mesh> meshList = new List<Mesh>();

            foreach (var obj in tmd.ObjectTable)
            {
                Mesh m = MeshFromTMDObject(obj);
            }
            
            return meshList;
        }

        public static Mesh MeshFromTMDObject(TMDObject obj)
        {   
            Mesh result = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color32> colors = new List<Color32>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            List<int> alphaBlendIndices = new List<int>();

            foreach (var prim in obj.Primitives)
            {
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON) continue;

                List<int> indicesList = (prim.Options & TMDPrimitivePacket.OptionsFlags.AlphaBlended) != 0
                    ? alphaBlendIndices
                    : indices;

                IPrimitivePacket primitivePacket = prim.PacketData;
                ITexturedPrimitivePacket texturedPrimitivePacket = prim.PacketData as ITexturedPrimitivePacket;
                IColoredPrimitivePacket coloredPrimitivePacket = prim.PacketData as IColoredPrimitivePacket;
                ILitPrimitivePacket litPrimitivePacket = prim.PacketData as ILitPrimitivePacket;

                List<int> polyIndices = new List<int>();
                int[] packetIndices = new int[primitivePacket.Vertices.Length];
                for (int i = 0; i < primitivePacket.Vertices.Length; i++)
                {
                    int vertIndex = primitivePacket.Vertices[i];
                    packetIndices[i] = verts.Count;

                    Vec3 vertPos = obj.Vertices[vertIndex];
                    Vector3 vec3VertPos = new Vector3(vertPos.X, -vertPos.Y, vertPos.Z) / 2048f;
                    Color32 vertCol = Color.white;
                    Vector3 vertNorm = Vector3.zero;
                    Vector2 vertUV = Vector2.one;
                    
                    // handle packet colour
                    if (coloredPrimitivePacket != null)
                    {
                        Vec3 packetVertCol =
                            coloredPrimitivePacket.Colors[coloredPrimitivePacket.Colors.Length > 1 ? i : 0];
                        vertCol = new Color(packetVertCol.X, packetVertCol.Y, packetVertCol.Z);
                        if (vertCol.r > 0 && vertCol.g > 0 && vertCol.b > 0 
                            && (prim.Options & TMDPrimitivePacket.OptionsFlags.Textured) == 0
                            && (prim.Options & TMDPrimitivePacket.OptionsFlags.AlphaBlended) != 0)
                        {
                            vertCol.a = 127;
                        }
                    }
                    
                    // handle packet normals
                    if (litPrimitivePacket != null)
                    {
                        TMDNormal packetVertNorm =
                            obj.Normals[litPrimitivePacket.Normals[litPrimitivePacket.Normals.Length > 1 ? i : 0]];
                        vertNorm = new Vector3(packetVertNorm.X, packetVertNorm.Y, packetVertNorm.Z);
                    }
                    
                    // handle packet UVs
                    if (texturedPrimitivePacket != null)
                    {
                        int texPage = texturedPrimitivePacket.Texture.TexturePageNumber;

                        int texPageXPos = ((texPage % 16) * 128) - 640;
                        int texPageYPos = texPage < 16 ? 256 : 0;

                        int uvIndex = i * 2;
                        int vramXPos = texPageXPos + texturedPrimitivePacket.UVs[uvIndex];
                        int vramYPos = texPageYPos + (256 - texturedPrimitivePacket.UVs[uvIndex + 1]);

                        float uCoord = vramXPos / (float)PsxVram.VRAM_WIDTH;
                        float vCoord = vramYPos / (float)PsxVram.VRAM_HEIGHT;
                        
                        vertUV = new Vector2(uCoord, vCoord);
                        
                        // check for overlapping UVs and fix them slightly
                        foreach (var uv in uvs)
                        {
                            if (uv.Equals(vertUV))
                            {
                                vertUV += new Vector2(0.0001f, 0.0001f);    
                            }
                        }
                    }

                    verts.Add(vec3VertPos);
                    normals.Add(vertNorm);
                    colors.Add(vertCol);
                    uvs.Add(vertUV);
                }

                bool isQuad = (prim.Options & TMDPrimitivePacket.OptionsFlags.Quad) != 0;
                
                polyIndices.Add(packetIndices[0]);
                polyIndices.Add(packetIndices[1]);
                polyIndices.Add(packetIndices[2]);

                if (isQuad)
                {
                    polyIndices.Add(packetIndices[2]);
                    polyIndices.Add(packetIndices[1]);
                    polyIndices.Add(packetIndices[3]);
                }

                if ((prim.Flags & TMDPrimitivePacket.PrimitiveFlags.DoubleSided) != 0)
                {
                    polyIndices.Add(packetIndices[1]);
                    polyIndices.Add(packetIndices[0]);
                    polyIndices.Add(packetIndices[2]);

                    if (isQuad)
                    {
                        polyIndices.Add(packetIndices[1]);
                        polyIndices.Add(packetIndices[2]);
                        polyIndices.Add(packetIndices[3]);
                    }
                }
                
                indicesList.AddRange(polyIndices);
            }

            result.vertices = verts.ToArray();
            result.normals = normals.ToArray();
            result.colors32 = colors.ToArray();
            result.uv = uvs.ToArray();

            // regular mesh
            if (indices.Count >= 3)
            {
                result.SetTriangles(indices, 0, false, 0);
            }

            // alpha blended mesh
            if (alphaBlendIndices.Count >= 3)
            {
                result.subMeshCount = 2;
                result.SetTriangles(alphaBlendIndices, 1, false, 0);
            }

            return result;
        }

        public static GameObject CreateLBDTileMap(LBD lbd)
        {
            GameObject lbdTilemap = new GameObject("LBD TileMap");
            List<CombineInstance> meshesCreated = new List<CombineInstance>();

            int tileNo = 0;
            foreach (LBDTile tile in lbd.TileLayout)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileWidth;

                if (tile.DrawTile)
                {
                    GameObject lbdTile = createLBDTile(tile, lbd.ExtraTiles, x, y, lbd.Tiles, meshesCreated);
                    lbdTile.transform.SetParent(lbdTilemap.transform);
                }

                tileNo++;
            }
            
            // combine into mesh
            Mesh combined = new Mesh();
            combined.CombineMeshes(meshesCreated.ToArray(), true);
            MeshCollider mc = lbdTilemap.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;

            LBDSlab slab = lbdTilemap.AddComponent<LBDSlab>();
            slab.MeshFog = lbdTilemap.GetComponentsInChildren<MeshFog>();
            slab.CullMesh = lbdTilemap.GetComponentsInChildren<CullMeshOnDistance>();
            slab.MeshRenderers = lbdTilemap.GetComponentsInChildren<MeshRenderer>();

            return lbdTilemap;
        }

        private static GameObject createLBDTile(LBDTile tile, LBDTile[] extraTiles, int x, int y, TMD tilesTmd, 
            List<CombineInstance> meshesCreated)
        {
            GameObject lbdTile = createSingleLBDTile(tile, x, y, tilesTmd, meshesCreated);

            LBDTile curTile = tile;
            int i = 0;
            while (curTile.ExtraTileIndex >= 0 && i <= 1)
            {
                LBDTile extraTile = extraTiles[curTile.ExtraTileIndex];
                GameObject extraTileObj = createSingleLBDTile(extraTile, x, y, tilesTmd, meshesCreated);
                extraTileObj.transform.SetParent(lbdTile.transform, true);
                curTile = extraTile;
                i++;
            }

            return lbdTile;
        }

        private static GameObject createSingleLBDTile(LBDTile tile, int x, int y, TMD tilesTmd, 
            List<CombineInstance> meshesCreated)
        {
            GameObject lbdTile = new GameObject($"Tile {tile.TileType}");
            MeshFilter mf = lbdTile.AddComponent<MeshFilter>();
            MeshRenderer mr = lbdTile.AddComponent<MeshRenderer>();
            lbdTile.AddComponent<CullMeshOnDistance>();
            lbdTile.AddComponent<MeshFog>();
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            Mesh tileMesh = MeshFromTMDObject(tileObj);
            mf.mesh = tileMesh;
            mr.sharedMaterials = new[] {PsxVram.VramMaterial, PsxVram.VramAlphaBlendMaterial};

            switch (tile.TileDirection)
            {
                case LBDTile.TileDirections.Deg90:
                {
                    lbdTile.transform.Rotate(Vector3.up, 90);
                    break;
                }
                case LBDTile.TileDirections.Deg180:
                {
                    lbdTile.transform.Rotate(Vector3.up, 180);
                    break;
                }
                case LBDTile.TileDirections.Deg270:
                {
                    lbdTile.transform.Rotate(Vector3.up, 270);
                    break;
                }
            }
            
            lbdTile.transform.position = new Vector3(x, -tile.TileHeight, y);

            var localToWorldMatrix = lbdTile.transform.localToWorldMatrix;
            CombineInstance combine = new CombineInstance()
            {
                mesh = tileMesh,
                transform = localToWorldMatrix,
                subMeshIndex = 0
            };
            meshesCreated.Add(combine);

            // if tile has transparent part
            if (tileMesh.subMeshCount > 1)
            {
                CombineInstance combineTrans = new CombineInstance()
                {
                    mesh = tileMesh,
                    transform = localToWorldMatrix,
                    subMeshIndex = 1
                };
                meshesCreated.Add(combineTrans); 
            }

            return lbdTile;
        }

        public static Texture2D GetTextureFromTIX(TIX tix)
        {
            Texture2D tex = new Texture2D(PsxVram.VRAM_WIDTH, PsxVram.VRAM_HEIGHT, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            Color[] fill = new Color[PsxVram.VRAM_WIDTH * PsxVram.VRAM_HEIGHT];
            for (int i = 0; i < fill.Length; i++)
            {
                fill[i] = new Color(1, 1, 1, 1);
            }
            tex.SetPixels(fill);
            tex.Apply();
            
            foreach (var chunk in tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    var image = GetImageDataFromTIM(tim);
                    int actualXPos = (tim.PixelData.XPosition - 320) * 2;
                    int actualYPos = 512 - tim.PixelData.YPosition - image.Height;

                    Color[] td = TimDataToColors(image);
                    
                    tex.SetPixels(actualXPos, actualYPos, image.Width, image.Height, TimDataToColors(image));
                    tex.Apply();
                }
            }
            
            return tex;
        }

        public static Texture2D GetTextureFromTIM(TIM tim, bool flip = true)
        {
            TimData data = GetImageDataFromTIM(tim);
            Texture2D tex = new Texture2D(data.Width, data.Height, TextureFormat.ARGB32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Point;
            
            tex.SetPixels(TimDataToColors(data, flip));

            tex.Apply();

            return tex;
        }

        public static TimData GetImageDataFromTIM(TIM tim)
        {
            TimData data;
            data.Colors = tim.GetImage();
            data.Width = data.Colors.GetLength(1);
            data.Height = data.Colors.GetLength(0);
            return data;
        }

        public static Color[] TimDataToColors(TimData data, bool flip = true)
        {
            Color[] imageData = new Color[data.Colors.Length];

            int i = 0;
            if (flip)
            {
                for (int y = data.Height - 1; y >= 0; y--)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        IColor col = data.Colors[y, x];
                        imageData[i] = new Color(col.Red, col.Green,
                            col.Blue, col.Alpha);
                        i++;
                    }
                }
            }
            else
            {
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        IColor col = data.Colors[y, x];
                        imageData[i] = new Color(col.Red, col.Green,
                            col.Blue, col.Alpha);
                        i++;
                    }
                }
            }

            return imageData;
        }
    }
}
