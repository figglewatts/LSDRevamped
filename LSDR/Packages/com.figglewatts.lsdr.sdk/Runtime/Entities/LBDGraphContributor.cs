using System;
using System.Collections.Generic;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class LBDGraphContributor : MonoBehaviour
    {
        public enum LBDLayoutType
        {
            Horizontal,
            Vertical
        }

        public LBDLayoutType LayoutType;
        public int TileWidth = 1;
        public List<GraphContribution> LBDGraphData = new List<GraphContribution>();

        protected const float UPDATE_INTERVAL = 0.25f;

        protected float _t = 0;
        protected Transform _player;
        protected int _lastLbdIndex = -1;

        public void Start()
        {
            _player = EntityIndex.Instance.Get("__player").transform;
        }

        public void Update()
        {
            _t += Time.deltaTime;
            if (_player != null && _t > UPDATE_INTERVAL)
            {
                _t = 0;
                processPlayerPosition();
            }
        }

        protected void processPlayerPosition()
        {
            // get the player's position and calculate which LBD tile we're standing on
            var playerPos = _player.position;
            var lbdIndex = positionToLBDIndex(playerPos);

            // if we're not on an LBD tile, or we're on the same tile as before - do nothing
            if (lbdIndex == -1 || _lastLbdIndex == lbdIndex) return;

            Debug.Log("Moved to another LBD tile - influencing graph");

            // check to see if we have graph data for this LBD tile
            if (lbdIndex >= LBDGraphData.Count)
            {
                // we don't - let's log a warning as this is unintentional
                Debug.LogWarning(
                    $"LBD tile at index {lbdIndex} did not have " +
                    $"graph data (only have graph data up to {LBDGraphData.Count})");
            }
            else
            {
                // otherwise we have the data, so we can log it!
                Debug.Log(
                    $"lbdIndex: {lbdIndex}, playerPos: {playerPos}, lastIndex: {_lastLbdIndex}, count: {LBDGraphData.Count}");
                var contribution = LBDGraphData[lbdIndex];
                DreamControlManager.Managed.LogGraphContributionFromArea(contribution.Dynamic, contribution.Upper);
            }

            _lastLbdIndex = lbdIndex;
        }

        // convert a player's worldspace position to the index of the LBD tile they are on
        // returns -1 if the player is not over an LBD tile
        protected int positionToLBDIndex(Vector3 position)
        {
            var localPos = transform.InverseTransformPoint(position);
            if (LayoutType == LBDLayoutType.Horizontal)
            {
                int lbdYPos = (int)localPos.z / 20;
                int xMod = 0;
                if (lbdYPos % 2 == 1) xMod = 10;
                int lbdXPos = (int)(localPos.x + xMod) / 20;

                if (lbdYPos < 0 || lbdYPos >= TileWidth || lbdXPos < 0 || lbdXPos > TileWidth) return -1;

                return lbdYPos * TileWidth + lbdXPos;
            }
            else
            {
                int flooredY = (int)localPos.y;
                if (flooredY < 0) return -1;
                return flooredY;
            }
        }
    }
}
