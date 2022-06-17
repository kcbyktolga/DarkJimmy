using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;

namespace DarkJimmy.UI
{
 
    public class Loading : Splash
    {
        CloudSaveManager csm;

        public override void Start()
        {
            csm = CloudSaveManager.Instance;

            Load(Menus.Lobby);
        }

        public void Load(Menus menuType)
        {
            StartCoroutine(Skip(menuType.ToString()));
        }
        public override IEnumerator Skip(string sceneName)
        {

            float time = duration + Time.time;

            while ((!csm.IsLoadedData || !AuthenticationService.Instance.IsSignedIn) && time >= Time.time)
            {
                yield return null;
            }

            if (!csm.IsSignedIn || !csm.IsLoadedData)
                UIManager.Instance.OpenMenu(Menus.Disconnect);         
            else
            {
                if (csm.AppVersion !=Application.version)
                {
                    UIManager.Instance.OpenMenu(Menus.AppUpdate);

                    yield break;
                }

                yield return new WaitForSeconds(duration);
                SceneManager.LoadScene(sceneName);
            }
                

        }
    }
}
