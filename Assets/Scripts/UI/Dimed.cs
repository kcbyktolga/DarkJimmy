using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class Dimed : MonoBehaviour
    {
        [SerializeField]
        private Material dimed;
        private readonly float duration=180;    
        private Vector2 endPos = new Vector2(-5, 5);

        void Start()
        {
            dimed.mainTextureOffset = Vector2.zero;
            dimed.DOOffset(endPos, duration).SetEase(Ease.Linear).SetLoops(10);
        }

        private void OnDestroy()
        {
            dimed.DOKill();
            dimed.mainTextureOffset = Vector2.zero;
        }
    }
}

