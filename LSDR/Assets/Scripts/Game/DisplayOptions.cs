using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LSDR.Game
{
    // resolution and framerate options for the video settings
    public static class DisplayOptions
    {
        public const int ORIGINAL_FRAMERATE = 20;

        // 0 is uncapped
        public static readonly int[] FramerateLimits = { 0, ORIGINAL_FRAMERATE, 30, 60, 90, 120, 144, 165, 240 };

        private static List<Vector2Int> _resolutions;

        // native (0x0) first, then each size largest first
        public static List<Vector2Int> Resolutions
        {
            get
            {
                if (_resolutions != null) return _resolutions;

                _resolutions = new List<Vector2Int> { Vector2Int.zero };
                _resolutions.AddRange(Screen.resolutions.Select(r => new Vector2Int(r.width, r.height))
                                            .Distinct()
                                            .OrderByDescending(r => r.x)
                                            .ThenByDescending(r => r.y));
                return _resolutions;
            }
        }

        public static Vector2Int NativeResolution => new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);

        public static string FramerateLabel(int limit)
        {
            if (limit == 0) return "Uncapped";
            if (limit == ORIGINAL_FRAMERATE) return $"Original ({ORIGINAL_FRAMERATE})";
            return limit.ToString();
        }

        public static string ResolutionLabel(Vector2Int size)
        {
            if (size == Vector2Int.zero) return $"Native ({NativeResolution.x}x{NativeResolution.y})";
            return $"{size.x}x{size.y}";
        }
    }
}