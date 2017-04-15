using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_LevelCompleteRetry : MonoBehaviour
{
    public void LoadLevel()
    {
        //Dont allow restarting during end level transitions
        if(!GameDirector.LevelManager.levelUIController.TransitioningIn && !GameDirector.LevelManager.levelUIController.TransitioningOut)
        {
            GameDirector.LevelManager.UnloadLevel(GameDirector.LevelManager.CurrentLevelID);
            GameDirector.LevelManager.LoadLevelAdditive(GameDirector.LevelManager.CurrentLevelID);
        }
        

        //Only trigger the transition if the end game menu is fully up
        if(GameDirector.LevelManager.levelUIController.MenuUp)
        {
            GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
        }
        
    }
}
