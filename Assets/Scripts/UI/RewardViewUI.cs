using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class RewardViewUI : MonoBehaviour,IAnimationEvent
    {     
        public List<Image> slotIcon;
        public Image slotBackground;
        public List<TMP_Text> text;

        private Animator animator;

        bool hasAnimator = false;

        private void Awake()
        {
            animator = transform.GetComponent<Animator>();
            hasAnimator = animator != null;
        }
 
        public void AnimationEvent()
        {
            animator.enabled = false;
        }

        public void SetSlot(RewardProduct lp)
        {
            for (int i = 0; i < lp.productIcon.Count; i++)
                slotIcon[i].sprite = lp.productIcon[i];

            text[0].text = $"{SystemManager.Instance.StringFormat(lp.amount)} {(lp.typeOfProduct.Equals(TypeofProduct.Stones)? LanguageManager.GetText(lp.stoneType.ToString()):LanguageManager.GetText(lp.typeOfProduct.ToString()))}";

            slotBackground.sprite = SystemManager.Instance.GetProductBackground(lp.typeOfProduct);
        }
        public void SlotBackground(Sprite sprite)
        {
            slotBackground.sprite = sprite;
        }

        private void OnDisable()
        {
            if (hasAnimator)
                animator.enabled = true;
        }
        private void OnEnable()
        {
            if (hasAnimator)
                AudioManager.Instance.PlaySound("Reward");
        }
    }

}


