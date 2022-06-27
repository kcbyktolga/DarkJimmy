using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace DarkJimmy
{
    public class Collectable : MonoBehaviour,IAnimationEvent
    {
        [SerializeField]
        private Stats stats;
        [SerializeField]
        private int amount;
      

        private Animator animator;
        private int onCollect;

        private void Start()
        {
            animator = GetComponent<Animator>();
            onCollect = Animator.StringToHash("Collect");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            animator.SetTrigger(onCollect);

            if (stats.Equals(Stats.Timer))
            {
                int value = GameSaveManager.Instance.CountDown += amount;
                SystemManager.Instance.updateStats(stats,value);
            }
            else
                GameSaveManager.Instance.UpdateStatsValue(stats, amount);

        }

        public void AnimationEvent()
        {
            gameObject.SetActive(false);
        }
    }
}



