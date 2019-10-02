using UnityEngine;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/GameExitSystem")]
    public class GameExitSystem : ScriptableObject
    {
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}