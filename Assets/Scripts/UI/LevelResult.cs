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
        private Image background;
        [SerializeField]
        private TMP_Text resultName;
        [SerializeField]
        private TMP_Text resultValue;

        private SystemManager system;
        private void Start()
        {
            system = SystemManager.Instance;
            resultName.text = $"{LanguageManager.GetText(result.ToString())}:";
        }
        public void SetResultValue(int value, int totalValue)
        {
            resultValue.text = $"{system.StringFormat(value)}/{system.StringFormat(totalValue)}";
        }
        public void SetColor(Color color)
        {
            background.color = resultName.color = resultValue.color = color;
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

