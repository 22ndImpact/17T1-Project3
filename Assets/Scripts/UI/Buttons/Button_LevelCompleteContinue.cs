using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelCompleteContinue : MonoBehaviour
{
    public string UnloackedText;
    public string LockedText;

    public Text text;

    public void UpdateState()
    {   
        if (GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID + 1).Unlocked == false)
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

    //Different to a normal scene change. This will unload the old level and load the new one, without destroying the other loaded scenes (LevelUI)
    public void LoadLevel()
    {
        GameDirector.LevelManager.UnloadLevel(GameDirector.LevelManager.CurrentLevelID);
        GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID + 1);

        //Transition the new level In
        GameDirector.LevelManager.levelUIController.StartLevelOpeningTransition();
    }
}
