using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_LevelCompleteRetry : MonoBehaviour
{
    public void LoadLevel()
    {
        GameDirector.LevelManager.UnloadLevel(GameDirector.LevelManager.CurrentLevelID);
        GameDirector.LevelManager.LoadLevelAdditive(GameDirector.LevelManager.CurrentLevelID);

        //Transition the new level in
        GameDirector.LevelManager.CurrentLevel.levelUIController.StartLevelOpeningTransition();
    }
}
