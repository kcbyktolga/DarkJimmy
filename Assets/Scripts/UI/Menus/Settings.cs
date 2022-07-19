using UnityEngine;
using TMPro;

namespace DarkJimmy.UI
{
    public class Settings : Menu
    {
        [Header("Settings")]
        [SerializeField]
        private TMP_Text userId;
        public override void Start()
        {
            base.Start();
            // ActivateBase();
            SetText();
 
            LanguageManager.onChangedLanguage +=SetText;
        }
        public override void ScaleAnimation()
        {
            base.ScaleAnimation();
            AudioManager.Instance.PlaySound("Open Page");
        }
        private void SetText()
        {
            userId.text = $"{LanguageManager.GetText("UserId")}: {CloudSaveManager.Instance.UserId}";
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            LanguageManager.onChangedLanguage -= SetText;

        }
    }

}

