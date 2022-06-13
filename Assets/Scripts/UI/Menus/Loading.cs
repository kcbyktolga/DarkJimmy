using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;

namespace DarkJimmy.UI
{
 
    public class Loading : Splash
    {
        public override void Start()
        {
            Load(Menus.Lobby);
        }

        public void Load(Menus menuType)
        {
            StartCoroutine(Skip(menuType.ToString()));
        }
        public override IEnumerator Skip(string sceneName)
        {

            float time = duration + Time.time;

            while ((!CloudSaveManager.Instance.IsLoadedData || !AuthenticationService.Instance.IsSignedIn) && time >= Time.time)
            {
                yield return null;
            }

            if (!CloudSaveManager.Instance.IsSignedIn || !CloudSaveManager.Instance.IsLoadedData)
                UIManager.Instance.OpenMenu(Menus.Disconnect);
            else
            {
                yield return new WaitForSeconds(duration);
                SceneManager.LoadScene(sceneName);
            }
                

        }
    }
}
