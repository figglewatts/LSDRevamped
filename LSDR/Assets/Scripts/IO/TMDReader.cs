using System.Collections.Generic;
using libLSD.Formats;
using UnityEngine;

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

        /// <summary>
        /// Create a new instance of TMDReader.
        /// </summary>
        public TMDReader()
        {
            initializeCachedLists(DEFAULT_LIST_CAPACITY);

            // special handling for packet indices initialization, as most indices
            // a packet can have is 4 (in the case of a quad)
            _packetIndices = new List<int>(4);
        }

        public Mesh MeshFromTMD(TMD tmd)
        {
            Mesh mesh = new Mesh();

            foreach (var obj in tmd.ObjectTable)
            {
                Mesh objMesh = createTMDObject(obj, ref mesh);
            }
            
            clearCachedLists();
        }

        private Mesh createTMDObjectMesh(TMDObject obj)
        {
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
                    

                }

                _packetIndices.Clear();
            }
        }

        private void initializeCachedLists(int defaultCapacity)
        {
            _verts = new List<Vector3>(defaultCapacity);
            _normals = new List<Vector3>(defaultCapacity);
            _colors = new List<Color32>(defaultCapacity);
            _uvs = new List<Vector2>(defaultCapacity);
            _indices = new List<int>(defaultCapacity);
            _alphaBlendIndices = new List<int>(defaultCapacity);
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
