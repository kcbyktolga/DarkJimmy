using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DarkJimmy.UI
{
    public class Popup : Menu
    {
        [SerializeField]
        private StateUIView stats;
        [SerializeField]
        private RectTransform statsContent;
        [SerializeField]
        private List<Color> colorList;
    
        private Shop shop;
 
        public override void Start()
        {
            shop = FindObjectOfType<Shop>();
            Catalog catalog = shop.catalog;
            int count = Enum.GetNames(typeof(CharacterProperty)).Length;

            for (int i = 0; i < count; i++)
            {
                StateUIView _stats = Instantiate(stats, statsContent);
                _stats.SetColor(colorList[i],((CharacterProperty)i).ToString());
                _stats.SetInfoSlider(catalog.characterDatas[0].GetCharacterPropert((CharacterProperty)i),100);       
            }
        }


    }
}

