using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Image))]
    public class UILoadingIcon : MonoBehaviour
    {
        public float AnimationSpeed;

        private Animator _loadingIconAnimator;

        public void Awake()
        {
            _loadingIconAnimator = GetComponent<Animator>();
            _loadingIconAnimator.speed = AnimationSpeed;
        }
    }
}