using System.Collections;
using UnityEngine;
namespace DarkJimmy.UI
{
    public class Splash : Menu
    {
        public float duration = 3f;

        public override void Start()
        {
            StartCoroutine(Skip(Menus.Loading.ToString(),duration));
        }
        public virtual IEnumerator Skip(string sceneName, float _time)
        {
            yield return new WaitForSeconds(_time);
            SceneManager.LoadScene(sceneName);
        }

    }


}
