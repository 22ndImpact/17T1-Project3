using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_InGameLevelSelect : MonoBehaviour
{
    public void ReturnToLevelSelect()
    {
        //If you are not transitioning the level and if you are not in slow mo
        if(!GameDirector.LevelManager.levelUIController.TransitioningIn && !GameDirector.LevelManager.levelUIController.TransitioningOut && Time.timeScale == 1)
        {
            GameDirector.menuController.ActivateLevelSelect();
        }
        

        //Trigger the transition if the end game menu is fully up
        if (GameDirector.LevelManager.levelUIController.MenuUp)
        {
            GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
        }
    }
}
