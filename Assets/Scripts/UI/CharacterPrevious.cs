using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DarkJimmy.UI
{
    public class CharacterPrevious : SlidePage<CharacterSkinSwap,CloudSaveManager>
    {
        [Header("Character Previous")]
        private SystemData systemData;
        [SerializeField]
        private TouchButton stageButton;
        [SerializeField]
        private Image block;
        [SerializeField]
        private Color onColor;
        [SerializeField]
        private Color offColor;
        [SerializeField]
        private List<StateUIView> stats;
        [SerializeField]
        private float endPos;

        private Animator animator;
        private Transform playerT;
        private float originalPosY;

        int idleParamId;
  
        public override void Start()
        {
            base.Start();

            globalData = CloudSaveManager.Instance;
            systemData = globalData.GetSystemData();
            Count = systemData.CharacterDatas.Count;

            prefab = FindObjectOfType<CharacterSkinSwap>();
            animator = prefab.GetComponent<Animator>();
            playerT = prefab.transform;
            originalPosY = playerT.position.y;

            idleParamId = Animator.StringToHash("Idle");

            for (int i = 0; i < stats.Count; i++)
            {
                stats[i].SetStatName(((CharacterProperty)i).ToString());
                stats[i].SetInfoSlider(globalData.GetCurrentCharacterData().GetCharacterProperty((CharacterProperty)i), 10);
            }              
        }

        public override void Move(bool onClick, int amount)
        {
            Index += amount;
            Index = Mathf.Clamp(Index, 0, Count - 1);

            bool islock = Index <= globalData.PlayerDatas.GetAllCharacterCount - 1;

            if (islock)
                globalData.SetCharacter(Index);

            stageButton.button.interactable = islock;

            Color endColor = islock ? onColor : offColor;
            StartCoroutine(BlackOut(endColor));
            StartCoroutine(Moving(endPos));

            prefab.SetSkin(Index);
            SetMoveButton();

            CharacterData data = islock ? globalData.GetCurrentCharacterData() : systemData.CharacterDatas[Index];

            for (int i = 0; i < stats.Count; i++)
                stats[i].SetInfoSlider(data.GetCharacterProperty((CharacterProperty)i), 10);

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
    }

}
