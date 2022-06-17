using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

namespace DarkJimmy
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Mixer Groups")]
        [SerializeField]
        private AudioMixerGroup musicMixer;           //The music mixer group
        [SerializeField]
        private AudioMixerGroup soundMixer;           //The sound mixer group
        [SerializeField]
        private List<SoundGroup> soundGroups;
        [SerializeField]
        private List<AudioClip> musicClips;

        private AudioSource musicSource;
        private AudioSource soundSource;
        private Dictionary<string, List<AudioClip>> soundGroupDictionary = new Dictionary<string, List<AudioClip>>();

        public override void Awake()
        {
            base.Awake();
            SetSoundGrop();
            musicSource = gameObject.AddComponent<AudioSource>();
            soundSource = gameObject.AddComponent<AudioSource>();
        }
        private void Start()
        {
           // SetAdiouManagerSource();

           // SceneManager.activeSceneChanged += Play;
        }


        #region Public Methods
        public void SetAdiouManagerSource()
        {
            SetSource(ref musicSource, ref musicMixer, true, UI.VolumeType.Music);
            SetSource(ref soundSource, ref soundMixer, false,UI.VolumeType.Sound);
        }
        public void PlaySound(string soundName)
        {
            if (!soundSource.enabled)
                return;

            soundSource.PlayOneShot(GetClipFromName(soundName));
        }
        public void PauseMusic()
        {
            if (!musicSource.enabled)
                return;

            musicSource.Pause();
        }
        public void PlayMusic()
        {
            if (!musicSource.enabled)
                return;

            musicSource.Play();
        }
        public void Stop()
        {
            if (!musicSource.enabled)
                return;

            musicSource.Stop();
        }

        //public void SetSourceStatus(ToggleType type, bool isOn)
        //{
        //    GetAudioSource(type).enabled = isOn;
        //}
        #endregion

        #region Private Methods
        private AudioClip GetClipFromName(string soundName)
        {
            if (soundGroupDictionary.ContainsKey(soundName))
            {
                List<AudioClip> sounds = soundGroupDictionary[soundName];
                return sounds[UnityEngine.Random.Range(0, sounds.Count)];
            }
            return null;
        }
        //private AudioClip GetClip()
        //{
        //    return musicClips[SceneManager.GetActiveScene().buildIndex];
        //}
        //private AudioSource GetAudioSource(ToggleType type)
        //{
        //    switch (type)
        //    {
        //        default:
        //        case ToggleType.Music:
        //            return musicSource;
        //        case ToggleType.Sound:
        //            return soundSource;
        //    }
        //}
        //private void Play(Scene e, Scene a)
        //{
        //    if (SceneManager.GetActiveScene().buildIndex.Equals(0) || SceneManager.GetActiveScene().buildIndex.Equals(1))
        //    {
        //        if (musicSource != null)
        //            musicSource.Stop();

        //        return;
        //    }


        //    PlayMusic(GetClip());
        //}
        private void SetSoundGrop()
        {
            foreach (SoundGroup soundGroup in soundGroups)
            {
                soundGroupDictionary.Add(soundGroup.groupID, soundGroup.group);
            }
        }
        private void PlayMusic(AudioClip musicClip)
        {
            if (musicSource == null || !musicSource.enabled || musicClip == null)
                return;

            musicSource.clip = musicClip;
            musicSource.Play();


        }
        private void SetSource(ref AudioSource source, ref AudioMixerGroup mixerGroup, bool isLoop,UI.VolumeType type)
        {
            source.volume = LocalSaveManager.GetIntValue(LocalSaveManager.GetSliderName(type),9);
            source.outputAudioMixerGroup = mixerGroup;
            source.loop = isLoop;
            source.enabled = LocalSaveManager.GetBoolValue(LocalSaveManager.GetToggleName(type), true);
        }
        #endregion

    }
    [Serializable]
    public class SoundGroup
    {
        public string groupID;
        public List<AudioClip> group;
    }
}

