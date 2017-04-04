using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GamePlay : SceneController
{
    void Start()
    {
        //Loads the current level immediatly upon loading the scene. This will only happen after transitioning from the level select, which sets the current level
        GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID);
    }

    void Update()
    {
        //TODO this is debug functionality, remove asap
        if(GameDirector.InputManager.DebugKeyDown)
        {
            GameDirector.LevelManager.ChangeLevel(2);
        }
    }
}
