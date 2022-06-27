using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Pause : Menu
    {
        [SerializeField]
        private BaseButton stage;
        [SerializeField]
        private BaseButton restart;
        [SerializeField]
        private BaseButton resume;

        AudioManager audioManager;
        GameSaveManager gsm;

        private  void Awake()
        {
            audioManager = AudioManager.Instance;
            gsm = GameSaveManager.Instance;
        }
        public override void Start()
        {
            base.Start();
            gsm.pause();
            stage.OnClick(Stage);
            restart.OnClick(Restart);
            resume.OnClick(Resume);
        }

        private void Stage()
        {
            Fade.Instance.FadeOut(LoadStageScene);
        }
        private void Restart()
        {         
            Fade.Instance.FadeOut(LoadCurrentScene);
        }
        private void LoadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveSceneName());
        }
        private void LoadStageScene()
        {
            SceneManager.LoadScene(Menus.Stages.ToString());
        }
        private void Resume()
        {
            GoBack();
            UIManager.Instance.OpenMenu(Menus.Starter);
            //gsm.StartCountDownTimer();
            audioManager.PlaySound("Pause");
        }

        private void SetVolume(bool isOn)
        {
            if (audioManager == null)
                return;

            audioManager.SourceFadeVolume(SoundGroupType.Music, isOn);
            audioManager.SourceFadeVolume(SoundGroupType.Ambient, isOn);
        }
        public override void OnEnable()
        {
            base.OnEnable();
            gsm.IsStartGame = false;
            audioManager.PlaySound("Pause");
            SetVolume(false);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            SetVolume(true);
        }

        
    }
}

