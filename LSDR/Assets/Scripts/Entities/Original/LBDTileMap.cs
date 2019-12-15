using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LSDR.IO;
using libLSD.Formats;
using LSDR.Dream;
using Torii.Resource;
using UnityEngine;
using LSDR.Util;
using LSDR.Visual;
using Torii.Graphics;
using Torii.Pooling;
using UnityEngine.Profiling;

namespace LSDR.Entities.Original
{
    /// <summary>
    /// LBDTileMap is a MonoBehaviour used for loading a tilemap of LBD files.
    /// When given an LBDFolder and a TIX file, it will load all of the LBD files in the folder,
    /// building GameObjects for each of them.
    /// </summary>
    public class LBDTileMap : MonoBehaviour
    {
        public Dictionary<TMDObject, FastMesh> TileCache { get; } =
            new Dictionary<TMDObject, FastMesh>(new TMDObjectEqualityComparer());

        public void Awake()
        {
            TileCache.Clear();
        }

        public void Update()
        {
            foreach (var fm in TileCache.Values)
            {
                fm.Draw();
            }
        }
    }
}
