using UnityEngine;
using UnityEngine.SceneManagement;
using Torii.UI;

namespace LSDR.Dream
{
    [CreateAssetMenu(menuName="System/DreamSystem")]
    public class DreamSystem : ScriptableObject
    {
        public ScenePicker DreamScene;

        public void BeginDream(Dream dream)
        {
            
        }
    }
}
