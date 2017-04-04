using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SelectLevel : MonoBehaviour
{
    //The ID of the level that this 
    public int LevelID;

    public void LoadLevel()
    {
        //Changes the Level Managers current level ID to the respective level given by the button. This is done so the game play scene will know what level to load
        GameDirector.LevelManager.CurrentLevelID = LevelID;
        //Changes to the Gameplay Scene
        GameDirector.SceneManager.ChangeScene(GD_SceneManager.SceneList.GamePlay);
    }
}
