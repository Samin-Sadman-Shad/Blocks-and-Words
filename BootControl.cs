using UnityEngine;
using UnityEngine.SceneManagement;

public class BootControl : MonoBehaviour
{
    public static string key = "level";
    public static int mainLevel;
    public static string lastScenekey = "lastSceneIndex";
    private void Awake()
    {
        //SceneManager.LoadScene(1);
        //PlayerPrefs.SetInt(key, Samin.BlocksAndWords.LevelManager.level);
        mainLevel = PlayerPrefs.GetInt(key, 1);
        Debug.Log("scenecount " + SceneManager.sceneCountInBuildSettings);
        Debug.Log("main level started from " + mainLevel);
        if(PlayerPrefs.GetInt(key) < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt(key, 1));   
        }
        else
        {
            /*
            var remainderLevel = (PlayerPrefs.GetInt(key) - (SceneManager.sceneCountInBuildSettings - 1)) % (SceneManager.sceneCountInBuildSettings - 2);
            Debug.Log("scene index to start " + remainderLevel);
            if(remainderLevel < 0)
            {
                SceneManager.LoadScene(PlayerPrefs.GetInt(key) - (SceneManager.sceneCountInBuildSettings - 2));
            }
            else if (remainderLevel != 0)
            {
                SceneManager.LoadScene(remainderLevel + 1);
            }
            else if(remainderLevel == 0)
            {
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }
            */
            Debug.Log(PlayerPrefs.GetInt(lastScenekey, 2));
            SceneManager.LoadScene(PlayerPrefs.GetInt(lastScenekey, 2));

        }
        
        SceneManager.sceneLoaded += OnLoadLevel;
        DontDestroyOnLoad(gameObject); 
    }

    private void OnLoadLevel(Scene scene, LoadSceneMode arg1)
    {
        if (scene.buildIndex == 0) { return; }
        TinySauce.OnGameStarted("" + Samin.BlocksAndWords.LevelManager.level);
        //PlayerPrefs.SetInt(key, Samin.BlocksAndWords.LevelManager.level);
        PlayerPrefs.SetInt(key, mainLevel);
        TinySauce.OnGameStarted("" + PlayerPrefs.GetInt(key));
        /*
        Debug.Log("<color='magenta'>Level started! : " + PlayerPrefs.GetInt(key) +
            " and loaded actual level: " + (SceneManager.GetActiveScene().buildIndex + 1) + "</color>");
         */
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnLoadLevel;
    }
}
