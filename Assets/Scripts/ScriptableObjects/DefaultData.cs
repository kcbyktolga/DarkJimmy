using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.Objects;

namespace DarkJimmy
{
    [CreateAssetMenu(menuName = "Data/System Data", fileName = "System Data")]
    public class DefaultData : ScriptableObject
    {

        [Header("Stages")]
        public List<Stage> Stages;
        [Header("Characters")]
        public List<CharacterData> CharacterDatas;
        [Header("Catalog")]
        public Catalog Catalog;

        [ContextMenu("Set Level Id")]
        public void SetLevel()
        {
            for (int i = 0; i < Stages.Count; i++)
            {
                for (int j = 0; j < Stages[i].levels.Count; j++)
                {
                    Stages[i].levels[j].levelId = $"{i}{j}";
                }   
            }
        }

        [ContextMenu("Set Stage Key Count")]
        private void SetStageKeyCount()
        {
            for (int i = 0; i < Stages.Count; i++)
                Stages[i].KeyCount = i * (Stages[i].levels.Count - 1) * 3;

        }
        [ContextMenu("Reset Stages")]
        private void ResetStages()
        {
            for (int i = 0; i < Stages.Count; i++)
            {
                for (int j = 0; j < Stages[i].levels.Count; j++)
                    Stages[i].levels[j].SetDefault(j);

                Stages[i].stageIsLocked = i!=0;
            }
        }
        [ContextMenu("Reset Character Data")]
        private void ResetCharacterData()
        {
            for (int i = 0; i < CharacterDatas.Count; i++)
                CharacterDatas[i].ResetCharacterData(i);
        }

    }

    [Serializable]
    public class Stage
    {
        [Header("Stage Settings")]      
        [SerializeField]
        private Sprite stageIcon;
        [SerializeField]
        private int stagePrice;
        [SerializeField]
        private GemType payType;
        [SerializeField]
        private int keyCount;
        [SerializeField]
        private BackgroundsType backgroundType;

        [Header("Stage Property")]
        public int stageIndex = 0;
        public string stageName;
        public bool stageIsLocked=true;      
        public List<Level> levels;

        public Sprite GetStageIcon()
        {
            return stageIcon;
        }     
        public int GetStagePrice()
        {
            return stagePrice;
        }
        public GemType GetPayType()
        {
            return payType;
        }
        public BackgroundsType GetBackgroundType()
        {
            return backgroundType;
        }    
        public int KeyCount { get { return keyCount; } set { keyCount = value; } }
    }
    [Serializable]
    public class Level
    {
        [SerializeField]
        private int countDownTime;
        [SerializeField]
        private Platform platform;
        [SerializeField]
        private Sprite levelIcon;
        [Header("Level Property")]
        public string levelName;
        public string levelId;
        public LevelStatus levelStatus;
        public int rankCount;
        public int keyCount;
        public int goldCount;
        public int currentScore;
        public int maxScore;
        public int GetLevelTime()
        {
            return countDownTime;
        }
        public Platform GetPlatform()
        {
            return platform;
        }
        public Sprite GetLevelIcon()
        {
            return levelIcon;
        }

        public void SetDefault(int index)
        {
            rankCount = 0;
            keyCount = 0;
            goldCount = 0;
            currentScore = 0;
            maxScore = 0;
            levelStatus = index==0?LevelStatus.Active:LevelStatus.Passive;

        }
    }
    [Serializable]
    public class CharacterData
    {
        [Header("Character Property")]
        [SerializeField]
        private Sprite characterIcon;

        public GemType payType;
        public bool isLock = true;
        public int price;
        public string Id;
        public int Level;
        public int JumpCount;

        public int MaxHPCapacity;
        public int MaxManaCapacity;
        public int MaxSpeedCapacity;

        public int HPLevel;
        public int ManaLevel;
        public int SpeedLevel;

        public List<int> Hps;
        public List<int> Manas;
        public List<int> Speeds;

        public  int GetCurrentCharacterProperty(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => Manas[ManaLevel],
                CharacterProperty.Speed => Speeds[SpeedLevel],
                _ => Hps[HPLevel],
            };
        }
        public int GetCharacterProperty(CharacterProperty property, int index)
        {
            return property switch
            {
                CharacterProperty.Mana => Manas[index],
                CharacterProperty.Speed => Speeds[index],
                _ => Hps[index],
            };
        }
        public int GetMaxCapacity(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => MaxManaCapacity,
                CharacterProperty.Speed => MaxSpeedCapacity,
                _ => MaxHPCapacity,
            };
        }
        public ref int GetCurrentSkillLevel(CharacterProperty property)
        {
            switch (property)
            {
                default:
                case CharacterProperty.HP:
                    return ref HPLevel;
                case CharacterProperty.Mana:
                    return ref ManaLevel;
                case CharacterProperty.Speed:
                    return ref SpeedLevel;
            }
        }

        public Sprite GetCharacterIcon()
        {
            return characterIcon;
        }

        public void ResetCharacterData(int index)
        {
            HPLevel = 0;
            ManaLevel = 0;
            SpeedLevel = 0;
            Level = 1;
            isLock = index != 0;
        }
    }
    public enum CharacterProperty
    {
        HP,
        Mana,
        Speed,
    }
    public enum LevelStatus
    {
        Passed,
        Active,
        Passive
    }

}

