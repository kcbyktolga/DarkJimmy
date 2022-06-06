using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class CharacterSelection : SlidePage<TabButton,PlayerData>
    {
        public float range;
        
        public override void Start()
        {
            data = CloudSaveManager.Instance.PlayerDatas;
            Count = data.GetAllCharacterCount;
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
            SetMoveButton();
        }

        public override void OnSelect(int index)
        {
            //base.OnSelect(index);

            Debug.Log(index);
        }

        
    }

}
