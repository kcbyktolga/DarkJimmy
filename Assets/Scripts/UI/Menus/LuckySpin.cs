using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private float speed;
        [SerializeField]
        private int speedMultiple;
        [SerializeField]
        private float effectDuration;

        [SerializeField]
        private Vector2 duration = new Vector2(1f, 3f);
      
        [SerializeField]
        private Color onColor;

        [SerializeField]
        private Color offColor;

        [SerializeField]
        private Sprite darkBackground;
        [SerializeField]
        private Sprite lightBackground;

        
        private PayType Paytype { get; set; }
        private int Index { get; set; } = 0;
        private bool IsSpin { get; set; } = false;

        private List<SpinSlot> slots = new List<SpinSlot>();

        private Catalog catalog;
        CloudSaveManager csm;

        public override void Start()
        {
            csm = CloudSaveManager.Instance;
            catalog = csm.GetSystemData().Catalog;
            gemType = GemType.Diamond;


            purchaseButton.OnClick(SpinWheel);
            CreateSlot();
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
        private void SpinWheel()
        {
            if (Paytype.Equals(PayType.Paid))
            {
                if (csm.CanSpendGem(gemType, price))
                {
                    

                    StartCoroutine(Spin());
                }
                else
                {
                    CloudSaveManager.Instance.GemType = gemType;
                    UIManager.Instance.OpenMenu(Menus.ShopOrientation);
                }
            }
            else
            {
                StartCoroutine(Spin());
            }
            
        }
        private void FixedUpdate()
        {
            if (IsSpin)
                return;

            wheel.transform.Rotate(speed*Time.fixedDeltaTime*Vector3.forward);
        }
        private IEnumerator Spin()
        {
            IsSpin = true;
            purchaseButton.button.interactable = !IsSpin;

            int index = GetCurrentIndex() + Random.Range(0, catalog.GetProductLuckySpin.Count);
            float time = 0;

            Vector3 currentAngle = wheel.localEulerAngles;

            float targetZ =  currentAngle.z + index*45 + 360*speedMultiple;
            float _duration = duration.x;
            Vector3 targetAngle = new Vector3(wheel.localEulerAngles.x,wheel.localEulerAngles.y,targetZ);
            bool startAnim = true;

            Debug.Log($"targetZ: {targetZ}");
            Debug.Log($"duration: {_duration}");
            while (time<=1)
            {
                time += Time.fixedDeltaTime /_duration;
                wheel.localEulerAngles = targetAngle*curve.Evaluate(time);

                if(time>=0.2f && time < 0.5f && startAnim)
                {
                    startAnim = false;
                    StartCoroutine(SpinColorEffect(onColor,offColor));                  
                }

                if (time>=0.8f && !startAnim)
                {
                    startAnim = true;
                    StartCoroutine(SpinColorEffect(offColor, onColor));
                }

                yield return null;
            }

            Index = (GetCurrentIndex() + 4) % catalog.GetProductLuckySpin.Count;

            ProductStruct ps = GetSlotProduct();
            Debug.Log($"{ps.typeOfProduct}, x{ps.amount}");

            yield return new WaitForSeconds(5);

            IsSpin = false;
            purchaseButton.button.interactable = !IsSpin;
        }
        private IEnumerator SpinColorEffect(Color startColor, Color endColor)
        {
            float time = 0;

            while (time<=1)
            {
                time += Time.deltaTime / effectDuration;

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
    }

  
    
}

