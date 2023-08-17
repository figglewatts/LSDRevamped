using System.Collections.Generic;
using System.Linq;
using libLSD.Formats;
using libLSD.Formats.Packets;
using libLSD.Types;
using LSDR.SDK.Assets;
using UnityEngine;

namespace LSDR.SDK
{
    /// <summary>
    ///     Contains numerous utility methods for converting data in original game format to formats Unity likes.
    ///     Utilises LibLSD to do this.
    /// </summary>
    public static class LibLSDUnity
    {
        // as it was in PSX datasheets
        public const int VRAM_WIDTH = 2056;
        public const int VRAM_HEIGHT = 512;

        public static List<GameObject> CreateGameObjectsFromMOM(MOM mom, Material mat)
        {
            List<Mesh> meshes = CreateMeshesFromTMD(mom.TMD);
            var meshObjects = new List<GameObject>();
            foreach (Mesh mesh in meshes)
            {
                GameObject meshObj = new GameObject($"mesh {meshObjects.Count}");
                MeshRenderer mr = meshObj.AddComponent<MeshRenderer>();
                MeshFilter mf = meshObj.AddComponent<MeshFilter>();
                mr.sharedMaterial = mat;
                mf.sharedMesh = mesh;
                meshObjects.Add(meshObj);
            }

            return meshObjects;
        }

        /// <summary>
        ///     Create a list of meshes based on the contents of a TMD model file.
        /// </summary>
        /// <param name="tmd">The loaded TMD to use.</param>
        /// <param name="uvMaterialOverrides">Optional - the list of UV material overrides to apply.</param>
        /// <returns>The loaded list of meshes.</returns>
        public static List<Mesh> CreateMeshesFromTMD(TMD tmd, List<UVMaterialOverride> uvMaterialOverrides = null)
        {
            var meshList = new List<Mesh>();

            foreach (TMDObject obj in tmd.ObjectTable)
            {
                Mesh m = MeshFromTMDObject(obj, uvMaterialOverrides);
                meshList.Add(m);
            }

            return meshList;
        }

        /// <summary>
        ///     Create a mesh from an object stored inside a TMD model file.
        /// </summary>
        /// <param name="obj">The TMD object to create a mesh from.</param>
        /// <param name="uvMaterialOverrides">Optional - the list of UV material overrides to apply.</param>
        /// <returns>The Mesh created from the object.</returns>
        public static Mesh MeshFromTMDObject(TMDObject obj, List<UVMaterialOverride> uvMaterialOverrides = null)
        {
            // create the mesh, and lists of vertices, normals, colors, uvs, and indices
            Mesh result = new Mesh();
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var colors = new List<Color32>();
            var uvs = new List<Vector2>();
            var indices = new List<int>();
            var alphaBlendIndices = new List<int>(); // alpha blended polygons are stored in a submesh

            // material overrides are also stored in submeshes
            List<List<int>> materialOverrideIndices = uvMaterialOverrides == null
                ? new List<List<int>>()
                : new List<List<int>>(uvMaterialOverrides.Select(o => new List<int>()));

            // local function used to check if a set of UV coords has a material override
            bool uvHasOverride(Vector2 uv, out int overrideIndex)
            {
                for (int i = 0; i < uvMaterialOverrides.Count; i++)
                {
                    if (uvMaterialOverrides[i].UVRect.Contains(uv))
                    {
                        overrideIndex = i;
                        return true;
                    }
                }
                overrideIndex = -1;
                return false;
            }

            // TMD objects are built from 'primitives'
            foreach (TMDPrimitivePacket prim in obj.Primitives)
            {
                // currently only polygon primitives are supported
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON) continue;

                // check which index list to use based on whether this primitive is alpha blended or not
                List<int> indicesList = (prim.Options & TMDPrimitivePacket.OptionsFlags.AlphaBlended) != 0
                    ? alphaBlendIndices
                    : indices;

                // get interfaces for different packet types from LibLSD
                ITMDPrimitivePacket primitivePacket = prim.PacketData;
                ITMDTexturedPrimitivePacket texturedPrimitivePacket = prim.PacketData as ITMDTexturedPrimitivePacket;
                ITMDColoredPrimitivePacket coloredPrimitivePacket = prim.PacketData as ITMDColoredPrimitivePacket;
                ITMDLitPrimitivePacket litPrimitivePacket = prim.PacketData as ITMDLitPrimitivePacket;

                // for each vertex in the primitive
                var polyIndices = new List<int>();
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

                        int texPageXPos = texPage % 16 * 128 - 640;
                        int texPageYPos = texPage < 16 ? 256 : 0;

                        // derive UV coords from the texture page
                        int uvIndex = i * 2;
                        int vramXPos = texPageXPos + texturedPrimitivePacket.UVs[uvIndex];
                        int vramYPos = texPageYPos + (256 - texturedPrimitivePacket.UVs[uvIndex + 1]);
                        float uCoord =
                            (vramXPos + 0.5f) / VRAM_WIDTH; // half-texel correction to prevent bleeding
                        float vCoord =
                            (vramYPos - 0.5f) / VRAM_HEIGHT; // half-texel correction to prevent bleeding

                        vertUV = new Vector2(uCoord, vCoord);

                        // handle UV material overrides
                        if (uvMaterialOverrides != null && uvHasOverride(vertUV, out int materialOverrideIndex))
                        {
                            // make sure we're adding this primitive to the correct submesh for the override
                            indicesList = materialOverrideIndices[materialOverrideIndex];

                            // map the UV to the overridden material
                            vertUV = uvMaterialOverrides[materialOverrideIndex].MapUVInRect(vertUV);
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
            result.subMeshCount = 2;
            result.SetTriangles(indices, submesh: 0, calculateBounds: false, baseVertex: 0);

            // alpha blended mesh
            result.SetTriangles(alphaBlendIndices, submesh: 1, calculateBounds: false, baseVertex: 0);

            // material override meshes
            if (uvMaterialOverrides != null)
            {
                for (int i = 0; i < uvMaterialOverrides.Count; i++)
                {
                    result.subMeshCount++; // inc submesh so we have empty submeshes

                    // ignore materials without any indices
                    if (materialOverrideIndices[i].Count == 0) continue;
                    result.SetTriangles(materialOverrideIndices[i].ToArray(), 2 + i, calculateBounds: false,
                        baseVertex: 0);
                }
            }

            return result;
        }

        /// <summary>
        ///     Get a Unity Texture2D from a loaded TIX image archive.
        /// </summary>
        /// <param name="tix">The TIX file loaded.</param>
        /// <returns>A Texture2D containing all images inside the TIX archive laid out as if in VRAM.</returns>
        public static Texture2D GetTextureFromTIX(TIX tix)
        {
            // create a texture 2D with the required format
            Texture2D tex = new Texture2D(VRAM_WIDTH, VRAM_HEIGHT, TextureFormat.ARGB32, mipChain: false)
            {
                wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Point, mipMapBias = -0.5f, anisoLevel = 2
            };

            // fill the texture with a white colour
            var fill = new Color[VRAM_WIDTH * VRAM_HEIGHT];
            for (int i = 0; i < fill.Length; i++)
            {
                fill[i] = new Color(r: 1, g: 1, b: 1, a: 1);
            }

            tex.SetPixels(fill);
            tex.Apply();

            // for each image within the archive, load it and paint it on the 'canvas' we created above
            foreach (TIXChunk chunk in tix.Chunks)
            {
                foreach (TIM tim in chunk.TIMs)
                {
                    TimData image = GetImageDataFromTIM(tim);
                    int actualXPos = (tim.PixelData.XPosition - 320) * 2;
                    int actualYPos = 512 - tim.PixelData.YPosition - image.Height;

                    tex.SetPixels(actualXPos, actualYPos, image.Width, image.Height, TimDataToColors(image));
                    tex.Apply();
                }
            }

            return tex;
        }

        /// <summary>
        ///     Load a PS1 TIM image to a Unity Texture2D.
        /// </summary>
        /// <param name="tim">The loaded TIM image.</param>
        /// <param name="clutIndex">The index of the CLUT to get the texture from.</param>
        /// <param name="flip">Whether or not we should flip the loaded image.</param>
        /// <returns></returns>
        public static Texture2D GetTextureFromTIM(TIM tim, int clutIndex = 0, bool flip = true)
        {
            // get the image data
            TimData data = GetImageDataFromTIM(tim, clutIndex);

            // create a texture with the required format
            Texture2D tex = new Texture2D(data.Width, data.Height, TextureFormat.ARGB32, mipChain: false)
            {
                wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Point
            };

            // set the texture's pixels to the TIM pixels
            tex.SetPixels(TimDataToColors(data, flip));
            tex.Apply();

            return tex;
        }

        /// <summary>
        ///     Get the pixels and width/height from a loaded TIM.
        /// </summary>
        /// <param name="tim">The loaded TIM.</param>
        /// <param name="clutIndex">The index of the CLUT to get the texture from.</param>
        /// <returns>TimData containing the TIM data.</returns>
        public static TimData GetImageDataFromTIM(TIM tim, int clutIndex = 0)
        {
            TimData data;
            data.Colors = tim.GetImage(clutIndex);
            data.Width = data.Colors.GetLength(dimension: 1);
            data.Height = data.Colors.GetLength(dimension: 0);
            return data;
        }

        /// <summary>
        ///     Convert a TimData struct to an array of Colors to be used for setting pixels in a texture.
        /// </summary>
        /// <param name="data">The TimData to use.</param>
        /// <param name="flip">Whether or not to flip the image.</param>
        /// <returns>The Color array corresponding to this TIM data.</returns>
        public static Color[] TimDataToColors(TimData data, bool flip = true)
        {
            // create the array
            var imageData = new Color[data.Colors.Length];

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

        /// <summary>
        ///     A struct to store data from a PS1 TIM file.
        /// </summary>
        public struct TimData
        {
            public IColor[,] Colors;
            public int Width;
            public int Height;
        }
    }
}
