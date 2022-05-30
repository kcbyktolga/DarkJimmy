using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters;

namespace DarkJimmy.Manager
{
    public class GameManager:MonoBehaviour
    {
        //This class holds a static reference to itself to ensure that there will only be
        //one in existence. This is often referred to as a "singleton" design pattern. Other
        //scripts access this one through its public static methods
        static GameManager current;

        void Awake()
        {
            //If a Game Manager exists and this isn't it...
            if (current != null && current != this)
            {
                //...destroy this and exit. There can only be one Game Manager
                Destroy(gameObject);
                return;
            }

            //Set this as the current game manager
            current = this;
        }

        public static PlayerMovement GetPlayer()
        {
            return FindObjectOfType<PlayerMovement>();
        }
        
     
        internal static bool IsGameOver()
        {
            return false;
        }
    }
}

