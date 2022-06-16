using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
        private List<StateUIView> stats;
        [SerializeField]
        private float endPos;

        [SerializeField]
        private GameObject buttonGroup;

        private Animator animator;
        private Transform playerT;
        private float originalPosY;

        int idleParamId;
        SystemManager system;
  
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
            bool islock = Index > globalData.PlayerDatas.GetAllCharacterCount-1;
            SetButtons(islock);

            base.Start();

            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].SetStatName(((CharacterProperty)i).ToString());
                stats[i].SetInfoSlider(globalData.GetCurrentCharacterData().GetCharacterProperty((CharacterProperty)i)*10, 100);
            }
        }
        public override void Move(bool onClick, int amount)
        {
            Index += amount;
            Index = Mathf.Clamp(Index, 0, Count-1);

            bool islock = globalData.PlayerDatas.Characters[Index].isLock;

            if (!islock)
                globalData.SetCharacterIndex(Index);

            Color endColor = system.GetBlackAlfaColor(islock);
            StartCoroutine(BlackOut(endColor));
            StartCoroutine(Moving(endPos));

            prefab.SetSkin(Index);
            SetMoveButton();
            SetButtons(islock);

            CharacterData data = !islock ? globalData.GetCurrentCharacterData() : defaultData.CharacterDatas[Index];

            if (islock)
            {
                purchaseButton.OnClick(Index, Purchase);
                purchaseButton.buttonName.text  = system.StringFormat(data.price);
                purchaseButton.priceIcon.sprite = system.GetPaySprite(data.payType);
            }

            for (int i = 0; i < stats.Count; i++)
                stats[i].SetInfoSlider(data.GetCharacterProperty((CharacterProperty)i)*10, 100);

        }
        IEnumerator BlackOut(Color endColor)
        {
            float time = 0;
            Color currentColor = block.color;

            while (time <= 1)
            {
                time += Time.deltaTime / duration;
                Color color = Color.Lerp(currentColor, endColor, time);
                block.color = color;
                yield return null;
            }
        }
        IEnumerator Moving(float endPosY)
        {
            float time = 0;
            float currentPosY = endPosY;


            animator.SetBool(idleParamId,false);

            while (time <= 1)
            {
                time += Time.deltaTime / (duration*0.25f);
                float posY = Mathf.Lerp(currentPosY, originalPosY, time);
                playerT.position = new Vector2(playerT.position.x,posY);
                yield return null;
            }

            animator.SetBool(idleParamId,true);
        }
        private void Purchase(int index)
        {
            CharacterData data = defaultData.CharacterDatas[index];

            if (globalData.CanSpendGem(data.payType, data.price))
            {
                data.isLock = false;
                globalData.SetCharacterData(Index,data);
                globalData.SetCharacterIndex(Index);
                globalData.SpendGem(data.payType, data.price);
          
                buttonGroup.SetActive(true);
                purchaseButton.gameObject.SetActive(false);
                stageButton.button.interactable = true;

                StartCoroutine(BlackOut(system.GetBlackAlfaColor(data.isLock)));       
            }
            else
            {
                system.GemType = data.payType;
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
