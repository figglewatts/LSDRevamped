using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Special Day - Random")]
    public class RandomSpecialDay : AbstractSpecialDay
    {
        public List<AbstractSpecialDay> SpecialDayChoices;

        public override void HandleDay(int dayNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
