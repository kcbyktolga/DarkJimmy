using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class Popup : Menu
    {
        [SerializeField]
        private List<StateUIView> statsList;
    
        private Shop shop;
        private CharacterData data;

        public override void Start()
        {
            shop = FindObjectOfType<Shop>();
            Catalog catalog = shop.catalog;

            statsList[0].SetInfoSlider(catalog.characterDatas[0].Energy,100);
            statsList[1].SetInfoSlider(catalog.characterDatas[0].Mana, 100);
            statsList[2].SetInfoSlider(catalog.characterDatas[0].Speed, 100);
            statsList[3].SetInfoSlider(catalog.characterDatas[0].ERR, 100);
            statsList[4].SetInfoSlider(catalog.characterDatas[0].MMR, 100);

        }


    }
}

