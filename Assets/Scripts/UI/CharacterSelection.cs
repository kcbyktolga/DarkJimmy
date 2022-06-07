using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class CharacterSelection : SlidePage<TabButton,PlayerData>
    {
        public override void Start()
        {
            globalData = CloudSaveManager.Instance.PlayerDatas;
            Count = globalData.GetAllCharacterCount;
            Index = globalData.CurrentCharacterIndex;
            base.Start();
            Generate();
        }

        public override void Generate()
        {
            float posX = 0;

            for (int i = 0; i < Count; i++)
            {
                TabButton tab = Instantiate(prefab, container);
                tab.OnClick(i, OnSelect);
                tabs.Add(tab);

                GetPosition.Add(posX);
                posX -= tab.GetComponent<RectTransform>().rect.width;         
            }       
        }
        public override void Move(bool onClick, int amount)
        {
            base.Move(onClick, amount);

            globalData.CurrentCharacterIndex = Index;
        }

        public override void OnSelect(int index)
        {

 
        }

        
    }

}
