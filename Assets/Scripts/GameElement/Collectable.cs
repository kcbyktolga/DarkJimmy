using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace DarkJimmy
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField]
        private Stats stats;   
        [SerializeField]
        private Material material;    
        [SerializeField]
        private int amount;

        private SystemManager system; 
        private BoxCollider2D boxCollider;
        private int defaultAmount;

        private void Awake()
        {
            system = SystemManager.Instance;           
            boxCollider = GetComponent<BoxCollider2D>();
            defaultAmount = amount;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
 
            if (stats.Equals(Stats.Mana) || stats.Equals(Stats.HP))
                if (GameSaveManager.Instance.IsMaxValue(stats))
                    return;
                else 
                {
                    int currentAmount;

                    if (amount >= GameSaveManager.Instance.ValueDiff(stats))
                    {
                        currentAmount = GameSaveManager.Instance.ValueDiff(stats);
                        amount -= GameSaveManager.Instance.ValueDiff(stats);
                    }
                    else
                    {
                        currentAmount = amount;
                        amount = 0;
                    }
                       
                    system.updateGMStats(stats, currentAmount);
                    OnCollect(amount==0);
                }
            else
            {
                system.updateGMStats(stats, amount);
                OnCollect(true);
            }


            if (stats == Stats.Key || stats == Stats.Gold)
                return;

            system.addGameElement(gameObject);


        }

        private void OnCollect(bool isOn)
        {
            system.particle(material, transform.position);
            AudioManager.Instance.PlaySound(stats.ToString());
            boxCollider.enabled = !isOn;
            gameObject.SetActive(!isOn);

        }

        private void OnDisable()
        {
            boxCollider.enabled = true;
            amount = defaultAmount;
        }
    }

    
}



