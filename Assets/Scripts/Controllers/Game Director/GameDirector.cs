using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameDirector
{
    public static GD_SceneManager SceneManager = new GD_SceneManager();
    public static GD_InputManager InputManager = new GD_InputManager();
    public static GD_LevelManager LevelManager = new GD_LevelManager();
}

public class GD_SceneManager
{
    public enum SceneList
    {
        Splash,
        MainMenu,
        Trophies,
        LevelSelect,
        GamePlay
    }

    public void ChangeScene(SceneList _Scene)
    {
        SceneManager.LoadScene((int)_Scene, LoadSceneMode.Single);
    }

    public void LoadScene(string _SceneName)
    {
        SceneManager.LoadScene(_SceneName, LoadSceneMode.Additive);
    }

    public void UnloadScene(string _SceneName)
    {
        SceneManager.UnloadSceneAsync(_SceneName);
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

    public void LoadLevel(int _LevelID)
    {
        //Hard coded prefix that corispondes with scene naming convention
        GameDirector.SceneManager.LoadScene("Level_" + _LevelID);

        //Updates the live tracking of the CurrentLevelID
        CurrentLevelID = _LevelID;
    }

    public void ChangeLevel(int _LevelID)
    {
        //Unloads the current level
        GameDirector.SceneManager.UnloadScene("Level_" + CurrentLevelID);

        //And loads the new one
        GameDirector.SceneManager.LoadScene("Level_" + _LevelID);

        //Updates the live tracking of the CurrentLevelID
        CurrentLevelID = _LevelID;
    }
}
