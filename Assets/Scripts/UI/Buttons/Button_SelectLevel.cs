using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_SelectLevel : MonoBehaviour
{
    #region Tracking variables
    //The ID of the level that this 
    public int LevelID;
    public string UnlockedText;
    public string LockedText;
    #endregion

    #region Tweaking Values
    public Color ColourLocked;
    public Color ColourUnlocked;
    public Color ColourPassed;
    public Color ColourPerfect;
    #endregion

    #region Component References
    public Text text;
    public Image image;
    #endregion

    /// <summary>
    /// Determines the values of the button based on current level data
    /// </summary>
    public void RefreshLevelButtonInfo()
    {
        //Sets the unlocked and locked colour and text
        if (GameDirector.LevelManager.GetLevelData(LevelID).Unlocked == false)
        {
            GetComponent<Button>().enabled = false;
            text.text = LockedText;
            image.color = ColourLocked;
        }
        else
        {
            GetComponent<Button>().enabled = true;
            text.text = UnlockedText;
            image.color = ColourUnlocked;
        }

        //Changes colour to passed or perfect if needed
        if (GameDirector.LevelManager.GetLevelData(LevelID).BestScore <= GameDirector.LevelManager.GetLevelData(LevelID).PerfectScore)
        {
            image.color = ColourPerfect;
        }
        else if (GameDirector.LevelManager.GetLevelData(LevelID).BestScore <= GameDirector.LevelManager.GetLevelData(LevelID).PassScore)
        {
            image.color = ColourPassed;
        }
    }

    /// <summary>
    /// Loads the selected level
    /// </summary>
    public void LoadLevel()
    {
        //Moves the menus away from the Camera
        GameDirector.menuController.ActivateGamePlay();

        //Removes the currently loaded level
        if(GameDirector.LevelManager.CurrentLevel != null)
        {
            GameDirector.SceneManager.UnloadScene(GameDirector.LevelManager.CurrentLevel.LevelName);
        }

        //Returns the in game menu (if up) to down
        if(GameDirector.LevelManager.levelUIController.MenuUp)
        {
            GameDirector.LevelManager.levelUIController.SetMenuToDown();
        }
        

        //Loads the selected level
        GameDirector.LevelManager.LoadLevel(LevelID);
    }
}
