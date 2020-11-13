using System.Collections.Generic;
using libLSD.Formats;
using UnityEngine;

namespace LSDR.Entities.Original
{
    public class TODAnimation
    {
        public TOD Tod { get; }
        public List<Mesh> ObjectTable { get; }
        public Material Material;

        public TODAnimation(TOD tod, List<Mesh> objectTable, Material mat)
        {
            Tod = tod;
            ObjectTable = objectTable;
            Material = mat;
        }
    }
}
