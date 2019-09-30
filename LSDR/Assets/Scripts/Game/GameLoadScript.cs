using UnityEngine;

namespace LSDR.Game
{
    public class GameLoadScript : MonoBehaviour
    {
        public GameLoadSystem GameLoadSystem;

        public void LoadGame()
        {
            StartCoroutine(GameLoadSystem.LoadGameCoroutine());
        }
    }
}