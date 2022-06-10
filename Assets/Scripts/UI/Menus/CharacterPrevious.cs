using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DarkJimmy.UI
{
    public class CharacterPrevious : Menu
    {
        [SerializeField]
        private StateUIView stats;
        [SerializeField]
        private RectTransform statsContent;
        [SerializeField]
        private List<Color> colorList;
         
        public override void Start()
        {
            SystemData system = CloudSaveManager.Instance.GetSystemData();
            int count = Enum.GetNames(typeof(CharacterProperty)).Length;

            for (int i = 0; i < count; i++)
            {
                StateUIView _stats = Instantiate(stats, statsContent);
                _stats.SetColor(colorList[i],((CharacterProperty)i).ToString());
                _stats.SetInfoSlider(system.CharacterDatas[0].GetCharacterProperty((CharacterProperty)i),100);       
            }
        }
    }
}
