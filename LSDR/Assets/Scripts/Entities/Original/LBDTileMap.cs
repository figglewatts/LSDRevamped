using System.IO;
using IO;
using libLSD.Formats;
using Torii.Resource;
using UnityEditor;
using UnityEngine;
using Util;
using Visual;

namespace Entities.Original
{
    public class LBDTileMap : MonoBehaviour
    {
        public string LBDFolder;
        public string TIXFile;

        private TIX _tix;

        public enum LBDTiling
        {
            None,
            Regular
        }

        public LBDTiling Mode;
        public int LBDWidth = 1;
        
        private void Start()
        {
            _tix = ResourceManager.Load<TIX>(IOUtil.PathCombine(Application.streamingAssetsPath, TIXFile));
            PsxVram.LoadVramTix(_tix);

            string[] lbdFiles = Directory.GetFiles(IOUtil.PathCombine(Application.streamingAssetsPath, LBDFolder),
                "*.LBD", SearchOption.AllDirectories);
            
            int i = 0;
            foreach (var file in lbdFiles)
            {
                var lbd = ResourceManager.Load<LBD>(file);
                GameObject lbdObj = LibLSDUnity.CreateLBDTileMap(lbd);

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
    }
}
