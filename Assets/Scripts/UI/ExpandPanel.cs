using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public abstract class ExpandPanel<T> : MonoBehaviour where T:MonoBehaviour
    {
        [SerializeField]
        private List<T> elements;
        [SerializeField]
        private float spacing;
        [SerializeField]
        private ExpandDirection direction;
        [SerializeField]
        private float duration;

        public delegate void OnComplete();
        public OnComplete onComplete;

        public SystemManager system;
        public virtual void Awake()
        {
            system = SystemManager.Instance;
        }

        public virtual void Expand(bool isForward)
        {
            Vector2 endPos = Vector2.zero;
            
            for (int i = 0; i < elements.Count; i++)
            {
                Color endColor = system.GetWhiteAlfaColor(isForward);
                ColorChange(elements[i],endColor,duration);
                RectTransform rectT = elements[i].GetComponent<RectTransform>();
                rectT.DOAnchorPos(endPos, duration);
                endPos += isForward? GetPos(direction, rectT):Vector2.zero;
            }

            onComplete?.Invoke();
        }

        public abstract void ColorChange(T element ,Color endColor,float duration);
        
        private Vector2 GetPos(ExpandDirection dir, RectTransform transform)
        {
            return dir switch
            {
                ExpandDirection.Down => (transform.rect.height +spacing)*Vector2.down ,
                ExpandDirection.Right => (transform.rect.width+spacing) * Vector2.right,
                ExpandDirection.Left => (transform.rect.width+spacing)*Vector2.left,
                _ => (transform.rect.height + spacing) * Vector2.up,
            };
        }
    }
    public enum ExpandDirection
    {
        Up,
        Down,
        Right,
        Left
    }
}

