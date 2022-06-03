using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy
{
    public class StateUIView : MonoBehaviour
    {
        [SerializeField]
        private State state;
        [SerializeField]
        private TMP_Text amount;
        [SerializeField]
        private Slider stateSlider;

        private bool init = true;


        private void Start()
        {
            UIManager.Instance.updateState += UpdateState;
        }

        public virtual void UpdateState(State state,int amount)
        {
            if (this.state != state)
                return;

            if(this.amount !=null)
                this.amount.text = amount.ToString();

            if (stateSlider != null)
            {
                stateSlider.value = amount;

                if (!init)
                    return;

                stateSlider.maxValue = amount;
                init = false;
            }
        }
    }

    public enum State
    {
        Timer,
        Gold,
        Energy,
        Key
    }

}

