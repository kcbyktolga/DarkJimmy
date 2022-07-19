using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class RewardPopup : Popup
    {
        [SerializeField]
        private BaseButton claim;
        [SerializeField]
        private BaseButton claimx2;
        [SerializeField]
        private RewardViewUI rewardUI;
        [SerializeField]
        private RectTransform buttonGroup;

        public override void Start()
        {
            base.Start();

            SystemManager.Instance.ClaimReward();
            rewardUI.SetSlot(SystemManager.Instance.Reward);

            claim.SetTabButtonName("Claim");
            claimx2.SetTabButtonName("WatchWin");

            claim.OnClick(LoadToScene);
            claimx2.OnClick(() => AdManager.Instance.ShowRewardedAd(RewardType.DoubleReward,ClaimDoubleReward));
            claimx2.gameObject.SetActive(AdManager.Instance.RewardAdReady(RewardType.DoubleReward));

            Invoke(nameof(ScaleAnimation),Duration*10);
        }
      
        private void LoadToScene()
        {
            if (Fade.Instance != null)
                Fade.Instance.FadeOut(() => SceneManager.LoadScene(Menus.Stages.ToString()),null);
            else
                GoBack();

            AudioManager.Instance.PlaySources();


        }
        private void ClaimDoubleReward(RewardType reward)
        {
            //Do something..
            SystemManager.Instance.ClaimReward();
            LoadToScene();
        }

        public override void ScaleAnimation()
        {
            if (baseTransform == null)
                return;
            AudioManager.Instance.PlaySound("Reward");
            baseTransform.localScale = 0.95f * Vector2.one;
            baseTransform.gameObject.SetActive(true);
            baseTransform.DOScale(Vector2.one, Duration).SetEase(SystemManager.Instance.GetMenuCurve()).OnComplete(AfterScaleAnimation);
        }

        public override void AfterScaleAnimation()
        {
            base.AfterScaleAnimation();
            buttonGroup.localScale = 0.95f * Vector2.one;
            buttonGroup.gameObject.SetActive(true);
            buttonGroup.DOScale(Vector2.one, Duration).SetEase(SystemManager.Instance.GetMenuCurve()).OnComplete(()=> buttonGroup.DOKill());
        }
        public override void OnEnable()
        {
          
        }
    }
}

