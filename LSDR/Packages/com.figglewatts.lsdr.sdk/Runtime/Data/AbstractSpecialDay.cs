using UnityEngine;

namespace LSDR.SDK.Data
{
    public abstract class AbstractSpecialDay : ScriptableObject
    {
        public GraphContribution Contribution;
        public bool Always;

        public abstract void HandleDay(int dayNumber);
    }
}
