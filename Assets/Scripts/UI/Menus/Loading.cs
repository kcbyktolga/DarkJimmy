using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DarkJimmy.UI
{
 
    public class Loading : Splash
    {
        [SerializeField]
        private TMP_Text loadingText;
        [SerializeField]
        private TMP_Text versionText;
        [SerializeField]
        private Image rotateIcon;

        CloudSaveManager csm;
        SystemManager system;
        public override void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;

            versionText.text = $"v{Application.version}";


            Load(Menus.Lobby,0);
        }

        public void Load(Menus menuType, float seconds)
        {        
            StartCoroutine(Skip(menuType.ToString(),seconds));
        }
        public override IEnumerator Skip(string sceneName, float _time)
        {
            yield return new WaitForSeconds(_time);

            string sentence = "Datalar yükleniyor..";
            SetLoadingText(sentence);

            float time = duration + Time.time;

            while ((!csm.IsLoadedData || !AuthenticationService.Instance.IsSignedIn) && time >= Time.time)
                yield return null;

            if (!csm.IsSignedIn || !csm.IsLoadedData)
            {
                UIManager.Instance.OpenMenu(Menus.Disconnect);
                sentence = "Internet baðlantýsý yok..";
                SetLoadingText(sentence);
            }                  
            else
            {
                while (string.IsNullOrEmpty(csm.AppVersion))
                    yield return null;

                sentence = "Versiyon kontrol ediliyor..";
                SetLoadingText(sentence);
                
                if (csm.AppVersion !=Application.version)
                {
                    UIManager.Instance.OpenMenu(Menus.AppUpdate);

                    yield break;
                }

                yield return new WaitForSeconds(1);

                sentence = "Oyun birazdan baþlýyor..";
                SetLoadingText(sentence);
               // AdManager.Instance.InitializeAds();

                yield return new WaitForSeconds(duration);
                {
                    float t = 0;
                    Color currentColor = system.GetBlackAlfaColor(true);
                    Color endColor = system.GetBlackAlfaColor(false);

                    while (t <= 1)
                    {
                        t += Time.deltaTime / 0.5f;
                        currentColor = Color.Lerp(currentColor, endColor, t);
                        rotateIcon.color = versionText.color = loadingText.color = currentColor;
                        yield return null;
                    }
                    SceneManager.LoadScene(sceneName);
                }

             
            }
        }

        public void SetLoadingText(string sentence)
        {
            loadingText.text = sentence;
        }

    }

    public enum LoadingState
    {
        DataLoading,
        Disconnect,
        Reconnect,
        VersionCheck,
        GameLoading
    }

}
