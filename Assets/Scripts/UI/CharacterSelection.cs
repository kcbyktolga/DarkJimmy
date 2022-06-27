using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class CharacterSelection : SlidePage<TabButton, PlayerData>
    {
        [SerializeField]
        private Image portrait;
        [SerializeField]
        private GameObject locked;
        [SerializeField]
        private TMP_Text levelText;
        [SerializeField]
        private TMP_Text characterLevel;
        [SerializeField]
        private Button playButton;
       
        [SerializeField]
        private Button upgradeButton;
        [SerializeField]
        private TMP_Text priceText;
        [SerializeField]
        private Image payIcon;

        CloudSaveManager csm;
        private bool IsLock { get; set; }
        SystemManager system;
        public override void Start()
        {
            csm = CloudSaveManager.Instance;
            system = SystemManager.Instance;
            globalData = csm.PlayerDatas;
            Count = csm.GetDefaultData().CharacterDatas.Count;
            Index = globalData.CurrentCharacterIndex;
            base.Start();
            Generate();

            levelText.text = LanguageManager.GetText("Level");

            SetCharacterInfo();

            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButton);

            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(OnUpgradeButton);
        }

        public override void Move(bool onClick, int amount)
        {
         
            Index += amount;
            Index = Mathf.Clamp(Index, 0, Count - 1);

            SetCharacterInfo();

            if (!IsLock)
                csm.SetCharacterIndex(Index);

            SetMoveButton();
        }

        private void SetCharacterInfo()
        {
            IsLock = csm.PlayerDatas.Characters[Index].isLock;
            locked.SetActive(IsLock);

            portrait.sprite = csm.GetDefaultData().CharacterDatas[Index].GetCharacterIcon();

            CharacterData data = !IsLock ? csm.GetCurrentCharacterData() : csm.GetDefaultData().CharacterDatas[Index];
            characterLevel.text = data.Level.ToString();

            upgradeButton.transform.GetChild(0).gameObject.SetActive(!IsLock);
            upgradeButton.transform.GetChild(1).gameObject.SetActive(IsLock);

            if (IsLock)
            {
                payIcon.sprite = system.GetPaySprite(data.payType);
                priceText.text = system.StringFormat(data.price);
            }
        }
        private void OnPlayButton()
        {
            if (!IsLock)
            {
                Fade.Instance.FadeOut(LoadGameScene);
                return;
            }

            Debug.Log("Önce karakteri satýn al aq!");

        }
        private void LoadGameScene()
        {
            SceneManager.LoadScene("Game");
        }
        private void OnUpgradeButton()
        {
            if (IsLock)
            {
                CharacterData data = csm.GetDefaultData().CharacterDatas[Index];

                if (csm.CanSpendGem(data.payType, data.price))
                {
                    csm.SetCharacterData(Index, data);
                    csm.SetCharacterIndex(Index);
                    csm.PlayerDatas.Characters[Index].isLock = false;
                    csm.SpendGem(data.payType, data.price);

                    Move(true,0);
                }
                else
                {
                    system.GemType = data.payType;
                    UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                }

            }
            else
            {
                //Upgrade..
            }
        }
        public override void OnSelect(int index)
        {

 
        }
        
      
    }

}
   