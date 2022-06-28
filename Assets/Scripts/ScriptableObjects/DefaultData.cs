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

        [Header("Stage Property")]
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
    }
    [Serializable]
    public class Level
    {
        [SerializeField]
        private int countDownTime;
        [SerializeField]
        private Platform platform;
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
        public int Energy;
        public int Mana;
        public int Speed;

        public int GetCharacterProperty(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => Mana,
                CharacterProperty.Speed => Speed,   
                _ => Energy,
            };
        }
        public Sprite GetCharacterIcon()
        {
            return characterIcon;
        }
    }
    public enum CharacterProperty
    {
        Energy,
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

