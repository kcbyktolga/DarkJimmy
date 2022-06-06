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
        public string stageName;
        public bool stageIsLocked=true;
        public Sprite stageIcon;
        public List<Level> levels;
    }

    [Serializable]
    public class Level
    {
        public LevelStatus levelStatus;
        public int rankCount;

    }
    public enum LevelStatus
    {
        Passed,
        Active,
        Passive
    }
}

