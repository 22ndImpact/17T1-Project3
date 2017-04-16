using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public static class GameDirector
{
    #region Object References
    public static MenuController menuController;
    #endregion

    #region Component References
    public static GD_SceneManager SceneManager = new GD_SceneManager();
    public static GD_LevelManager LevelManager = new GD_LevelManager();
    public static DataManager dataManager = new DataManager();
    #endregion
}
public class GD_SceneManager
{
    /// <summary>
    /// Returns the first scene name of a "Level_X" scene currently active in the game
    /// </summary>
    public string CurrentLevelSceneName
    {
        get
        {
            //Loop over all scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                //If it contains "Level_"
                if(SceneManager.GetSceneAt(i).name.Contains("Level_"))
                {
                    //Return the scene name
                    return SceneManager.GetSceneAt(i).name;
                }
            }

            return "No Level Loaded";
        }
    }
    /// <summary>
    /// Adds a scene to the game based on its Scene Name
    /// </summary>
    /// <param name="_SceneName"></param>
    public void LoadScene(string _SceneName)
    {
        SceneManager.LoadScene(_SceneName, LoadSceneMode.Additive);
    }
    /// <summary>
    /// Removes a scene to the game based on its Scene Name
    /// </summary>
    /// <param name="_SceneName"></param>
    public void UnloadScene(string _SceneName)
    {
        SceneManager.UnloadSceneAsync(_SceneName);
    }
}
public class GD_LevelManager
{
    #region Tracking Variables
    public int CurrentLevelID;
    public bool LevelListPopulated = false;
    #endregion

    #region Object References
    public List<Button_SelectLevel> LevelSelectButtons = new List<Button_SelectLevel>();
    public LevelController CurrentLevel;
    public LevelUIController levelUIController;
    //List of persistant level data that is used by the level selector and updated by indervidual level controllers
    List<LevelData> LevelDataList = new List<LevelData>();
    #endregion

    /// <summary>
    /// Strips the prefix text from a scene name and returs the ID number, intended only for use with level names with the format "Level_XX"
    /// </summary>
    /// <param name="_SceneName"></param>
    /// <returns></returns>
    public int GetLevelIDFromScene(string _SceneName)
    {
        //Got this black magic code from the internet 10/10
        return Int32.Parse(System.Text.RegularExpressions.Regex.Match(_SceneName, @"\d+$").Value);
    }
    /// <summary>
    /// Creates and populates a list of level data from either the default or saved file.
    /// </summary>
    public void PopulateLevelList()
    {
        //If saved data is found
        if (GameDirector.dataManager.SaveDataFound)
        {
            //Load from that
            LoadLevelData();
        }
        //If i is not found
        else
        {
            //Load from the "Level Collection" scriptable object
            LevelDataList = Resources.Load<LevelDataCollection>("ScriptableObjects/Level Collection").LevelList;
            SaveLevelData();
        }
    }
    /// <summary>
    /// Updates all the information on the level select buttons
    /// </summary>
    public void UpdateLevelButtonInfo()
    {
        foreach (Button_SelectLevel levelSelectButton in LevelSelectButtons)
        {
            levelSelectButton.RefreshLevelButtonInfo();
        }
    }
    /// <summary>
    /// Overwrites the saved data file with the info from the default level info
    /// </summary>
    public void ResetLocalData()
    {
        //Reset data
        LevelDataList = Resources.Load<LevelDataCollection>("ScriptableObjects/Level Collection").LevelList;
        //Save newly reset data
        SaveLevelData();
        //Repopulate level list
        PopulateLevelList();
    }
    /// <summary>
    /// Returns a levelData object from the levelDataList that corrispondes to the given level id
    /// </summary>
    /// <param name="_LevelID"></param>
    /// <returns></returns>
    public LevelData GetLevelData(int _LevelID)
    {
        foreach(LevelData levelData in LevelDataList)
        {
            if(levelData.LevelID == _LevelID)
            {
                return levelData;
            }
        }
        return null;
    }
    /// <summary>
    /// Loads a new level scene from a given Level ID
    /// </summary>
    /// <param name="_LevelID"></param>
    public void LoadLevel(int _LevelID)
    {
        //TODO Chenge: Hard coded prefix that corispondes with scene naming convention
        GameDirector.SceneManager.LoadScene("Level_" + _LevelID);
    }
    /// <summary>
    /// Unloads a new level scene from a given Level ID
    /// </summary>
    /// <param name="_LevelID"></param>
    public void UnloadLevel(int _LevelID)
    {
        //TODO Change: Hard coded prefix that corispondes with scene naming convention
        GameDirector.SceneManager.UnloadScene("Level_" + _LevelID);
    }
    /// <summary>
    /// Saves the currelt levelDataList to the extrenal save file
    /// </summary>
    public void SaveLevelData()
    {
        GameDirector.dataManager.SaveLevelData(LevelDataList);
    }
    /// <summary>
    /// Loads the currelt levelDataList to the extrenal save file
    /// </summary>
    public void LoadLevelData()
    {
        LevelDataList = GameDirector.dataManager.LoadLevelData();
    }
}

