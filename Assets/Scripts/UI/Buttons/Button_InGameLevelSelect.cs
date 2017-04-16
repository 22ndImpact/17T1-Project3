using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_InGameLevelSelect : MonoBehaviour
{
    public void ReturnToLevelSelect()
    {
        //Activate the level select
        GameDirector.menuController.ActivateLevelSelect();

        //Trigger the transition if the end game menu is fully up
        if (GameDirector.LevelManager.levelUIController.MenuUp)
        {
            GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
        }
    }
}
