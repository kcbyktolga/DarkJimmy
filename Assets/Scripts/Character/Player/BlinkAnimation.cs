using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkJimmy.Characters;

namespace DarkJimmy
{
    public class BlinkAnimation : MonoBehaviour
    {
        [SerializeField]
        private Vector2 blinkTimeRange;
        [SerializeField]
        private Vector2 blinkDuration;
        [SerializeField]
        private int blinkRate = 3;

        public GameObject headDie;
        public float blinkTime;

        PlayerAnimation playerAnim;

        private void Start()
        {
            playerAnim = GetComponent<PlayerAnimation>();
        }
        private void Update()
        {
            if (playerAnim != null)
                return;

            if (blinkTime > Time.time)
                return;

                PlayBlink(true);

        }
        public void PlayBlink(bool isAlive)
        {
            StartCoroutine(Blink(isAlive));
        }

        IEnumerator Blink(bool isAlive)
        {
            blinkTime = Random.Range(blinkTimeRange.x, blinkTimeRange.y) + Time.time;

            int currentBlink = 0;
            int targetBlink = Random.Range(0, blinkRate);

            while (currentBlink <= targetBlink)
            {
                float time = 0;
                float duration = Random.Range(blinkDuration.x, blinkDuration.y);

                while (time <= duration)
                {
                    time += Time.deltaTime;
                    headDie.SetActive(true);

                    if (!isAlive)
                        yield break;

                    yield return null;
                }

                if (!isAlive)
                    yield break;

                headDie.SetActive(false);
                currentBlink++;
                yield return new WaitForSeconds(0.1f);

            }

        }
    }
}

