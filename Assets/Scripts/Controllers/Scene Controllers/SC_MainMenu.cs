using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_MainMenu : SceneController
{

	// Use this for initialization
	void Start ()
    {
        Debug.Log(GameDirector.LevelManager.CurrentLevelID);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
