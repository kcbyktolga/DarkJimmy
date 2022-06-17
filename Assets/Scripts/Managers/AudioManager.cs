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
        private AudioMixerGroup sound2DMixer;         //The sound 2D mixer group
        [SerializeField]
        private AudioMixerGroup sound3DMixer;         //The sound 3D mixer group
        [SerializeField]
        private List<SoundGroup> soundGroups;
        [SerializeField]
        private List<AudioClip> musicClips;

        private AudioSource musicSource;
        private AudioSource sound2DSource;
        private AudioSource sound3DSource;
        private Dictionary<string, List<AudioClip>> soundGroupDictionary = new Dictionary<string, List<AudioClip>>();

        public delegate void SetVolume(UI.VolumeType type);
        public SetVolume setVolume;
        public override void Awake()
        {
            base.Awake();
            SetSoundGroup();
            musicSource = gameObject.AddComponent<AudioSource>();
            sound2DSource = gameObject.AddComponent<AudioSource>();
            sound3DSource = gameObject.AddComponent<AudioSource>();
        }
        private void Start()
        {
            SetAdiouManagerSource();

            // SceneManager.activeSceneChanged += Play;
            setVolume += SetSourceStatus;
        }


        #region Public Methods
        public void SetAdiouManagerSource()
        {
            SetSource(ref musicSource, ref musicMixer, true, UI.VolumeType.Music);
            SetSource(ref sound2DSource, ref sound2DMixer, false,UI.VolumeType.Sound);
            SetSource(ref sound3DSource, ref sound3DMixer, false, UI.VolumeType.Sound);
        }
        public void PlaySound(string soundName)
        {
            if (!sound2DSource.enabled)
                return;

            sound2DSource.PlayOneShot(GetClipFromName(soundName));
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

        private void SetSourceStatus(UI.VolumeType volumeType)
        {
            switch (volumeType)
            {
                case UI.VolumeType.Music:
                    SetSource(ref musicSource, ref musicMixer, true, volumeType);               
                    break;
                case UI.VolumeType.Sound:
                    SetSource(ref sound2DSource, ref sound2DMixer, false, volumeType);
                    SetSource(ref sound3DSource, ref sound3DMixer, false, volumeType);
                    break;
                default:
                    break;
            }
        }
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
        private void SetSoundGroup()
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
            float value = LocalSaveManager.GetIntValue(LocalSaveManager.GetSliderName(type), 9) + 1;
            source.volume = value*0.1f;
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

