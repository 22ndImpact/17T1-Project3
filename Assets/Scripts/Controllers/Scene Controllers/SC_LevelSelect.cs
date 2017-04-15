 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LevelSelect : SceneController
{
    void Awake()
    {
        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();
    }
}
