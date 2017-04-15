using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButtons : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //Scans for all the buttons and adds them to the gameDirectors list
        for (int i = 0; i < transform.childCount; i++)
        {
            GameDirector.LevelManager.LevelSelectButtons.Add(transform.GetChild(i).GetComponent<Button_SelectLevel>());
        }
	}
}
