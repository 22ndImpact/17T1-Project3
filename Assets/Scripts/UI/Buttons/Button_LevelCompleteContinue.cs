using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_LevelCompleteContinue : MonoBehaviour
{
    public string UnloackedText;
    public string LockedText;

    public Text text;

    void Start()
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

    public void LoadLevel()
    {
        GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID + 1);
    }
}
