using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_SelectLevel : MonoBehaviour
{
    //The ID of the level that this 
    public int LevelID;

    public string UnloackedText;
    public string LockedText;

    public Color ColourLocked;
    public Color ColourUnlocked;
    public Color ColourPassed;
    public Color ColourPerfect;

    public Text text;
    public Image image;

    void Start()
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
            text.text = UnloackedText;
            image.color = ColourUnlocked;
        }

        //Changes colour to passed or perfect if needed
        if(GameDirector.LevelManager.GetLevelData(LevelID).BestScore < GameDirector.LevelManager.GetLevelData(LevelID).PerfectScore)
        {
            image.color = ColourPerfect;
        }
        else if (GameDirector.LevelManager.GetLevelData(LevelID).BestScore < GameDirector.LevelManager.GetLevelData(LevelID).PassScore)
        {
            image.color = ColourPassed;
        }
    }

    public void LoadLevel()
    {
        GameDirector.LevelManager.LoadLevel(LevelID);
    }
}
