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

        public GemType payType;
        public bool isLock = true;
        public int price;
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

