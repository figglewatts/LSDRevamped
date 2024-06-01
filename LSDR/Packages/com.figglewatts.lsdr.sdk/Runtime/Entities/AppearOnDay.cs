using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class AppearOnDay : MonoBehaviour
    {
        public DayKind Day;

        public void Start()
        {
            var dreamSystem = DreamControlManager.Managed;
            var dayNumber = dreamSystem.CurrentDay;

            if (!checkDayKind(dayNumber)) gameObject.SetActive(false);
        }

        protected bool checkDayKind(int dayNumber)
        {
            bool result = false;
            if (Day == DayKind.None) return false;

            if (Day.HasFlag(DayKind.Even))
            {
                result |= dayNumber % 2 == 0;
            }

            if (Day.HasFlag(DayKind.Odd))
            {
                result |= dayNumber % 2 != 0;
            }

            bool dayIsNumber(int dayNumber, int weekIndex)
            {
                return ((dayNumber - 1) % 7) + 1 == weekIndex;
            }

            if (Day.HasFlag(DayKind.One))
            {
                result |= dayIsNumber(dayNumber, 1);
            }

            if (Day.HasFlag(DayKind.Two))
            {
                result |= dayIsNumber(dayNumber, 2);
            }

            if (Day.HasFlag(DayKind.Three))
            {
                result |= dayIsNumber(dayNumber, 3);
            }

            if (Day.HasFlag(DayKind.Four))
            {
                result |= dayIsNumber(dayNumber, 4);
            }

            if (Day.HasFlag(DayKind.Five))
            {
                result |= dayIsNumber(dayNumber, 5);
            }

            if (Day.HasFlag(DayKind.Six))
            {
                result |= dayIsNumber(dayNumber, 6);
            }

            if (Day.HasFlag(DayKind.Seven))
            {
                result |= dayIsNumber(dayNumber, 7);
            }

            return result;
        }
    }
}
