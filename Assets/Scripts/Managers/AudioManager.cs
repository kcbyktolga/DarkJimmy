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
        private AudioMixerGroup ambientMixer;         //The sound 3D mixer group
        [SerializeField]
        private AudioMixerGroup playerSoundMixer;     //The sound 3D mixer group
        [SerializeField]
        private AudioMixerGroup sound2DMixer;         //The sound 2D mixer group
        [SerializeField]
        private AudioMixerGroup sound3DMixer;         //The sound 3D mixer group
        [SerializeField]
        private AudioMixerGroup uýSoundMixer;         //The sound 3D mixer group
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
       // private Dictionary<string, List<AudioClip>> soundGroupDictionary = new Dictionary<string, List<AudioClip>>();
        private Dictionary<string, SoundGroup> soundGroupDictionary = new Dictionary<string, SoundGroup>();

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

            Debug.Log(source);
            Debug.Log(clip.name);

            if (!source.enabled)
                return;
           
            source.clip = clip;
            source.Play();
    
        }
        public void PauseMusic()
        {
            if (musicSource.enabled && musicSource.isPlaying)
                musicSource.Pause();

            if (ambientSource.enabled && musicSource.isPlaying)
                ambientSource.Pause();

        }
        public void PlayMusic()
        {
            if (musicSource.enabled && !musicSource.isPlaying)
                musicSource.Play();

            if (ambientSource.enabled && !musicSource.isPlaying)
                ambientSource.Play();
        }
        public void StopMusic()
        {
            if (musicSource.enabled && musicSource.isPlaying)
                musicSource.Stop();

            if (ambientSource.enabled && musicSource.isPlaying)
                ambientSource.Stop();
        }

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
        #endregion

        #region Private Methods
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
                soundGroupDictionary.Add(soundGroup.groupID, soundGroup);
            }
        }

        //private void ChangeMusicAndAmbient()
        //{
         
        //    if (SceneManager.GetActiveSceneName()=="Lobby")
        //    {
        //        // remote config den belirlenecek..
        //        PlayMusic("Theme 1");
        //        PlayMusic("Ambient 1");
        //    }
        //}

        public void MusicVolumeSet(SoundGroupType type, bool isOn)
        {
            AudioSource source = GetAudioSource(type);
            if (!source.enabled)
                return;

            float multiple = isOn ? 4 : 0.25f;
            source.volume *= multiple;
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

