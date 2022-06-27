using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using DG.Tweening;

namespace DarkJimmy
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Mixer Groups")]
        [SerializeField]
        private AudioMixerGroup musicMixer;           
        [SerializeField]
        private AudioMixerGroup ambientMixer;         
        [SerializeField]
        private AudioMixerGroup playerSoundMixer;     
        [SerializeField]
        private AudioMixerGroup sound2DMixer;         
        [SerializeField]
        private AudioMixerGroup sound3DMixer;         
        [SerializeField]
        private AudioMixerGroup uýSoundMixer;        
        [SerializeField]
        private List<SoundGroup> soundGroups;
        [SerializeField]
        private List<AudioClip> musicClips;

        private AudioSource musicSource;
        private AudioSource ambientSource;
        private AudioSource playerSoundSource;
        private AudioSource sound2DSource;
        private AudioSource sound3DSource;
        private AudioSource uýSoundSource;
        
        private readonly Dictionary<string, SoundGroup> soundGroupDictionary = new Dictionary<string, SoundGroup>();

        public delegate void SetVolume(UI.VolumeType type);
        public SetVolume setVolume;
        public override void Awake()
        {
            base.Awake();
            SetSoundGroup();
            musicSource = gameObject.AddComponent<AudioSource>();
            ambientSource= gameObject.AddComponent<AudioSource>();
            playerSoundSource= gameObject.AddComponent<AudioSource>();
            sound2DSource = gameObject.AddComponent<AudioSource>();
            sound3DSource = gameObject.AddComponent<AudioSource>();
            uýSoundSource= gameObject.AddComponent<AudioSource>();
        }
        private void Start()
        {
            SetAdiouManagerSource();

            //SceneManager.onChangedScene += ChangeMusicAndAmbient;
            setVolume += SetSourceStatus;
        }

        #region Public Methods
        public void SetAdiouManagerSource()
        {
            SetSource(ref musicSource, ref musicMixer, true, UI.VolumeType.Music);
            SetSource(ref ambientSource, ref ambientMixer, true, UI.VolumeType.Music);
            SetSource(ref playerSoundSource, ref playerSoundMixer, false, UI.VolumeType.Sound);
            SetSource(ref sound2DSource, ref sound2DMixer, false,UI.VolumeType.Sound);
            SetSource(ref sound3DSource, ref sound3DMixer, false, UI.VolumeType.Sound);
            SetSource(ref uýSoundSource, ref uýSoundMixer, false, UI.VolumeType.Sound);
        }
        public void PlaySound(string soundName)
        {
            AudioClip clip = GetClipFromName(soundName.Trim(), out SoundGroupType type);
            AudioSource source = GetAudioSource(type);

            if (!source.enabled)
                return;

            source.PlayOneShot(clip);
        }
        public void PlayMusic(string soundName)
        {
            AudioClip clip = GetClipFromName(soundName.Trim(), out SoundGroupType type);
            AudioSource source = GetAudioSource(type);
            source.clip = clip;

            if (!source.enabled)
                return;

            source.Play(); 
        }   
        public void StopSource(SoundGroupType type)
        {
            AudioSource source = GetAudioSource(type);

            if(source.enabled && source.isPlaying)
                source.Stop();
        }
        public void PauseSource(SoundGroupType type)
        {
            AudioSource source = GetAudioSource(type);

            if (source.enabled && source.isPlaying)
                source.Pause();
        }
        public void PlaySource(SoundGroupType type)
        {
            AudioSource source = GetAudioSource(type);

            if (source.enabled && source.isPlaying)
                source.Play();
        }

        #endregion

        #region Private Methods
        private void SetSourceStatus(UI.VolumeType volumeType)
        {
            switch (volumeType)
            {
                case UI.VolumeType.Music:
                    SetSource(ref musicSource, ref musicMixer, true, volumeType);
                    SetSource(ref ambientSource, ref ambientMixer, true, volumeType);
                    break;
                case UI.VolumeType.Sound:
                    SetSource(ref playerSoundSource, ref playerSoundMixer, false, volumeType);
                    SetSource(ref sound2DSource, ref sound2DMixer, false, volumeType);
                    SetSource(ref sound3DSource, ref sound3DMixer, false, volumeType);
                    SetSource(ref uýSoundSource, ref uýSoundMixer, false, volumeType);
                    break;
                default:
                    break;
            }
        }
        private AudioClip GetClipFromName(string soundName, out SoundGroupType type)
        {
            type = 0;
            if (soundGroupDictionary.ContainsKey(soundName))
            {
                type = soundGroupDictionary[soundName].groupType;
                List<AudioClip> sounds = soundGroupDictionary[soundName].group;
                return sounds[UnityEngine.Random.Range(0, sounds.Count)];
            }
            return null;
        }
        private void SetSoundGroup()
        {
            foreach (SoundGroup soundGroup in soundGroups)
            {
                soundGroupDictionary.Add(soundGroup.groupID, soundGroup);
            }
        }
        public void SourceFadeVolume(SoundGroupType type, bool isOn)
        {
            AudioSource source = GetAudioSource(type);
            if (!source.enabled)
                return;

            float multiple = isOn ? 2 : 0.5f;
            float volume = source.volume * multiple;

            source.DOFade(volume, 0.5f);
           
        }
        private void SetSource(ref AudioSource source, ref AudioMixerGroup mixerGroup, bool isLoop,UI.VolumeType type)
        {
            float value = LocalSaveManager.GetIntValue(LocalSaveManager.GetSliderName(type), 9) + 1;
            source.volume = value*0.1f;
            source.outputAudioMixerGroup = mixerGroup;
            source.loop = isLoop;
            source.enabled = LocalSaveManager.GetBoolValue(LocalSaveManager.GetToggleName(type), true);
        }
        private AudioSource GetAudioSource(SoundGroupType groupType)
        {
            return groupType switch
            {
                SoundGroupType.Ambient => ambientSource,
                SoundGroupType.Player => playerSoundSource,
                SoundGroupType.Sound2D => sound2DSource,
                SoundGroupType.Sound3D => sound3DSource,
                SoundGroupType.SoundUI => uýSoundSource,
                _ => musicSource,
            };
        }
        #endregion

    }
    [Serializable]
    public class SoundGroup
    {
        public string groupID;
        public SoundGroupType groupType;
        public List<AudioClip> group;
    }
    public enum SoundGroupType
    {
        Music,
        Ambient,
        Player,
        Sound2D,
        Sound3D,
        SoundUI
    }
}

