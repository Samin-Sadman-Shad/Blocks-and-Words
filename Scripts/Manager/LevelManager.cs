using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samin.BlocksAndWords
{
    public class LevelManager : MonoBehaviour
    {
        /*
        [SerializeField] List<int> rowNumbers;
        [SerializeField] List<int> colNumbers;
        [SerializeField] List<int> emptyBoxNumbers;
        public static int level = 0;
        public bool endLevel = false;
        void EveryLevel(int level)
        {
            if (!endLevel)
            {
                LevelManager.level = level;
                //GameManager levelGame = new GameManager(rowNumbers[level], colNumbers[level], emptyBoxNumbers[level], level);
                GridManager.gridSingleton.gameObject.SetActive(true);
                GridManager.gridSingleton.SetGridDimention(rowNumbers[level], colNumbers[level], emptyBoxNumbers[level]);
                GameManager.GameManagerSingleton.gameObject.SetActive(true);
                GameManager.GameManagerSingleton.level = level;

                if (GameManager.GameManagerSingleton.unmatchedIconWords.Count == 0)
                {
                    endLevel = true;
                    LevelManager.level++;
                }
            }
           
        }
        */
        private void Start()
        {
            //level = PlayerPrefs.GetInt(BootControl.key, 1);
            level = BootControl.mainLevel;
        }

        public static int level ;
        //public static int level;
        bool readyToChangeLevel = false;

        public void NextLevel()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;

            if (level == 1)
            {
                if (TutorialManager.tutorialSingleton.endLevel)
                {
                    readyToChangeLevel = true;
                }
            }
            else
            {
                if (GameManager.GameManagerSingleton.endLevel)
                {
                    readyToChangeLevel = true;
                }
            }

            if (readyToChangeLevel)
            {
               // Debug.Log(GameManager.GameManagerSingleton.level);
                if (currentScene < SceneManager.sceneCountInBuildSettings - 1)
                {
                    SceneManager.LoadScene(currentScene + 1);
                    //BootControl.lastSceneIndex = currentScene + 1;
                    PlayerPrefs.SetInt(BootControl.lastScenekey, currentScene + 1);
                }
                else
                {
                    SceneManager.LoadScene(2);
                    //BootControl.lastSceneIndex = 2;
                    PlayerPrefs.SetInt(BootControl.lastScenekey, 2);
                }

                var score = level * 5f;
                TinySauce.OnGameFinished(true, score, "" + level);
                /*
                Debug.Log("<color='magenta'>Level completed! : " + Samin.BlocksAndWords.LevelManager.level + " With score: " + score + "" +
                    " and loaded actual level: "+(SceneManager.GetActiveScene().buildIndex+1) +"</color>");

                //level++;
                */
                BootControl.mainLevel++;
                
            }
            
        }
    }
}

