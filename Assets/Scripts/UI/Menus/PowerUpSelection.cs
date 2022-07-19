using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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
        [SerializeField]
        private TMP_Text playButtonName;
        [SerializeField]
        private TMP_Text pageText;
        [SerializeField]
        private BaseButton rewardButton;
        [SerializeField]
        private TMP_Text rewardText;
        [SerializeField]
        private TMP_Text watchWinText;
        [SerializeField]
        private Image rewardIcon;
        [SerializeField]
        private Image priceTextBG;

        [Header("Display")]
        [SerializeField]
        private GameObject gameDisplay;
        [SerializeField]
        private GameObject prepareDisplay;
   
        private Catalog catalog;
        private SystemManager system;
        private GameSaveManager gsm;
        private AdManager adManager;
        private Fade fade;

        private GemType PayType { get; set; }      
        private int Price { get; set; }

        private bool HasReward { get; set; } = false;
        public bool HasItem { get; set; } = false;

        private void Start()
        {
            system = SystemManager.Instance;
            globalData = CloudSaveManager.Instance;
            adManager = AdManager.Instance;
            catalog = globalData.GetDefaultData().Catalog;
            gsm = GameSaveManager.Instance;
            fade = Fade.Instance;

            AudioManager.Instance.PlayMusic("Power Up Theme");
            pageText.text = LanguageManager.GetText("ChosePower");
            playButtonName.text = LanguageManager.GetText("Start");
            watchWinText.text = LanguageManager.GetText("WatchWin");

            bool isOn = adManager.RewardAdReady(RewardType.StartGame);         
            rewardButton.button.interactable = isOn;
            string rewardButtonName = isOn ? "PowerUpFree" : "Not avaýlable";
            rewardText.text = LanguageManager.GetText(rewardButtonName);
            rewardIcon.sprite = system.GetRewardAdSprite(isOn);

            rewardButton.OnClick(ShowRewardedAd);
            playButton.OnClick(OnPlay);

            Generate();


            adManager.onClosed += FadeOut;
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

            HasItem = isOn;

            SetPricetTextboard();
        }

        private void ShowRewardedAd()
        {
            if (!HasReward)
                adManager.ShowRewardedAd(RewardType.StartGame, TakeReward);
        }
        private void SetPricetTextboard()
        {

            priceText.text = HasItem && HasReward ? $"<color=green>{LanguageManager.GetText("Free")}</color>" :( Price>0?$"<color=red>{-Price}</color>": $"<color=white>{Price}</color>");

            Color endColor = system.GetWhiteAlfaColor(HasItem);
            priceTextBG.DOColor(endColor,duration);
            priceText.DOColor(endColor, duration);

            float anchorPosY = HasItem ? 520f : 645f;
            priceTextBG.rectTransform.DOAnchorPosY(anchorPosY,duration);
        }
        public void SetPowerUpInformation(string description, string header, bool isOn,GemType payType, int price )
        {
            powerUpHeader.transform.parent.gameObject.SetActive(isOn);

            PayType = payType;       
            Price  =  isOn? price : 0;
            //priceText.text = Price != 0 ? $"-{price}" : $"{0}";

            if (!isOn)
                return;

            powerUpHeader.text      = LanguageManager.GetText(header);
            powerUpDescription.text = LanguageManager.GetText(description);
        }

        private void OnPlay()
        {
            if (HasItem&&!HasReward)
            {
                if (globalData.CanSpendGem(PayType, Price))
                    globalData.SpendGem(PayType, Price);
                else
                {
                    UIManager.Instance.PageIndex= (int)PayType;
                    UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                    return;
                }
            }
            //After watching the interstitial, the fade will close and the game will begin.
            adManager.ShowInterstitial();
        }

        private void TakeReward(RewardType reward)
        {
            HasReward = true;
            SetPricetTextboard();
        }
        private void FadeOut()
        {
            fade.FadeOut(FadeIn,null);
            adManager.onClosed -= FadeOut;
        }
        private void FadeIn()
        {      
            prepareDisplay.SetActive(false);
            gameDisplay.SetActive(true);
            gsm.ActivateGameElement();

            Invoke(nameof(Wait), 1.5f);
        }

        private void Wait() { fade.FadeIn(() => UIManager.Instance.OpenMenu(Menu.Menus.TapToStart), null); }
    }
    public enum PowerUp
    {
        Time,
        Speed,
        JumpCount,
        HP,
        Mana
    }
}

