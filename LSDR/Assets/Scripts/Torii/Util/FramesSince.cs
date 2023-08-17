using System.Globalization;
using UnityEngine;

namespace Torii.Util
{
    // all credit goes to Garry Newman for this one: https://garry.tv/timesince
    public struct FramesSince
    {
        private int frames;

        public static implicit operator int(FramesSince ts) { return Time.frameCount - ts.frames; }

        public static implicit operator FramesSince(int ts)
        {
            return new FramesSince { frames = Time.frameCount - ts };
        }

        public override string ToString() { return ((int)this).ToString(CultureInfo.InvariantCulture); }
    }
}
