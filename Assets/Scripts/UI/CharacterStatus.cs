using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class CharacterStatus : MonoBehaviour
    {
        [SerializeField]
        private List<Image> characterIcon;  
        CloudSaveManager csm;
        int characterIndex;
        private void Start()
        {
            csm = CloudSaveManager.Instance;
            characterIndex =csm.PlayerDatas.CurrentCharacterIndex;

            SetCharacterStatus();     
        }

        private void SetCharacterStatus()
        {
            for (int i = 0; i < characterIcon.Count; i++)
                characterIcon[i].sprite = csm.GetDefaultData().CharacterDatas[characterIndex].GetCharacterIcon();

        } 
    }
}

