using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkJimmy.UI
{
    public class Stages : Menu
    {
        [Header("Stages Property")]
        [SerializeField]
        private LevelData levelData;
        [SerializeField]
        private RectTransform StageContainer;


        private int newStageIndex = 0;
        private int previousStageIndex = 0;



        public override void Start()
        {
                
        }

        private void GenerateTabAndLevel()
        {

        }

        private void Select(int index)
        {
            
        }

    }

}
