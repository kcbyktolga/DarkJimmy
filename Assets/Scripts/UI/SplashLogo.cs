using UnityEngine;


namespace DarkJimmy
{
    public class SplashLogo : MonoBehaviour,IAnimationEvent
    {
        
        public void AnimationEvent()
        {
            AudioManager.Instance.PlaySound("Logo Intro");
        }
    }
}

