using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace DarkJimmy.UI
{
    public class Defeat : Menu
    {
        [SerializeField]
        private BaseButton priceButton;
        [SerializeField]
        private BaseButton rewardButton;
        [SerializeField]
        private BaseButton restartButton;
        [SerializeField]
        private BaseButton stagesButton;

        [SerializeField]
        private Image priceIcon;
        [SerializeField]
        private Image rewardIcon;
        [SerializeField]
        private TMP_Text priceText;
        [SerializeField]
        private TMP_Text rewardText;
        [SerializeField]
        private TMP_Text continueText;
        [SerializeField]
        private TMP_Text stagesText;
        [SerializeField]
        private TMP_Text restartText;
        [SerializeField]
        private GameObject continuePanel;

        private CloudSaveManager csm;
        private GameSaveManager gsm;
        private SystemManager system;
        private AdManager adManager;
        private Fade fade;
        private GemType payType;
        private int price;
        private RewardType rewardType;
        public override void Start()
        {
            base.Start();
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;
            gsm = GameSaveManager.Instance;
            adManager = AdManager.Instance;
            fade = Fade.Instance;


            rewardType = RewardType.EndGame;
            price = csm.EndGamePrice;
            payType = (GemType)csm.EndGamePayType;

            priceButton.OnClick(Pay);
            rewardButton.OnClick(ShowRewardAd);
            restartButton.OnClick(Restart);
            stagesButton.OnClick(LoadStages);

            bool isOn = adManager.RewardAdReady(rewardType);
            continuePanel.SetActive(gsm.AnyReachedCheckPoint());
            rewardButton.button.interactable = isOn;

            string rewardButtonName = isOn ? "Free" : "Not avaýlable";
            rewardText.text = LanguageManager.GetText(rewardButtonName);
            continueText.text = LanguageManager.GetText("Continue");
            restartText.text = LanguageManager.GetText("Restart");
            stagesText.text = LanguageManager.GetText(Menus.Stages.ToString());

            priceText.text = system.StringFormat(price);
            priceIcon.sprite = system.GetPaySprite(payType);
            rewardIcon.sprite = system.GetRewardAdSprite(isOn);
        }

        private void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveSceneName());
        }
        private void LoadStages()
        {
            SceneManager.LoadScene(Menus.Stages.ToString());
        }
        private void ShowRewardAd()
        {
            AdManager.Instance.ShowRewardedAd(rewardType,Continue);
        }
     
        private void Pay()
        {
            if (csm.CanSpendGem(payType, price))
            {
                csm.SpendGem(payType, price);
                Continue(rewardType);
            }
            else
            {
                UIManager.Instance.PageIndex = (int)payType;
                UIManager.Instance.OpenMenu(Menus.ShopOrientation);
            }
        }
        private void Continue(RewardType reward)
        {
            fade.FadeOut(FadeIn,null);
        }
        
        private void FadeIn()
        {
            UIManager.Instance.GoBack();
            gsm.ToRewindElements();
            fade.FadeIn(()=> UIManager.Instance.OpenMenu(Menus.TapToStart),null);
        }

    }
}

