using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace DarkJimmy.UI
{
    public class Shop : Menu
    {
        public override void Start()
        {
            base.Start();

            AudioManager.Instance.SourceFadeVolume(SoundGroupType.Music,false);
            AudioManager.Instance.SourceFadeVolume(SoundGroupType.Ambient, false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            AudioManager.Instance.SourceFadeVolume(SoundGroupType.Music, true);
            AudioManager.Instance.SourceFadeVolume(SoundGroupType.Ambient, true);
        }
    }

}

