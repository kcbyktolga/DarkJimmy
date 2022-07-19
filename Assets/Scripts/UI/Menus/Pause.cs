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
            Fade.Instance.FadeOut(LoadStageScene,null);
        }
        private void Restart()
        {         
            Fade.Instance.FadeOut(LoadCurrentScene,null);
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
            UIManager.Instance.OpenMenu(Menus.TapToStart);
            //gsm.StartCountDownTimer();
            audioManager.PlaySound("Pause");
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

