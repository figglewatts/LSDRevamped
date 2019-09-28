using System.Collections.Generic;
using System.Linq;
using libLSD.Formats;

namespace LSDR.IO
{
    public class TMDObjectEqualityComparer : IEqualityComparer<TMDObject>
    {
        public bool Equals(TMDObject x, TMDObject y)
        {
            if (!x.Normals.SequenceEqual(y.Normals)) return false;
            if (!x.Vertices.SequenceEqual(x.Vertices)) return false;

            for (int i = 0; i < x.NumPrimitives; i++)
            {
                if (!tmdPrimitivePacketEquals(x.Primitives[i], y.Primitives[i])) return false;
            }

            return true;
        }

        public int GetHashCode(TMDObject obj)
        {
            unchecked
            {
                var hashCode = obj.Normals.Length;
                foreach (var norm in obj.Normals)
                {
                    hashCode = (hashCode * 397) ^ norm.GetHashCode();
                }

                foreach (var vert in obj.Vertices)
                {
                    hashCode = (hashCode * 397) ^ vert.GetHashCode();
                }

                foreach (var prim in obj.Primitives)
                {
                    hashCode = (hashCode * 397) ^ tmdPrimitivePacketHashCode(prim);
                }

                return hashCode;
            }
        }

        private int tmdPrimitivePacketHashCode(TMDPrimitivePacket p)
        {
            var hashCode = p.Flags.GetHashCode();
            hashCode = (hashCode * 397) ^ p.Options.GetHashCode();
            hashCode = (hashCode * 397) ^ p.Type.GetHashCode();
            hashCode = (hashCode * 397) ^ p.ILen.GetHashCode();
            hashCode = (hashCode * 397) ^ p.OLen.GetHashCode();
            hashCode = (hashCode * 397) ^ p.PacketData.Vertices.Length.GetHashCode();
            
            IColoredPrimitivePacket coloredPacket = p.PacketData as IColoredPrimitivePacket;
            if (coloredPacket != null)
            {
                foreach (var col in coloredPacket.Colors)
                {
                    hashCode = (hashCode * 397) ^ col.GetHashCode();
                }
            }

            ILitPrimitivePacket litPacket = p.PacketData as ILitPrimitivePacket;
            if (litPacket != null)
            {
                foreach (var norm in litPacket.Normals)
                {
                    hashCode = (hashCode * 397) ^ norm.GetHashCode();
                }
            }

            ITexturedPrimitivePacket texturedPacket = p.PacketData as ITexturedPrimitivePacket;
            if (texturedPacket != null)
            {
                hashCode = (hashCode * 397) ^ texturedPacket.Texture.TexturePageNumber.GetHashCode();
                hashCode = (hashCode * 397) ^ texturedPacket.Texture.ColorMode.GetHashCode();
                hashCode = (hashCode * 397) ^ texturedPacket.Texture.AlphaBlendRate.GetHashCode();
                hashCode = (hashCode * 397) ^ texturedPacket.ColorLookup.XPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ texturedPacket.ColorLookup.YPosition.GetHashCode();
                foreach (var uv in texturedPacket.UVs)
                {
                    hashCode = (hashCode * 397) ^ uv.GetHashCode();
                }
            }
            
            foreach (var vert in p.PacketData.Vertices)
            {
                hashCode = (hashCode * 397) ^ vert.GetHashCode();
            }
            hashCode = (hashCode * 397) ^ p.SpriteSize.GetHashCode();
            return hashCode;
        }

        private bool tmdPrimitivePacketEquals(TMDPrimitivePacket x, TMDPrimitivePacket y)
        {
            if (x.Flags != y.Flags) return false;
            if (x.Options != y.Options) return false;
            if (x.Type != y.Type) return false;
            if (x.ILen != y.ILen) return false;
            if (x.OLen != y.OLen) return false;
            if (!x.PacketData.Vertices.SequenceEqual(y.PacketData.Vertices)) return false;
            if (x.SpriteSize != y.SpriteSize) return false;
            return true;
        }
    }
}