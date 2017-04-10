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

	// Use this for initialization
	void Start ()
    {
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
