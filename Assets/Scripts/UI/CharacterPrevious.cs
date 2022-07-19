using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;


namespace DarkJimmy.UI
{
    public class CharacterPrevious : SlidePage<CharacterSkinSwap,CloudSaveManager>
    {
        [Header("Character Previous")]
        private DefaultData defaultData;
        [SerializeField]
        private TouchButton stageButton;
        [SerializeField]
        private PurchaseButton purchaseButton;
        [SerializeField]
        private Image block;

        [SerializeField]
        private List<StatsUISlider> stats;
        [SerializeField]
        private float endPos;

        [SerializeField]
        private GameObject buttonGroup;

        private Animator animator;
        private Transform playerT;
        private float originalPosY;

        int idleParamId;
        private SystemManager system;
    
        public override void Start()
        {
            system = SystemManager.Instance;
            globalData = CloudSaveManager.Instance;
            defaultData = globalData.GetDefaultData();
            Count = defaultData.CharacterDatas.Count;
            prefab = FindObjectOfType<CharacterSkinSwap>();
            animator = prefab.GetComponent<Animator>();
            playerT = prefab.transform;
            originalPosY = playerT.position.y;
            idleParamId = Animator.StringToHash("Idle");

            Index = globalData.GetCurrentCharacterIndex();
            SetButtons(false);

            base.Start();

            SetStats(globalData.GetCurrentCharacterData());
        }
        public override void Move(bool onClick, int amount)
        {
            Index += amount;
            Index = Mathf.Clamp(Index, 0, Count-1);

            bool islock = globalData.PlayerDatas.Characters[Index].isLock;

            if (!islock)
                globalData.SetCharacterIndex(Index);

            Dimed(islock);
            //Color endColor = system.GetBlackAlfaColor(islock);
            // StartCoroutine(BlackOut(endColor));
            // block.DOColor(endColor, duration);
            //StartCoroutine(Moving(endPos));
            Jumping();

            prefab.SetSkin(Index);
            SetMoveButton();
            SetButtons(islock);

            CharacterData data = !islock ? globalData.PlayerDatas.Characters[Index] : defaultData.CharacterDatas[Index];

            if (islock)
            {
                purchaseButton.OnClick(Purchase);
                purchaseButton.buttonName.text  = system.StringFormat(data.price);
                purchaseButton.priceIcon.sprite = system.GetPaySprite(data.payType);
            }

            //for (int i = 0; i < stats.Count; i++)
            //    stats[i].SetSliderValues(data.GetCharacterProperty((CharacterProperty)i),10);

            SetStats(data);
        }

        private void SetStats(CharacterData data)
        {
            //for (int i = 0; i < stats.Count; i++)
            //{
            //    stats[i].SetSliderValues(data.GetCurrentCharacterProperty((CharacterProperty)i), data.GetMaxCapacity((CharacterProperty)i));

            //    if (init)
            //        continue;

            //    stats[i].SetStatName(((CharacterProperty)i).ToString());
            //}

            for (int i = 0; i < Enum.GetNames(typeof(CharacterProperty)).Length; i++)
            {
                if (Enum.TryParse(((CharacterProperty)i).ToString(), out Stats stats))
                    system.updateStatsMax(stats, data.GetCurrentCharacterProperty((CharacterProperty)i), data.GetMaxCapacity((CharacterProperty)i));
            }
        }
        private void Dimed(bool isOn)
        {
           // block.color = system.GetBlackAlfaColor(!isOn);
            Color endColor = system.GetBlackAlfaColor(isOn);
            block.DOColor(endColor, duration);
        }

        private void Jumping()
        {
            playerT.position = new Vector2(playerT.position.x, originalPosY);
            animator.SetBool(idleParamId, false);
            AudioManager.Instance.PlaySound("Jump");

            playerT.DOJump(playerT.position, 2, 1, duration * 0.5f).OnComplete(OnIdle);
                
        }

        private void OnIdle()
        {
            animator.SetBool(idleParamId, true);
            playerT.position = new Vector2(playerT.position.x,originalPosY);

        }
        private void Purchase()
        {
            CharacterData data = defaultData.CharacterDatas[Index];

            if (globalData.CanSpendGem(data.payType, data.price))
            {
                globalData.SetCharacterData(Index,data);
                globalData.SetCharacterIndex(Index);
                globalData.PlayerDatas.Characters[Index].isLock = false;
                globalData.SpendGem(data.payType, data.price);

                //buttonGroup.SetActive(true);
                //purchaseButton.gameObject.SetActive(false);
                //stageButton.button.interactable = true;

                //StartCoroutine(BlackOut(system.GetBlackAlfaColor(data.isLock)));       

                Move(true,0);
            }
            else

            {
                //system.GemType = data.payType;
                UIManager.Instance.PageIndex = (int)data.payType;
                UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                
            }
        }
        private void SetButtons(bool islock)
        {
            stageButton.button.interactable = !islock;
            purchaseButton.gameObject.SetActive(islock);
            buttonGroup.SetActive(!islock);
        }
    }

}
