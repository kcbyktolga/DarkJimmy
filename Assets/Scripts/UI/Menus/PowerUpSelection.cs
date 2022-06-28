using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DarkJimmy.UI
{
    public class PowerUpSelection : TabGenerator<PowerUpButton, CloudSaveManager>
    {
        [Header("Preparation Property")]
        [SerializeField]
        private TMP_Text powerUpHeader;
        [SerializeField]
        private TMP_Text powerUpDescription;
        [SerializeField]
        private TMP_Text priceText;
        [SerializeField]
        private BaseButton playButton;

        [Header("Display")]
        [SerializeField]
        private GameObject gameDisplay;
        [SerializeField]
        private GameObject prepareDisplay;

        private Catalog catalog;
        private SystemManager system;
        private GameSaveManager gsm;
        private Fade fader;

        private GemType PayType { get; set; }      
        private int Price { get; set; }

        private void Start()
        {
            system = SystemManager.Instance;
            globalData = CloudSaveManager.Instance;
            catalog = globalData.GetDefaultData().Catalog;
            gsm = GameSaveManager.Instance;
            fader = Fade.Instance;

            playButton.OnClick(OnPlay);

            Generate();
        }
        public override void Generate()
        {
            for (int i = 0; i < catalog.GetPowerUps.Count; i++)
            {
                PowerUpStruct ps = catalog.GetPowerUps[i];
                PowerUpButton tab = Instantiate(prefab, container);
                tab.SetPowerUpButton(ps);
                tab.OnClick(i, OnSelect);
                tabs.Add(tab);
            }
        }
        public override void OnSelect(int index)
        {
            bool isSame = NextIndex == index;

            base.OnSelect(index);

            PowerUpButton prev = GetTab(PreviousIndex);
            PowerUpButton next = GetTab(NextIndex);

            if(!isSame)
                prev.SetTabButton(false);

            next.SetTabButton(true);

            SetPowerUpInformation(next.GetDescription(out string header, out bool isOn, out GemType payType, out int price),header,isOn, payType, price);

        }
        public void SetPowerUpInformation(string description, string header, bool isOn,GemType payType, int price )
        {
            powerUpHeader.transform.parent.gameObject.SetActive(isOn);

            PayType = payType;       
            Price  =  isOn? price : 0;
            priceText.text = Price != 0 ? $"-{price}" : $"{0}";

            if (!isOn)
                return;

            powerUpHeader.text      = header;
            powerUpDescription.text = description;
        }

        private void OnPlay()
        {
            if (globalData.CanSpendGem(PayType,Price))
                fader.FadeOut(FadeIn);
            else
            {
                system.GemType = PayType;
                UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
            }
        }

        private void FadeIn()
        {      
            prepareDisplay.SetActive(false);
            gameDisplay.SetActive(true);
            gsm.ActivateGameElement();         
            fader.FadeIn(OnStarter);
        }
        void OnStarter()
        {
            UIManager.Instance.OpenMenu(Menu.Menus.Starter);
        }
    

    }
    public enum PowerUp
    {
        Time,
        Speed,
        JumpCount,
        Energy,
        Mana
    }
}

