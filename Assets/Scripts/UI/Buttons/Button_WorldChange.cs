using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_WorldChange : MonoBehaviour
{
    public int World;

    void Awake()
    {
        //Adds this button to the list of world change buttons
        GameDirector.LevelManager.WorldChangeButtons.Add(this);
    }

    public void ChangeWorld()
    {
        GameDirector.menuController.ActivateLevelSelect(World);
    }
}
