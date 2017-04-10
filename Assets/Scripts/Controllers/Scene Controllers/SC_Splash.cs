﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Splash : SceneController
{

    [SerializeField] private float WaitTime;
    private float WaitTimer;

    void Start()
    {
        ResetTimer();
    }

	void Update ()
    {
        UpdateTimer();
	}

    void LoadMainMenu()
    {
        GameDirector.SceneManager.ChangeScene(GD_SceneManager.SceneList.MainMenu);
    }

    void ResetTimer()
    {
        WaitTimer = WaitTime;
    }

    void UpdateTimer()
    {
        //Check player input to cancel the countdown short
        if (GameDirector.InputManager.LeftClickDown)
            WaitTimer = 0;

        //Update and check timer to activate LoadMainMenu
        if (WaitTimer <= 0)
            LoadMainMenu();
        else
            WaitTimer -= Time.deltaTime;
    }
}
