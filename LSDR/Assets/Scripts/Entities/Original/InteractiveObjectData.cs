using System.Collections.Generic;
using libLSD.Formats;
using LSDR.IO;
using UnityEngine;

namespace LSDR.Entities.Original
{
    public class InteractiveObjectData
    {
        public List<TODAnimation> Animations { get; }
        public List<Mesh> ObjectTable { get; }

        private readonly MOM _mom;

        public InteractiveObjectData(MOM mom, Material mat)
        {
            _mom = mom;

            ObjectTable = LibLSDUnity.CreateMeshesFromTMD(mom.TMD);

            Animations = new List<TODAnimation>();
            foreach (var anim in _mom.MOS.TODs)
            {
                Animations.Add(new TODAnimation(anim, ObjectTable, mat));
            }
        }
    }
}
