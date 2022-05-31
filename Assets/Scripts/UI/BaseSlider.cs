using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class BaseSlider : MonoBehaviour
    {
        [Header("Slider Property")]
        [SerializeField]
        private Image sliderIcon;
        [SerializeField]
        private Sprite onIcon;
        [SerializeField]
        private Sprite offIcon;
        [SerializeField]
        private Button next;
        [SerializeField]
        private Button previous;
        [SerializeField]
        private List<GameObject> bloks;

        int volumeIndex = 9;
        int VolumeIndex { 
            get { return volumeIndex; } 
            set
            {
                volumeIndex = value;
                sliderIcon.sprite = volumeIndex < 0 ? offIcon : onIcon;
            } 
        }

        private void Start()
        {
            next.onClick.AddListener(Increase);
            previous.onClick.AddListener(Decrease);

            for (int i = -1; i < VolumeIndex; i++)
                bloks[i+1].SetActive(true);

        }

        private void Increase()
        {
            if (VolumeIndex >= bloks.Count-1)
                return;

            bloks[++VolumeIndex].SetActive(true);
        }
        private void Decrease()
        {
            if (VolumeIndex <0)
                return;

            bloks[VolumeIndex--].SetActive(false);

        }

        private void SetIcon()
        {
            sliderIcon.sprite = VolumeIndex < 0 ? offIcon : onIcon;
        }
    }
}

