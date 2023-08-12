using System.Collections.Generic;
using libLSD.Formats;
using LSDR.IO;
using Torii.Graphics;
using UnityEngine;

namespace LSDR.Entities.Original
{
    /// <summary>
    ///     LBDTileMap is a MonoBehaviour used for loading a tilemap of LBD files.
    ///     When given an LBDFolder and a TIX file, it will load all of the LBD files in the folder,
    ///     building GameObjects for each of them.
    /// </summary>
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class LBDTileMap : MonoBehaviour
    {
        public Dictionary<TMDObject, FastMesh> TileCache { get; } =
            new Dictionary<TMDObject, FastMesh>(new TMDObjectEqualityComparer());

        public void Update()
        {
            foreach (FastMesh fm in TileCache.Values)
            {
                fm.Draw();
            }
        }
    }
}
