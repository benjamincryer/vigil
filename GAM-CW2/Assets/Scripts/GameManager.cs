using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveable
{
    public int levelID = 0;
    public string levelStats;

    //elapsedTime is the amount of time already taken at previous save
    public float startTime = 0, elapsedTime = 0;
    public int kills = 0, deaths = 0;

    private bool loadOnStart = false;

    public void LoadLevel(int id)
    {
        SceneManager.LoadScene(id);
        loadOnStart = true;
    }

    public void LoadLevelFromMenu()
    {
        //Load last level
        LoadGame();
        LoadLevel(levelID);
    }

    public void LoadGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        var ss = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        //var ss = FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable>();
        IEnumerable<ISaveable> ies = ss.AsEnumerable();

        SaveDataManager.LoadJsonData(ies, "Checkpoint");
    }

    public void SaveGame()
    {
        var ss = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();
        //var ss = FindObjectsOfTypeAll<MonoBehaviour>().OfType<ISaveable>();
        IEnumerable<ISaveable> ies = ss.AsEnumerable();

        SaveDataManager.SaveJsonData(ies, "Checkpoint");
    }

    //Get all gameobjects and search their transform children for components matching T
    //This includes deactivated gameobjects
    public static List<T> FindObjectsOfTypeAll<T>()
    {
        List<T> results = new List<T>();

        var s = SceneManager.GetActiveScene();
        if (s.isLoaded)
        {
            var allGameObjects = s.GetRootGameObjects();
            for (int j = 0; j < allGameObjects.Length; j++)
            {
                var go = allGameObjects[j];
                results.AddRange(go.GetComponentsInChildren<T>(true));
            }
        }
        return results;
    }

    void Start()
    {
        //Make this object persistent between scenes
        DontDestroyOnLoad(gameObject.transform);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    //Get level ID on scene load
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        int sceneID = scene.buildIndex;

        //first 3 scenes are menus, don't count them
        if (sceneID >= 3)
        {
            levelID = sceneID;
            startTime = Time.time;
            kills = 0;
            deaths = 0;

            if (loadOnStart)
            {
                LoadGame();
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }

        //skip to last level for demo
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            SceneManager.LoadScene("Level3");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        //Restore stats from save
        levelID = a_SaveData.m_LevelData.LevelID;
        kills = a_SaveData.m_LevelData.Kills;
        deaths = a_SaveData.m_LevelData.Deaths;
        elapsedTime = a_SaveData.m_LevelData.Time;
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        //Store level stats, to be restored if the level is later continued/reloaded after death
        a_SaveData.m_LevelData.LevelID = levelID;
        a_SaveData.m_LevelData.Kills = 0;
        a_SaveData.m_LevelData.Deaths = 0;
        a_SaveData.m_LevelData.Time = elapsedTime + (Time.time - startTime);
    }
}
