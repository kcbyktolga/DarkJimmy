using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkJimmy.UI
{
    public class SpinSlot : MonoBehaviour,IAnimationEvent
    {     
        public List<Image> slotIcon;
        public Image slotBackground;
        public List<TMP_Text> text;

        public void AnimationEvent()
        {
            transform.GetComponent<Animator>().enabled = false;
        }

        public void SetSlot(LuckyProduct ps)
        {
            for (int i = 0; i < ps.productIcon.Count; i++)
                slotIcon[i].sprite = ps.productIcon[i];

            text[0].text = $"{SystemManager.Instance.StringFormat(ps.amount)} {LanguageManager.GetText(ps.typeOfProduct.ToString())}";
        }
        public void SlotBackground(Sprite sprite)
        {
            slotBackground.sprite = sprite;
        }
    }
}

