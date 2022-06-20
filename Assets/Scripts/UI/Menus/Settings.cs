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
            ActivateBase();

            userId.text = $" User ID: {CloudSaveManager.Instance.UserId}";
        }

    }

}

