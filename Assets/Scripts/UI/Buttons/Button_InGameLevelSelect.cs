using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_InGameLevelSelect : MonoBehaviour
{
    public void ReturnToLevelSelect()
    {
        //Activate the level select
        GameDirector.menuController.ActivateLevelSelect();
    }
}
