using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Dream environment sequence")]
    public class DreamEnvironmentSequence : ScriptableObject
    {
        public List<DreamEnvironment> Environments;
    }
}
