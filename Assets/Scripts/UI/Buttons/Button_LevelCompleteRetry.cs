using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_LevelCompleteRetry : MonoBehaviour
{
    /// <summary>
    /// reload the current level, not do-able while in menu transition
    /// </summary>
    public void LoadLevel()
    {
        //Dont allow restarting during end level transitions
        if(!GameDirector.LevelManager.levelUIController.TransitioningIn && !GameDirector.LevelManager.levelUIController.TransitioningOut)
        {
            GameDirector.LevelManager.UnloadLevel(GameDirector.LevelManager.CurrentLevelID);
            GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID);
        }
        

        //Trigger the transition if the end game menu is fully up
        if(GameDirector.LevelManager.levelUIController.MenuUp)
        {
            GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
        }
        
    }
}
