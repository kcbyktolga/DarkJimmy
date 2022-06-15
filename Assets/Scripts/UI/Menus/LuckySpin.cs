using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace DarkJimmy.UI
{
    public class LuckySpin : Menu
    {
        [Header("Lucky Spin")]
        [SerializeField]
        private SpinSlot prefab;
        [SerializeField]
        private PurchaseButton purchaseButton;
        [SerializeField]
        private RectTransform wheel;
        [SerializeField]
        private AnimationCurve curve;
        [SerializeField]
        private GemType gemType;
        [SerializeField]
        private int price;
        [SerializeField]
        private TMP_Text infoText;
        [SerializeField]
        private float speed;
        [SerializeField]
        private int speedMultiple;
        [SerializeField]
        private float effectDuration;
        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;
        [SerializeField]
        private Sprite darkBackground;
        [SerializeField]
        private Sprite lightBackground;
        [SerializeField]
        private int spinDuration = 2;

        private PayType payType;
        private int Index { get; set; } = 0;
        private bool IsSpin { get; set; } = false;

        private List<SpinSlot> slots = new List<SpinSlot>();

        private Catalog catalog;
        private CloudSaveManager csm;
        private RewardType rewardType;
        private Coroutine Timer;
        private DateTime resetTime;

        public override void Start()
        {
            csm = CloudSaveManager.Instance;
            catalog = csm.GetSystemData().Catalog;
            gemType = GemType.Diamond;
            rewardType = RewardType.LuckySpin;

            
            resetTime = csm.GetResetTime(rewardType);
            payType = DateTime.Now < resetTime ? PayType.Paid : PayType.Free;

            if(payType!=PayType.Free)
                StartCoroutine(TimeUpdate());
            else
                infoText.text = LanguageManager.GetText(PayType.Free.ToString());

            CreateSlot();
            purchaseButton.OnClick(SpinWheel);
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
        private void CreateSlot()
        {
            float diffAngle = 360 / catalog.GetProductLuckySpin.Count;
            float angle = 0;

            for (int i = 0; i < catalog.GetProductLuckySpin.Count; i++)
            {
                SpinSlot slot = Instantiate(prefab, wheel);
                slot.SetSlot(catalog.GetProductLuckySpin[i]);

                Sprite sprite = i % 2 == 0 ? lightBackground : darkBackground;
                slot.SlotBackground(sprite);

                slot.transform.eulerAngles = new Vector3(0, 0, angle);
                angle += diffAngle;

                slots.Add(slot);
            }
        }
        private void FreeSpin(RewardType type)
        {
            StartCoroutine(Spin());

            resetTime = DateTime.Now.AddMinutes(1);
            csm.SetResetTime(resetTime,rewardType);
            payType = PayType.Paid;
        }
        private void SpinWheel()
        {
            if (payType.Equals(PayType.Paid))
            {
                if (csm.CanSpendGem(gemType, price))
                {
                    csm.SpendGem(gemType,price);
                    StartCoroutine(Spin());
                }
                else
                {
                    CloudSaveManager.Instance.GemType = gemType;
                    UIManager.Instance.OpenMenu(Menus.ShopOrientation);
                }
            }
            else
                GoogleAdsManager.Instance.ShowRewardedAd(rewardType, FreeSpin);

        }
        private void FixedUpdate()
        {
            if (IsSpin)
                return;

            wheel.transform.Rotate(speed*Time.fixedDeltaTime*Vector3.forward);
        }
        private IEnumerator Spin()
        {
            IsSpin = !IsSpin;
            purchaseButton.button.interactable = !IsSpin; 
            float time = 0;
            int index = GetCurrentIndex() + UnityEngine.Random.Range(0, catalog.GetProductLuckySpin.Count);
            float targetZ = wheel.localEulerAngles.z + index*45 + 360*spinDuration*24;
            Vector3 targetAngle = new Vector3(wheel.localEulerAngles.x,wheel.localEulerAngles.y,targetZ);
            bool startColorAnim = true;
            infoText.text = LanguageManager.GetText("Spining");
            while (time<=1)
            {
                time += Time.fixedDeltaTime /spinDuration;
                wheel.localEulerAngles = targetAngle*curve.Evaluate(time);

                if(time>=0.3f && time < 0.5f && startColorAnim)
                {
                    startColorAnim = false;
                    StartCoroutine(SpinColorEffect(onColor,offColor));                  
                }

                if (time>=0.7f && !startColorAnim)
                {
                    startColorAnim = true;
                    StartCoroutine(SpinColorEffect(offColor, onColor));
                }

                yield return null;
            }
 
            Index = (GetCurrentIndex() + 4) % catalog.GetProductLuckySpin.Count;

           // ProductStruct ps = GetSlotProduct();

            if (payType != PayType.Free)
                StartCoroutine(TimeUpdate());

            yield return new WaitForSeconds(5);

            IsSpin = !IsSpin;
            purchaseButton.button.interactable =!IsSpin;

        }
        private IEnumerator SpinColorEffect(Color startColor, Color endColor)
        {
            float time = 0;

            while (time<=1)
            {
                time += Time.deltaTime/0.25f;

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
            DateTime resetTime = csm.GetResetTime(rewardType);

            while (DateTime.Now <= resetTime)
            {         
                TimeSpan diffTime = resetTime.Subtract(DateTime.Now);

               string timer = diffTime.Hours > 0 ? $"{diffTime.Hours}{LanguageManager.GetText("sa")} : {diffTime.Minutes}{LanguageManager.GetText("d")}" : $"{diffTime.Minutes}{LanguageManager.GetText("dk")} : {diffTime.Seconds}{LanguageManager.GetText("sn")}";

                infoText.text = $"{LanguageManager.GetText("Remaining")}: {timer}";

                yield return new WaitForSeconds(1);
            }
            payType = PayType.Free;
            infoText.text = LanguageManager.GetText(PayType.Free.ToString());
        }
        private int GetCurrentIndex()
        {
            float zAngle = wheel.transform.eulerAngles.z;

            float absAngle = Mathf.Abs(zAngle % 360);

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
        private ProductStruct GetSlotProduct()
        {
            return catalog.GetProductLuckySpin[Index];
        }

        private void OnDestroy()
        {
            StopTimer();
        }
    }

  
    
}

