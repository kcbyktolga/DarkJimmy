using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DarkJimmy.UI
{
    public class SlidePage<T, A> : TabGenerator<T,A> where T:class where A:class
    {
        [SerializeField]
        private BaseButton next;
        [SerializeField]
        private BaseButton previous;

        public List<float> GetPosition = new List<float>();
        public int Count { get; set; }
        public virtual void Start()
        {
            next.OnClick(true, 1, Move);
            previous.OnClick(true, -1, Move);

            SetMoveButton();
        }

        public override void Generate()
        {
            
        }

        public  virtual void Move(bool onClick, int amount)
        {
            if (onClick)
            {
                Index += amount;
                Index = Mathf.Clamp(Index, 0, Count - 1);

                PreviousIndex = NextIndex;
                NextIndex = Index;
            }

            SetMoveButton();
            // StartCoroutine(Slide(container, GetPosition[NextIndex]));
            Sliding(container,GetPosition[NextIndex]);

        }

        public virtual void SetMoveButton()
        {
            previous.gameObject.SetActive(Index != 0);
            next.gameObject.SetActive(Index != Count - 1);
        }


    }
}

