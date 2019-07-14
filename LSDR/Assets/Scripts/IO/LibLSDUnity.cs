using System.Collections.Generic;
using UnityEngine;
using libLSD.Formats;
using libLSD.Types;
using LSDR.Visual;

namespace LSDR.IO
{
    /// <summary>
    /// Contains numerous utility methods for converting data in original game format to formats Unity likes.
    /// Utilises LibLSD to do this.
    /// </summary>
    public static class LibLSDUnity
    {
        /// <summary>
        /// A struct to store data from a PS1 TIM file.
        /// </summary>
        public struct TimData
        {
            public IColor[,] Colors;
            public int Width;
            public int Height;
        }
        
        /// <summary>
        /// Create a list of meshes based on the contents of a TMD model file.
        /// </summary>
        /// <param name="tmd">The loaded TMD to use.</param>
        /// <returns>The loaded list of meshes.</returns>
        public static List<Mesh> CreateMeshesFromTMD(TMD tmd)
        {
            List<Mesh> meshList = new List<Mesh>();

            foreach (var obj in tmd.ObjectTable)
            {
                Mesh m = MeshFromTMDObject(obj);
                meshList.Add(m);
            }
            
            return meshList;
        }

        /// <summary>
        /// Create a mesh from an object stored inside a TMD model file.
        /// </summary>
        /// <param name="obj">The TMD object to create a mesh from.</param>
        /// <returns>The Mesh created from the object.</returns>
        public static Mesh MeshFromTMDObject(TMDObject obj)
        {   
            // create the mesh, and lists of vertices, normals, colors, uvs, and indices
            Mesh result = new Mesh();
            List<Vector3> verts = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color32> colors = new List<Color32>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> indices = new List<int>();
            List<int> alphaBlendIndices = new List<int>(); // alpha blended polygons are stored in a submesh

            // TMD objects are built from 'primitives'
            foreach (var prim in obj.Primitives)
            {
                // currently only polygon primitives are supported
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON) continue;

                // check which index list to use based on whether this primitive is alpha blended or not
                List<int> indicesList = (prim.Options & TMDPrimitivePacket.OptionsFlags.AlphaBlended) != 0
                    ? alphaBlendIndices
                    : indices;

                // get interfaces for different packet types from LibLSD
                IPrimitivePacket primitivePacket = prim.PacketData;
                ITexturedPrimitivePacket texturedPrimitivePacket = prim.PacketData as ITexturedPrimitivePacket;
                IColoredPrimitivePacket coloredPrimitivePacket = prim.PacketData as IColoredPrimitivePacket;
                ILitPrimitivePacket litPrimitivePacket = prim.PacketData as ILitPrimitivePacket;

                // for each vertex in the primitive
                List<int> polyIndices = new List<int>();
                int[] packetIndices = new int[primitivePacket.Vertices.Length];
                for (int i = 0; i < primitivePacket.Vertices.Length; i++)
                {
                    // get its index into the vertices array
                    int vertIndex = primitivePacket.Vertices[i];
                    packetIndices[i] = verts.Count;

                    // create variables for each of the vertex types
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
                        // calculate which texture page we're on
                        int texPage = texturedPrimitivePacket.Texture.TexturePageNumber;

                        int texPageXPos = ((texPage % 16) * 128) - 640;
                        int texPageYPos = texPage < 16 ? 256 : 0;

                        // derive UV coords from the texture page
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

                    // add all computed aspects of vertex to lists
                    verts.Add(vec3VertPos);
                    normals.Add(vertNorm);
                    colors.Add(vertCol);
                    uvs.Add(vertUV);
                }

                // we want to add extra indices if this primitive is a quad (to triangulate)
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

                // if this primitive is double sided we want to add more vertices with opposite winding order
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
                
                // add the indices to the list
                indicesList.AddRange(polyIndices);
            }

            // set the mesh arrays
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

        /// <summary>
        /// Create an LBD tilemap GameObject from an LSD level tileset.
        /// </summary>
        /// <param name="lbd">The loaded LBD file.</param>
        /// <returns>A GameObject containing loaded meshes for all tiles in their layout.</returns>
        public static GameObject CreateLBDTileMap(LBD lbd)
        {
            GameObject lbdTilemap = new GameObject("LBD TileMap");
            List<CombineInstance> meshesCreated = new List<CombineInstance>(); // we're combining meshes into a collision mesh

            // for each tile in the tilemap
            int tileNo = 0;
            foreach (LBDTile tile in lbd.TileLayout)
            {
                int x = tileNo / lbd.Header.TileWidth;
                int y = tileNo % lbd.Header.TileWidth;

                // create an LBD tile if we should draw it
                if (tile.DrawTile)
                {
                    GameObject lbdTile = createLBDTile(tile, lbd.ExtraTiles, x, y, lbd.Tiles, meshesCreated);
                    lbdTile.transform.SetParent(lbdTilemap.transform);
                }

                tileNo++;
            }
            
            // combine all tiles into mesh for efficient collision
            Mesh combined = new Mesh();
            combined.CombineMeshes(meshesCreated.ToArray(), true);
            MeshCollider mc = lbdTilemap.AddComponent<MeshCollider>();
            mc.sharedMesh = combined;

            // add LBDSlab component for controlling fog/culling
            LBDSlab slab = lbdTilemap.AddComponent<LBDSlab>();
            slab.MeshFog = lbdTilemap.GetComponentsInChildren<MeshFog>();
            slab.CullMesh = lbdTilemap.GetComponentsInChildren<CullMeshOnDistance>();
            slab.MeshRenderers = lbdTilemap.GetComponentsInChildren<MeshRenderer>();

            return lbdTilemap;
        }

        // create an LBD tile GameObject
        private static GameObject createLBDTile(LBDTile tile, LBDTile[] extraTiles, int x, int y, TMD tilesTmd, 
            List<CombineInstance> meshesCreated)
        {
            // create the GameObject for the base tile
            GameObject lbdTile = createSingleLBDTile(tile, x, y, tilesTmd, meshesCreated);

            // now see if it has any extra tiles, and create those
            LBDTile curTile = tile;
            int i = 0;
            while (curTile.ExtraTileIndex >= 0 && i <= 1)
            {
                LBDTile extraTile = extraTiles[curTile.ExtraTileIndex];
                GameObject extraTileObj = createSingleLBDTile(extraTile, x, y, tilesTmd, meshesCreated);
                extraTileObj.transform.SetParent(lbdTile.transform, true); // parent them to original tile
                curTile = extraTile;
                i++;
            }

            return lbdTile;
        }

        // create a single LBD tile GameObject (not including extra tiles)
        private static GameObject createSingleLBDTile(LBDTile tile, int x, int y, TMD tilesTmd, 
            List<CombineInstance> meshesCreated)
        {
            // create the GameObject and add/setup necessary components
            GameObject lbdTile = new GameObject($"Tile {tile.TileType}");
            MeshFilter mf = lbdTile.AddComponent<MeshFilter>();
            MeshRenderer mr = lbdTile.AddComponent<MeshRenderer>();
            lbdTile.AddComponent<CullMeshOnDistance>();
            lbdTile.AddComponent<MeshFog>();
            TMDObject tileObj = tilesTmd.ObjectTable[tile.TileType];
            Mesh tileMesh = MeshFromTMDObject(tileObj);
            mf.mesh = tileMesh;
            
            // the renderer needs to use virtual PSX Vram as its materials
            mr.sharedMaterials = new[] {PsxVram.VramMaterial, PsxVram.VramAlphaBlendMaterial};

            // rotate the tile based on its direction
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
            
            // set the tile's height
            lbdTile.transform.position = new Vector3(x, -tile.TileHeight, y);

            // make a CombineInstance for combining all tiles into one mesh later on
            var localToWorldMatrix = lbdTile.transform.localToWorldMatrix;
            CombineInstance combine = new CombineInstance()
            {
                mesh = tileMesh,
                transform = localToWorldMatrix,
                subMeshIndex = 0
            };
            meshesCreated.Add(combine);

            // if tile has transparent part, do the same for the transparent mesh
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

        /// <summary>
        /// Get a Unity Texture2D from a loaded TIX image archive.
        /// </summary>
        /// <param name="tix">The TIX file loaded.</param>
        /// <returns>A Texture2D containing all images inside the TIX archive laid out as if in VRAM.</returns>
        public static Texture2D GetTextureFromTIX(TIX tix)
        {
            // create a texture 2D with the required format
            Texture2D tex = new Texture2D(PsxVram.VRAM_WIDTH, PsxVram.VRAM_HEIGHT, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Point
            };
            
            // fill the texture with a white colour
            Color[] fill = new Color[PsxVram.VRAM_WIDTH * PsxVram.VRAM_HEIGHT];
            for (int i = 0; i < fill.Length; i++)
            {
                fill[i] = new Color(1, 1, 1, 1);
            }
            tex.SetPixels(fill);
            tex.Apply();
            
            // for each image within the archive, load it and paint it on the 'canvas' we created above
            foreach (var chunk in tix.Chunks)
            {
                foreach (var tim in chunk.TIMs)
                {
                    var image = GetImageDataFromTIM(tim);
                    int actualXPos = (tim.PixelData.XPosition - 320) * 2;
                    int actualYPos = 512 - tim.PixelData.YPosition - image.Height;
                    
                    tex.SetPixels(actualXPos, actualYPos, image.Width, image.Height, TimDataToColors(image));
                    tex.Apply();
                }
            }
            
            return tex;
        }

        /// <summary>
        /// Load a PS1 TIM image to a Unity Texture2D.
        /// </summary>
        /// <param name="tim">The loaded TIM image.</param>
        /// <param name="flip">Whether or not we should flip the loaded image.</param>
        /// <returns></returns>
        public static Texture2D GetTextureFromTIM(TIM tim, bool flip = true)
        {
            // get the image data
            TimData data = GetImageDataFromTIM(tim);
            
            // create a texture with the required format
            Texture2D tex = new Texture2D(data.Width, data.Height, TextureFormat.ARGB32, false)
            {
                wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Point
            };

            // set the texture's pixels to the TIM pixels
            tex.SetPixels(TimDataToColors(data, flip));
            tex.Apply();

            return tex;
        }

        /// <summary>
        /// Get the pixels and width/height from a loaded TIM.
        /// </summary>
        /// <param name="tim">The loaded TIM.</param>
        /// <returns>TimData containing the TIM data.</returns>
        public static TimData GetImageDataFromTIM(TIM tim)
        {
            TimData data;
            data.Colors = tim.GetImage();
            data.Width = data.Colors.GetLength(1);
            data.Height = data.Colors.GetLength(0);
            return data;
        }

        /// <summary>
        /// Convert a TimData struct to an array of Colors to be used for setting pixels in a texture.
        /// </summary>
        /// <param name="data">The TimData to use.</param>
        /// <param name="flip">Whether or not to flip the image.</param>
        /// <returns>The Color array corresponding to this TIM data.</returns>
        public static Color[] TimDataToColors(TimData data, bool flip = true)
        {
            // create the array
            Color[] imageData = new Color[data.Colors.Length];

            // iterate through each pixel and create its entry in the array
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
