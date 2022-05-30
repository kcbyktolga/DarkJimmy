using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters;

namespace DarkJimmy
{
    public class BackgroundManager : MonoBehaviour
    {
        public BackgroundsType backgroundType;
        public List<BackgroundElement> backgrounds;

        PlayerMovement player;

        void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            player = FindObjectOfType<PlayerMovement>();

            for (int i = 0; i < backgrounds.Count; i++)
                backgrounds[i].SetBackground(backgroundType);
        }
        void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            for (int i = 0; i < backgrounds.Count; i++)
                backgrounds[i].Move(player);
        }
    }

    [System.Serializable]
    public class BackgroundElement
    {
        public List<Sprite> spriteList;
        public List<SpriteRenderer> backgrounds;
        public List<float> startPosx = new List<float>();
        private float diffX;

  
        public void SetBackground(BackgroundsType type)
        {
            diffX = Vector2.Distance(backgrounds[0].transform.position, backgrounds[1].transform.position);

            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgrounds[i].sprite = spriteList[(int)type];
                startPosx.Add(backgrounds[i].transform.position.x);
            }
               
        }

        public void Move(PlayerMovement player)
        {
            for (int i = 0; i < backgrounds.Count; i++)
            {             
                if (player.transform.position.x> startPosx[i] + diffX)
                {
                    backgrounds[i].transform.position = new Vector3(startPosx[i] +diffX*2,backgrounds[i].transform.position.y,backgrounds[i].transform.position.z);

                    startPosx[i] = backgrounds[i].transform.position.x;
                }
                else if(player.transform.position.x < startPosx[i] - diffX)
                {
                    backgrounds[i].transform.position = new Vector3(startPosx[i] - diffX*2, backgrounds[i].transform.position.y, backgrounds[i].transform.position.z);

                    startPosx[i] = backgrounds[i].transform.position.x;
                }
            }
           
        }
    }
    public enum BackgroundsType
    {
        Forest,
        Cave,
        Snow
    }
}

