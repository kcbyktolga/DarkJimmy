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
        private VolumeType volumeType;       
        [SerializeField]
        private Image sliderIcon;
        [SerializeField]
        private Sprite onIcon;
        [SerializeField]
        private Sprite offIcon;
        [SerializeField]
        private Button toggle;
        [SerializeField]
        private Button next;
        [SerializeField]
        private Button previous;
        [SerializeField]
        private List<GameObject> bloks;
   
        int volumeIndex;
        bool isOn;
        int VolumeIndex { 
            get { return volumeIndex; } 
            set
            {
                volumeIndex = value;
                LocalSaveManager.Save(LocalSaveManager.GetSliderName(volumeType), volumeIndex);               
               // SetIcon(!(volumeIndex < 0));
                if (volumeIndex < 0 && IsOn)
                    IsOn = false;
            } 
        }
        bool IsOn
        {
            get { return isOn; }
            set
            {
                isOn = value;
                LocalSaveManager.Save(LocalSaveManager.GetToggleName(volumeType), isOn);
                SetIcon(isOn);
            }
        }
        private void Start()
        {
            toggle.onClick.AddListener(SetToggle);
            next.onClick.AddListener(Increase);
            previous.onClick.AddListener(Decrease);

            VolumeIndex=LocalSaveManager.GetIntValue(LocalSaveManager.GetToggleName(volumeType), bloks.Count -1);
            IsOn = LocalSaveManager.GetBoolValue(LocalSaveManager.GetToggleName(volumeType), true);
            
            if (IsOn)
                SetSlider(VolumeIndex,true);

        }
        private void SetSlider(int count, bool isOn)
        {
            for (int i = -1; i < count; i++)
                bloks[i + 1].SetActive(isOn);
        }
        private void SetToggle()
        {
            if (VolumeIndex < 0)
                VolumeIndex = bloks.Count - 1;
            
            IsOn = !IsOn;

            int count = IsOn ? VolumeIndex : bloks.Count-1;

            SetSlider(count, IsOn);
        }
        private void Increase()
        {
            if (!IsOn)
                SetToggle();

            if (VolumeIndex >= bloks.Count - 1)
                return;

            bloks[++VolumeIndex].SetActive(true);
        }
        private void Decrease()
        {
            if (VolumeIndex < 0)
                return;

            if (!IsOn)
                SetToggle();

            bloks[VolumeIndex--].SetActive(false);
        }
        private void SetIcon(bool isOn)
        {
            sliderIcon.sprite = isOn ? onIcon : offIcon;
        }

        private void OnDestroy()
        {
            toggle.onClick.RemoveAllListeners();
            next.onClick.RemoveAllListeners();
            previous.onClick.RemoveAllListeners();
        }
    }

    public enum VolumeType
    {
        Music,
        Sound
    }
}

