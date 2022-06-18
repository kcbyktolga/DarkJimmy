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
        private Image characterIcon;
        [SerializeField]
        private TMP_Text characterLevel;

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
            characterIcon.sprite = csm.GetDefaultData().CharacterDatas[characterIndex].GetCharacterIcon();
            characterLevel.text = csm.PlayerDatas.Characters[characterIndex].Level.ToString();
        }
    }
}

