using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
namespace DarkJimmy.UI
{
    public class GemEffect : MonoBehaviour
    {
        [SerializeField]
        private float endPos;
        [SerializeField]
        private TMP_Text amountText;

        Color fadeColor;
        Color originalColor;
        SystemManager system;

        private void Awake()
        {
            system = SystemManager.Instance;
            system.updateTransform += SetPosition;
            system.updateStats += SetAmount;
        }
        private void Start()
        {           
            fadeColor = system.GetWhiteAlfaColor(false);
            originalColor = amountText.color;
            gameObject.SetActive(false);
        }
        private void SetPosition(Vector2 position)
        {
            transform.position = position;
        }
        public void SetAmount(Stats stats, int amount)
        {
            if (stats.Equals(Stats.JumpCount) || stats.Equals(Stats.Time))
                return;

            string k = amount < 0 ? "-" : "+";
            amountText.text = $"{k}{amount}";
            gameObject.SetActive(true);
        }

        private void Scale()
        {
            amountText.transform.DOScale(Vector3.one, 0.5f)
                .SetEase(Ease.OutBounce)
                .OnComplete(Fade);

        }
        private void Fade()
        {
            transform.DOMoveY(transform.position.y + endPos, 0.5f);
            amountText.DOColor(fadeColor, 0.5f).OnComplete(DeActive);
        }
        private void DeActive()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Scale();
        }
        private void OnDisable()
        {
            amountText.color = originalColor;
        }
        private void OnDestroy()
        {
            system.updateTransform -= SetPosition;
            system.updateStats -= SetAmount;
        }
    }
}

