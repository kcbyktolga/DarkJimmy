using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.Experimental.U2D.Animation;

namespace DarkJimmy
{
    public class CharacterSkinSwap : MonoBehaviour
    {
        [Header("Sprites")]
        public SpriteResolver Head;
        public SpriteResolver Body;
        public SpriteResolver HeadDie;

        private void Start()
        {           
            SetSkin(CloudSaveManager.Instance.GetCurrentCharacter());
        }
  
        
        public void SetSkin(int index)
        {
            Head.SetCategoryAndLabel("Head", $"Head-{index}");
            Body.SetCategoryAndLabel("Body", $"Body-{index}");
            HeadDie.SetCategoryAndLabel("HeadDie", $"HeadDie-{index}");
        }
        
    }
}

