using System;
using System.Collections;
using System.IO;
using LSDR.IO;
using libLSD.Formats;
using Torii.Resource;
using UnityEngine;
using LSDR.Util;
using LSDR.Visual;
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
        /// <summary>
        /// The folder to load LBD files from. Set in inspector.
        /// </summary>
        public string LBDFolder;
        
        /// <summary>
        /// The TIX file to load into virtual PSX VRAM. Set in inspector.
        /// </summary>
        public string TIXFile;

        // the loaded TIX file
        private TIX _tix;

        /// <summary>
        /// The LBD tiling mode.
        /// </summary>
        public enum LBDTiling
        {
            None,
            Regular
        }

        /// <summary>
        /// How the loaded LBD should be tiled. Set in inspector.
        /// </summary>
        public LBDTiling Mode;
        
        /// <summary>
        /// The width of the LBD tiling. LBD 'slabs' will be placed along the X-axis until the counter exceeds this
        /// value, after which they will be placed along the X-axis on the next Z increment. Set in inspector.
        /// </summary>
        public int LBDWidth = 1;

        private bool _loaded = false;

        private void Start()
        {
            /*StartCoroutine(loadLbd());*/
            // load the TIX into memory, then put it into the virtual PSX VRAM
            
            
        }

        public void Update()
        {
            if (!_loaded)
            {
                Profiler.BeginSample("LBD");
            
                _tix = ResourceManager.Load<TIX>(IOUtil.PathCombine(Application.streamingAssetsPath, TIXFile));
                PsxVram.LoadVramTix(_tix);

                // get an array of all of the LBD files in the given directory
                // TODO: error checking for LBD path
                string[] lbdFiles = Directory.GetFiles(IOUtil.PathCombine(Application.streamingAssetsPath, LBDFolder),
                    "*.LBD", SearchOption.AllDirectories);
            
                int i = 0;
                foreach (var file in lbdFiles)
                {
                    // load the LBD and create GameObjects for its tiles
                    var lbd = ResourceManager.Load<LBD>(file);
                    GameObject lbdObj = LibLSDUnity.CreateLBDTileMap(lbd);

                    // position the LBD 'slab' based on its tiling mode
                    if (Mode == LBDTiling.Regular)
                    {
                        int xPos = i % LBDWidth;
                        int yPos = i / LBDWidth;
                        int xMod = 0;
                        if (yPos % 2 == 1)
                        {
                            xMod = 10;
                        }
                        lbdObj.transform.position = new Vector3((xPos * 20) - xMod, 0, yPos * 20);
                        i++;
                    }
                }
            
                Profiler.EndSample();

                _loaded = true;
            }
        }

        /*
        private IEnumerator loadLbd()
        {
            yield return new WaitForSeconds(1);
            
            // load the TIX into memory, then put it into the virtual PSX VRAM
            _tix = ResourceManager.Load<TIX>(IOUtil.PathCombine(Application.streamingAssetsPath, TIXFile));
            PsxVram.LoadVramTix(_tix);

            // get an array of all of the LBD files in the given directory
            // TODO: error checking for LBD path
            string[] lbdFiles = Directory.GetFiles(IOUtil.PathCombine(Application.streamingAssetsPath, LBDFolder),
                "*.LBD", SearchOption.AllDirectories);
            
            int i = 0;
            foreach (var file in lbdFiles)
            {
                // load the LBD and create GameObjects for its tiles
                var lbd = ResourceManager.Load<LBD>(file);
                GameObject lbdObj = LibLSDUnity.CreateLBDTileMap(lbd);

                // position the LBD 'slab' based on its tiling mode
                if (Mode == LBDTiling.Regular)
                {
                    int xPos = i % LBDWidth;
                    int yPos = i / LBDWidth;
                    int xMod = 0;
                    if (yPos % 2 == 1)
                    {
                        xMod = 10;
                    }
                    lbdObj.transform.position = new Vector3((xPos * 20) - xMod, 0, yPos * 20);
                    i++;
                }
            }
        }
        */
    }
}
