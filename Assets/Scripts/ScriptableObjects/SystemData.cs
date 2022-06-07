using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;

namespace DarkJimmy
{
    [CreateAssetMenu(menuName = "Data/System Data", fileName = "System Data")]
    public class SystemData : ScriptableObject
    {
        [Header("Property")]
        [SerializeField]
        private Sprite passed;
        [SerializeField]
        private Sprite active;
        [SerializeField]
        private Sprite passive;

        public LevelTab levelTab;
        public LevelPage levelPage;

        [Header("Stages")]
        public List<Stage> Stages;
        [Header("Characters")]
        public List<CharacterData> CharacterDatas;
        
        public Sprite GetLevelSprite(LevelStatus status)
        {
            return status switch
            {
                LevelStatus.Active => active,
                LevelStatus.Passive => passive,
                _ => passed,
            };
        }     

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

        [Header("Stage Property")]
        public string stageName;
        public bool stageIsLocked=true;      
        public List<Level> levels;

        public Sprite GetStageIcon()
        {
            return stageIcon;
        }
    }

    [Serializable]
    public class Level
    {    
        [Header("Level Property")]
        public string levelName;
        public string levelId;
        public LevelStatus levelStatus;
        public int rankCount;
        public int keyCount;
        public int goldCount;
        public int currentScore;
        public int maxScore;
    }
    [Serializable]
    public class CharacterData
    {
        [Header("Character Property")]
        [SerializeField]
        private Sprite characterIcon;
        
        public string Id;
        public float Level;
        public float Energy;
        public float Mana;
        public float ERR;
        public float MMR;
        public float Speed;

        public float GetCharacterProperty(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => Mana,
                CharacterProperty.Speed => Speed,
                CharacterProperty.MMR => MMR,
                CharacterProperty.ERR => ERR,
                _ => Energy,
            };
        }
    }
    public enum CharacterProperty
    {
        Energy,
        Mana,
        Speed,
        MMR,
        ERR
    }
    public enum LevelStatus
    {
        Passed,
        Active,
        Passive
    }
}

