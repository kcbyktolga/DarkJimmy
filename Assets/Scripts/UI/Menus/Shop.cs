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

            AudioManager.Instance.MusicVolumeSet(SoundGroupType.Music,false);
            AudioManager.Instance.MusicVolumeSet(SoundGroupType.Ambient, false);
        }

        private void OnDestroy()
        {
            AudioManager.Instance.MusicVolumeSet(SoundGroupType.Music, true);
            AudioManager.Instance.MusicVolumeSet(SoundGroupType.Ambient, true);
        }
    }

}

