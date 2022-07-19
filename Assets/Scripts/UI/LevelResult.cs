using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class LevelResult : MonoBehaviour
    {
        public Result result;  
        [SerializeField]
        private TMP_Text resultValue;
        [SerializeField]
        private Image resultIcon;

        private void Start()
        {
            resultIcon.sprite = SystemManager.Instance.GetResultSprite(result);
        }
        public void SetResultValue(int value, int totalValue)
        {
            resultValue.text = $"{SystemManager.Instance.StringFormat(value)}/{SystemManager.Instance.StringFormat(totalValue)}";
        }
      
    }

    public enum Result
    {
        Time,
        Gold,
        Key,
        Score
    }
}

