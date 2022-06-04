using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DarkJimmy.UI
{
    public class Splash : Menu
    {
        [SerializeField]
        private float duration = 3f;

        public override void Start()
        {
            StartCoroutine(nameof(Skip));
        }
        IEnumerator Skip()
        {
            yield return new WaitForSeconds(duration);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }


}
