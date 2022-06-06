using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DarkJimmy.UI;

namespace DarkJimmy
{
    [CreateAssetMenu(menuName = "Data/Levels", fileName = "Levels")]
    public class LevelData : ScriptableObject
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
        public List<Stage> stages;    
        
        public Sprite GetLevelSprite(LevelStatus status)
        {
            return status switch
            {
                LevelStatus.Active => active,
                LevelStatus.Passive => passive,
                _ => passed,
            };
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
        [Header("Level Settings")]
     
        [Header("Level Property")]
        public string levelName;
        public LevelStatus levelStatus;
        public int rankCount;
        public int keyCount;
        public int goldCount;
        public int currentScore;
        public int maxScore;
    }
    public enum LevelStatus
    {
        Passed,
        Active,
        Passive
    }

 
    public class LevelSettings
    {
    
    }

    public class StageSettings
    {

        public Sprite stageIcon;
    }

}

