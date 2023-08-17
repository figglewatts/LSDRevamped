using System.Globalization;
using UnityEngine;

namespace Torii.Util
{
    // all credit goes to Garry Newman for this one: https://garry.tv/timesince
    public struct TimeSince
    {
        private float time;

        public static implicit operator float(TimeSince ts) { return Time.time - ts.time; }

        public static implicit operator TimeSince(float ts) { return new TimeSince { time = Time.time - ts }; }

        public override string ToString() { return ((float)this).ToString(CultureInfo.InvariantCulture); }
    }
}
