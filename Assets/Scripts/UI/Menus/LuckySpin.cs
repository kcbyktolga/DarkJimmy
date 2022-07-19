using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace DarkJimmy.UI
{
    public class LuckySpin : Menu
    {
        [Header("Lucky Spin")]
        [SerializeField]
        private RewardViewUI prefab;
        [SerializeField]
        private PurchaseButton purchaseButton;
        [SerializeField]
        private RectTransform wheel;
        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        private Ease ease;
        [SerializeField]
        private GemType gemType;        
        [SerializeField]
        private TMP_Text infoText;
        [SerializeField]
        private TMP_Text spinText;
        [SerializeField]
        private float speed;
        [SerializeField]
        private int speedMultiple;
        [SerializeField]
        private float effectDuration;     
        [SerializeField]
        private int spinDuration = 2;
        [SerializeField]
        private GameObject paidGroup;
        [SerializeField]
        private GameObject freeGroup;

        [SerializeField]
        private Image payIcon;
        [SerializeField]
        private TMP_Text priceText;

        //[SerializeField]
        //private RewardViewUI rewardPopup;
        //[SerializeField]
        //private BaseButton claimButton;

        private int Price { get; set; }
        private int Index { get; set; } = 0;
        private int Seed { get { return UnityEngine.Random.Range(0, 100);} }
        private bool IsSpin { get; set; } = false;

        private readonly List<RewardViewUI> slots = new List<RewardViewUI>();
      //  private List<int> luckFactor = new List<int>();
     
        private Catalog catalog;
        private CloudSaveManager csm;
        private SystemManager system;
        private RewardType rewardType;
        private Coroutine Timer;
        private DateTime resetTime;
        private ProductPayType payType;
        //private RewardProduct luckyProduct;
        public override void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;
            catalog = csm.GetDefaultData().Catalog;
            gemType = (GemType)csm.SpinPayType;
            Price = csm.SpinPrice;
            rewardType = RewardType.LuckySpin;

            //claimButton.OnClick(ClaimReward);

           // claimButton.SetTabButtonName(LanguageManager.GetText("Claim"));
            spinText.text = LanguageManager.GetText("SpinNow");

            priceText.text = Price.ToString();
            payIcon.sprite = system.GetPaySprite(gemType);

            payType = !IsTimeOut() ? ProductPayType.Paid : ProductPayType.Free;

            if (payType != ProductPayType.Free)
                StartTimer();
            else
                infoText.text = string.Empty;

            SetSpinButton();

            CreateSlot();
            //ShuffledElement();
            purchaseButton.OnClick(SpinWheel);
        }
        private void CreateSlot()
        {
            float diffAngle = 360 / catalog.GetProductLuckySpin.Count;
            float angle = 0;

            for (int i = 0; i < catalog.GetProductLuckySpin.Count; i++)
            {
                RewardProduct lp = catalog.GetProductLuckySpin[i];
                RewardViewUI slot = Instantiate(prefab, wheel);
                slot.SetSlot(lp);

                //Sprite sprite = system.GetProductBackground(lp.typeOfProduct); 
                //slot.SlotBackground(sprite);

                slot.transform.eulerAngles = new Vector3(0, 0, angle);
                angle += diffAngle;

                slots.Add(slot);

                //int factor = lp.luckyFactor;

                //for (int j = 0; j < factor; j++)
                //    luckFactor.Add(i);
            }
        }
 
        private void StartTimer()
        {
            if (Timer == null)
                Timer = StartCoroutine(nameof(TimeUpdate));
        }
        private void StopTimer()
        {
            StopCoroutine(nameof(TimeUpdate));
            Timer = null;
        }
        private void FreeSpin(RewardType type)
        {
            StartCoroutine(Spin());

            resetTime = DateTime.Now.AddMinutes(csm.RewardAdFrequency);
            LocalSaveManager.Save(rewardType.ToString(),resetTime);
            //csm.SetResetTime(resetTime,rewardType);
            payType = ProductPayType.Paid;
        }
        private void SpinWheel()
        {
            if (payType.Equals(ProductPayType.Paid))
            {
                if (csm.CanSpendGem(gemType, Price))
                {
                    csm.SpendGem(gemType, Price);
                    StartCoroutine(Spin());
                    
                }
                else
                {
                    UIManager.Instance.PageIndex = (int)gemType;
                    UIManager.Instance.OpenMenu(Menus.ShopOrientation);
                }
            }
            else
                AdManager.Instance.ShowRewardedAd(rewardType, FreeSpin);
        }
        private void FixedUpdate()
        {
            if (IsSpin)
                return;

            wheel.transform.Rotate(speed*Time.fixedDeltaTime*Vector3.forward);
        }
        public override void ScaleAnimation()
        {
            base.ScaleAnimation();
            AudioManager.Instance.PlaySound("Open Page");
        } 
        private IEnumerator Spin()
        {
            IsSpin = !IsSpin;
            purchaseButton.button.interactable = !IsSpin;
            //AudioManager.Instance.FadeVolume0(false);
            AudioManager.Instance.PauseSources();
            float time = 0;
      
            Vector3 startAngle = wheel.localEulerAngles;

            float targetZ = GetTargetStep(system.GetRandomRewardIndex())*45 + 360*spinDuration*24;

             Index = (GetIndex(startAngle.z + targetZ)+3)% catalog.GetProductLuckySpin.Count;

            system.Reward = catalog.GetProductLuckySpin[Index];
            Vector3 targetAngle = new Vector3(startAngle.x,startAngle.y,targetZ);
            bool startColorAnim = true;
            infoText.text = LanguageManager.GetText("Spinning");
            AudioManager.Instance.PlaySound("Spin Wheel");
           
            while (time<=1)
            {
                time += Time.fixedDeltaTime /spinDuration;
                Vector3 currentAngle = targetAngle * curve.Evaluate(time);

                wheel.localEulerAngles = currentAngle+startAngle;

                if (time>=0.4f && time < 0.5f && startColorAnim)
                {
                    startColorAnim = false;
                    StartCoroutine(SpinColorEffect(!startColorAnim));                  
                }

                if (time>=0.6f && !startColorAnim)
                {
                    startColorAnim = true;
                    StartCoroutine(SpinColorEffect(!startColorAnim));
                }
                yield return null;
            }
           
            // ShuffledElement();

            if (payType != ProductPayType.Free)
                StartCoroutine(TimeUpdate());

            yield return new WaitForSeconds(1.5f);
            // rewardPopup.gameObject.SetActive(true);
            UIManager.Instance.OpenMenu(Menus.RewardPopup);
            IsSpin = !IsSpin;
            purchaseButton.button.interactable =!IsSpin;
        }

        private IEnumerator SpinColorEffect(bool isOn)
        {
            float time = 0;

            Color startColor = SystemManager.Instance.GetWhiteAlfaColor(isOn);
            Color endColor = SystemManager.Instance.GetWhiteAlfaColor(!isOn);

            while (time<=1)
            {
                time += Time.deltaTime/0.1f;

                Color color0 = Color.Lerp(startColor,endColor,time);
                Color color1 = Color.Lerp(endColor, startColor, time);

                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].slotIcon[0].color = color0;
                    slots[i].slotIcon[1].color = color1;

                    slots[i].text[0].color = color0;
                    slots[i].text[1].color = color1;
                }

                yield return null;
            }
        }
        private IEnumerator TimeUpdate()
        {
            SetSpinButton();

            while (!IsTimeOut())
            {         
                TimeSpan diffTime = LocalSaveManager.GetResetTime(rewardType.ToString()).Subtract(DateTime.Now);

               string timer = diffTime.Hours > 0 ? $"{diffTime.Hours}{LanguageManager.GetText("sa")} : {diffTime.Minutes}{LanguageManager.GetText("dk")}" : $"{diffTime.Minutes}{LanguageManager.GetText("dk")} : {diffTime.Seconds}{LanguageManager.GetText("sn")}";

                if(!IsSpin)
                    infoText.text = $"{LanguageManager.GetText("Remaining")}: {timer}";

                yield return new WaitForSeconds(1);
            }
            payType = ProductPayType.Free;
            infoText.text = string.Empty;

            SetSpinButton();
        }
        private int GetIndex(float angle)
        {
            float absAngle = Mathf.Abs(angle % 360);

            if (absAngle < 22.5f)
                return 0;
            else if (absAngle >= 22.5f && absAngle < 67.5f)
                return 7;
            else if (absAngle >= 67.5f && absAngle < 112.5f)
                return 6;
            else if (absAngle >= 112.5f && absAngle < 157.5f)
                return 5;
            else if (absAngle >= 157.5f && absAngle < 202.5f)
                return 4;
            else if (absAngle >= 202.5f && absAngle < 247.5f)
                return 3;
            else if (absAngle >= 247.5f && absAngle < 292.5f)
                return 2;
            else if (absAngle >= 292.5f && absAngle < 337.5f)
                return 1;
            else
                return 0;
        }
        private int GetTargetStep(int index)
        {  
            int diff = GetIndex(wheel.localEulerAngles.z)-5 - index;
            int step = diff % catalog.GetProductLuckySpin.Count;
            return step;
        }
        private bool IsTimeOut()
        {
           return  DateTime.Now >= LocalSaveManager.GetResetTime(rewardType.ToString());
        }
     
        private void SetSpinButton()
        {
            freeGroup.SetActive(IsTimeOut());
            paidGroup.SetActive(!IsTimeOut());
        }

        
        public override void OnDestroy()
        {
            if (IsSpin)
            {
                UIManager.Instance.OpenMenu(Menus.RewardPopup);
                //  system.ClaimReward();
                //AudioManager.Instance.FadeVolume0(true);
            }
        
            SetVolume(true);
            StopTimer();
        }
        public override void OnEnable()
        {
            base.OnEnable();
            SetVolume(false);
        }
    }
}

