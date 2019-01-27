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
        public string[] LBDFile;
        public string TIXFile;

        private TIX _tix;
        
        private void Start()
        {
            _tix = ResourceManager.Load<TIX>(IOUtil.PathCombine(Application.streamingAssetsPath, TIXFile));
            PsxVram.LoadVramTix(_tix);

            int i = 0;
            foreach (var file in LBDFile)
            {
                var lbd = ResourceManager.Load<LBD>(IOUtil.PathCombine(Application.streamingAssetsPath, file));
                GameObject lbdObj = LibLSDUnity.CreateLBDTileMap(lbd);
                i++;
            }
        }
    }
}
