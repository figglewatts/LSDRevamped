using System;
using libLSD.Formats;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace LSDR.SDK.Entities
{
    public class LBDChunk : BaseEntity
    {
        public LBDTileData[] Tiles;

        public bool DrawDebug = false;

        #if UNITY_EDITOR
        public void OnDrawGizmosSelected()
        {
            if (!DrawDebug) return;

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 20; x++)
                {
                    var tile = Tiles[x * 20 + y];

                    if (tile.UnknownFlag == 0) continue;

                    Vector3 pos = transform.position + new Vector3(x, -tile.Height, y);

                    string footstepCollision = Convert.ToString(tile.FootstepSoundAndCollision, 2).PadLeft(8, '0');
                    string tileInfo = $"{footstepCollision}";

                    Handles.Label(pos, tileInfo);
                    Handles.color = new Color(1, 0.5f, 1, 0.3f);
                    Handles.DrawWireCube(pos, new Vector3(1, 0, 1));
                }
            }
        }
        #endif

        [Serializable]
        public class LBDTileData
        {
            public byte UnknownFlag;
            public byte FootstepSoundAndCollision;
            public short Height;


            public LBDTileData(LBDTile tile)
            {
                UnknownFlag = tile.UnknownFlag;
                FootstepSoundAndCollision = tile.FootstepSoundAndCollision;
                Height = tile.TileHeight;
            }
        }
    }
}
