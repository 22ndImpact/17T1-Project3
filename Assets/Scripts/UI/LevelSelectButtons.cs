﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectButtons : MonoBehaviour
{
    public GameObject levelSelectButtonPrefab;

	void Start ()
    {
        InitializeLevelSelectButtons();
	}

    public void InitializeLevelSelectButtons()
    {
        for (int i = 0; i < GameDirector.LevelManager.LevelDataList.Count; i++)
        {
            GameObject newButton = GameObject.Instantiate(levelSelectButtonPrefab);
            //Set the properties of the button
            newButton.GetComponent<Button_SelectLevel>().LevelID = GameDirector.LevelManager.LevelDataList[i].LevelID;
            newButton.GetComponent<Button_SelectLevel>().UnlockedText = GameDirector.LevelManager.LevelDataList[i].LevelID.ToString();
            newButton.GetComponent<Button_SelectLevel>().LockedText = GameDirector.LevelManager.LevelDataList[i].LevelID.ToString();
            //Set the position of the buttons
            newButton.transform.SetParent(this.transform, false);
        }


        //Scans for all the buttons and adds them to the gameDirectors list
        for (int i = 0; i < transform.childCount; i++)
        {
            GameDirector.LevelManager.LevelSelectButtons.Add(transform.GetChild(i).GetComponent<Button_SelectLevel>());
        }
    }
}
