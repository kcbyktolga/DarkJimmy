using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
    public class PrivacyPolicy : Popup
    {
        [SerializeField]
        private BaseButton policyButton;
        CloudSaveManager csm;
        public override void Start()
        {
            csm = CloudSaveManager.Instance;
            base.Start();

            popupButton.OnClick(OnAgree);
            popupButton.SetName(LanguageManager.GetText("I Agree"));
            policyButton.OnClick(() => UIManager.Instance.OpenMenu(Menus.Policy));
        }

        private void OnAgree()
        {
            csm.IAgree();
            GoBack();
        }

        public override void SetPageName()
        {
            pageName.text = LanguageManager.GetText($"{menuType}Desc");
        }
    }
}

