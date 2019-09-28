using System.Collections.Generic;
using libLSD.Formats;
using libLSD.Types;
using LSDR.Visual;
using UnityEngine;
using UnityEngine.Profiling;

namespace LSDR.IO
{
    /// <summary>
    /// TMDReader is used to load/create meshes from PSX TMD files.
    /// </summary>
    public class TMDReader
    {
        // default list capacity, used to avoid resizing lists a lot
        private const int DEFAULT_LIST_CAPACITY = 64;
        
        // cached instances of lists to use when building a mesh
        private List<Vector3> _verts;
        private List<Vector3> _normals;
        private List<Color32> _colors;
        private List<Vector2> _uvs;
        private List<int> _indices;
        private List<int> _alphaBlendIndices;
        private List<int> _packetIndices;
        private List<int> _polyIndices;

        /// <summary>
        /// Create a new instance of TMDReader.
        /// </summary>
        public TMDReader()
        {
            initializeCachedLists(DEFAULT_LIST_CAPACITY);

            // special handling for packet indices initialization, as most indices
            // a packet can have is 4 (in the case of a quad)
            _packetIndices = new List<int>
            {
                0, 0, 0, 0
            };
        }

        public Mesh MeshFromTMD(TMD tmd)
        {
            Mesh mesh = new Mesh();

            foreach (var obj in tmd.ObjectTable)
            {
                Mesh objMesh = CreateTMDObjectMesh(obj);
            }

            return null;
        }

        public Mesh CreateTMDObjectMesh(TMDObject obj)
        {
            clearCachedLists();
            
            foreach (var prim in obj.Primitives)
            {
                // currently only polygon primitives are supported
                if (prim.Type != TMDPrimitivePacket.Types.POLYGON) continue;

                // figure out which index list to use based on whether or not this primitive is alpha blended
                List<int> indicesList = (prim.Options & TMDPrimitivePacket.OptionsFlags.AlphaBlended) != 0
                    ? _alphaBlendIndices
                    : _indices;
                
                // get interfaces for different packet types from LibLSD
                IPrimitivePacket primitivePacket = prim.PacketData;
                ITexturedPrimitivePacket texturedPrimitivePacket = prim.PacketData as ITexturedPrimitivePacket;
                IColoredPrimitivePacket coloredPrimitivePacket = prim.PacketData as IColoredPrimitivePacket;
                ILitPrimitivePacket litPrimitivePacket = prim.PacketData as ILitPrimitivePacket;

                for (int i = 0; i < primitivePacket.Vertices.Length; i++)
                {
                    // get index into vertices array
                    int vertIndex = primitivePacket.Vertices[i];
                    _packetIndices[i] = _verts.Count;

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
                        foreach (var uv in _uvs)
                        {
                            if (uv.Equals(vertUV))
                            {
                                vertUV += new Vector2(0.0001f, 0.0001f);    
                            }
                        }
                    }
                    
                    // add all computed aspects of vertex to lists
                    _verts.Add(vec3VertPos);
                    _normals.Add(vertNorm);
                    _colors.Add(vertCol);
                    _uvs.Add(vertUV);
                }
                // we want to add extra indices if this primitive is a quad (to triangulate)
                bool isQuad = (prim.Options & TMDPrimitivePacket.OptionsFlags.Quad) != 0;
                
                _polyIndices.Add(_packetIndices[0]);
                _polyIndices.Add(_packetIndices[1]);
                _polyIndices.Add(_packetIndices[2]);

                if (isQuad)
                {
                    _polyIndices.Add(_packetIndices[2]);
                    _polyIndices.Add(_packetIndices[1]);
                    _polyIndices.Add(_packetIndices[3]);
                }

                // if this primitive is double sided we want to add more vertices with opposite winding order
                if ((prim.Flags & TMDPrimitivePacket.PrimitiveFlags.DoubleSided) != 0)
                {
                    _polyIndices.Add(_packetIndices[1]);
                    _polyIndices.Add(_packetIndices[0]);
                    _polyIndices.Add(_packetIndices[2]);

                    if (isQuad)
                    {
                        _polyIndices.Add(_packetIndices[1]);
                        _polyIndices.Add(_packetIndices[2]);
                        _polyIndices.Add(_packetIndices[3]);
                    }
                }
                
                // add the indices to the list
                indicesList.AddRange(_polyIndices);
                _polyIndices.Clear();
            }

            Mesh result = new Mesh();
            result.SetVertices(_verts);
            result.SetNormals(_normals);
            result.SetColors(_colors);
            result.SetUVs(0, _uvs);
            
            // regular mesh
            if (_indices.Count >= 3)
            {
                result.SetTriangles(_indices, 0, false, 0);
            }

            // alpha blended mesh
            if (_alphaBlendIndices.Count >= 3)
            {
                result.subMeshCount = 2;
                result.SetTriangles(_alphaBlendIndices, 1, false, 0);
            }

            return result;
        }

        private void initializeCachedLists(int defaultCapacity)
        {
            _verts = new List<Vector3>(defaultCapacity);
            _normals = new List<Vector3>(defaultCapacity);
            _colors = new List<Color32>(defaultCapacity);
            _uvs = new List<Vector2>(defaultCapacity);
            _indices = new List<int>(defaultCapacity);
            _alphaBlendIndices = new List<int>(defaultCapacity);
            _polyIndices = new List<int>(12); // at most can hold double sided quad
        }

        private void clearCachedLists()
        {
            _verts.Clear();
            _normals.Clear();
            _colors.Clear();
            _uvs.Clear();
            _indices.Clear();
            _alphaBlendIndices.Clear();
        }
    }
}
