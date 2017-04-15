using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_ResetData : MonoBehaviour
{
    public void ResetLocalData()
    {
        GameDirector.LevelManager.ResetLocalData();
    }
}
