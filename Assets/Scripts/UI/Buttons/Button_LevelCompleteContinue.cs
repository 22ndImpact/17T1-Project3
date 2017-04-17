using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelCompleteContinue : MonoBehaviour
{
    #region Tweaking Variables
    public string UnloackedText;
    public string LockedText;
    public string NoLevelsText;
    #endregion

    #region Object References
    public Text text;
    #endregion

    /// <summary>
    /// Sets the unlock state and colour of the button based on the level data
    /// </summary>
    public void UpdateState()
    {   
        if(GameDirector.LevelManager.CurrentLevelID == GameDirector.LevelManager.LevelDataList.Count)
        {
            GetComponent<Button>().enabled = true;
            text.text = NoLevelsText;
        }
        else if (GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID + 1).Unlocked == false)
        {
            GetComponent<Button>().enabled = false;
            text.text = LockedText;
        }
        else
        {
            GetComponent<Button>().enabled = true;
            text.text = UnloackedText;
        }
    }

    /// <summary>
    /// Unloads the current level and loads the new one
    /// </summary>
    public void LoadLevel()
    {
        //If you are on the last level
        if (GameDirector.LevelManager.CurrentLevelID == GameDirector.LevelManager.LevelDataList.Count)
        {
            //Activate the level select
            GameDirector.menuController.ActivateLevelSelect();

            //Trigger the transition if the end game menu is fully up
            if (GameDirector.LevelManager.levelUIController.MenuUp)
            {
                GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
            }
        }
        //If you are not on the last level
        else
        {
            GameDirector.LevelManager.UnloadLevel(GameDirector.LevelManager.CurrentLevelID);
            GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID + 1);

            //Transition the new level In
            GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
        }
    }
}
