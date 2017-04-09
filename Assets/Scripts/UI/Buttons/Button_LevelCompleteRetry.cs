using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_LevelCompleteRetry : MonoBehaviour
{
    public void LoadLevel()
    {
        GameDirector.LevelManager.LoadLevel(GameDirector.LevelManager.CurrentLevelID);
    }
}
