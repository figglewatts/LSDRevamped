using System.Globalization;
using UnityEngine;

namespace Torii.Util
{
    // all credit goes to Garry Newman for this one: https://garry.tv/timesince
    public struct FixedTimeSince
    {
        private float time;

        public static implicit operator float(FixedTimeSince ts) { return Time.fixedTime - ts.time; }

        public static implicit operator FixedTimeSince(float ts)
        {
            return new FixedTimeSince { time = Time.fixedTime - ts };
        }

        public override string ToString() { return ((float)this).ToString(CultureInfo.InvariantCulture); }
    }
}
