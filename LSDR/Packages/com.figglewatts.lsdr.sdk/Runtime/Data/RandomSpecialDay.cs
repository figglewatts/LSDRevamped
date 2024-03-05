using System.Collections.Generic;
using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Random")]
    public class RandomSpecialDay : AbstractSpecialDay
    {
        public List<AbstractSpecialDay> SpecialDayChoices;

        public override void HandleDay(int dayNumber)
        {
            RandUtil.RandomListElement(SpecialDayChoices).HandleDay(dayNumber);
        }
    }
}
