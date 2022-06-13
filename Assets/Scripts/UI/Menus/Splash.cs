using System.Collections;
using UnityEngine;
namespace DarkJimmy.UI
{
    public class Splash : Menu
    {
        public float duration = 3f;

        public override void Start()
        {
            StartCoroutine(Skip(Menus.Loading.ToString()));
        }
        public virtual IEnumerator Skip(string sceneName)
        {
            yield return new WaitForSeconds(duration);
            SceneManager.LoadScene(sceneName);
        }

    }


}
