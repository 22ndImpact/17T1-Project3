using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class GameDirector
{
    public static GD_SceneManager SceneManager = new GD_SceneManager();
    public static GD_InputManager InputManager = new GD_InputManager();
    public static GD_LevelManager LevelManager = new GD_LevelManager();
    public static DataManager dataManager = new DataManager();
}

public class GD_SceneManager
{
    public string CurrentSceneName
    {
        get
        {
            return SceneManager.GetActiveScene().name;
        }
    }

    public enum SceneList
    {
        Splash,
        MainMenu,
        Trophies,
        LevelSelect,
        LevelComplete
    }

    public void ChangeScene(SceneList _Scene)
    {
        SceneManager.LoadScene((int)_Scene, LoadSceneMode.Single);
    }

    public void ChangeScene(string _SceneName)
    {
        SceneManager.LoadScene(_SceneName, LoadSceneMode.Single);
    }

    public void LoadScene(string _SceneName)
    {
        SceneManager.LoadScene(_SceneName, LoadSceneMode.Additive);
    }

    public void LoadScene(int _SceneBuildID)
    {
        SceneManager.LoadScene(_SceneBuildID, LoadSceneMode.Additive);
    }

    public void UnloadScene(string _SceneName)
    {
        SceneManager.UnloadSceneAsync(_SceneName);
    }

    public void UnloadScene(int _SceneBuildID)
    {
        SceneManager.UnloadSceneAsync(_SceneBuildID);
    }
}

public class GD_InputManager
{
    public bool DebugKeyDown
    {
        get
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
}

public class GD_LevelManager
{
    public int CurrentLevelID;

    public LevelController CurrentLevel;

    //List of persistant level data that is used by the level selector and updated by indervidual level controllers
    List<LevelData> LevelDataList = new List<LevelData>();

    public int GetLevelIDFromScene(string _SceneName)
    {
        return Int32.Parse(System.Text.RegularExpressions.Regex.Match(_SceneName, @"\d+$").Value);
    }

    //Scans scenes and populates a list of level data based on level scenes
    public void PopulateLevelList()
    {

        //TODO remove: Debug code that always loads from the scriptable object, effectily disabling persistant data
        LevelDataList = Resources.Load<LevelDataCollection>("ScriptableObjects/Level Collection").LevelList;

        //if (GameDirector.dataManager.SaveDataFound)
        //{
        //    LoadLevelData();
        //}
        //else
        //{
        //    LevelDataList = Resources.Load<LevelDataCollection>("ScriptableObjects/Level Collection").LevelList;
        //    SaveLevelData();
        //}
    }

    //Returns the level data from the level data list that corrispondes to the level id
    public LevelData GetLevelData(int _LevelID)
    {
        foreach(LevelData levelData in LevelDataList)
        {
            if(levelData.LevelID == "Level_" + _LevelID)
            {
                return levelData;
            }
        }

        return null;
    }

    public void LoadLevel(int _LevelID)
    {
        //Hard coded prefix that corispondes with scene naming convention
        GameDirector.SceneManager.ChangeScene("Level_" + _LevelID);

        //Updates the live tracking of the CurrentLevelID
        CurrentLevelID = _LevelID;
    }

    public void SaveLevelData()
    {
        GameDirector.dataManager.SaveLevelData(LevelDataList);
    }

    public void LoadLevelData()
    {
        LevelDataList = GameDirector.dataManager.LoadLevelData();
    }
}

