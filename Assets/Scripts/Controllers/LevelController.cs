﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelController : MonoBehaviour
{
    #region Tracking Variables
    //Level Data Values
    public float reloadTime;
    public string LevelName;
    public int LevelID;
    public int passScore;
    public int perfectScore;
    public int bestScore;
    //Other tracking values
    public int orbsUsed;
    public float reloadTimer;
    public bool reloading;
    public int DestructibleObjectCount;
    public bool LevelOver;
    #endregion

    #region Tweaking Variables
    //The time it takes to end the level
    float EndOfLevelStall = 1f;
    #endregion

    #region Object References
    public LevelData levelData;
    public PrefabList PrefabList;
    private Transform Anchor;
    #endregion

    void Awake()
    {
        #region variable Initialization
        //Set the level over to false so the game doesnt think the previous level is still over
        LevelOver = false;
        //Resets the time scale back to 1, this is a fail safe incase of restarts during slow mo
        Time.timeScale = 1;
        //Takes the values form the scriptable object and applies them to the level
        InitializeFromLevelData();
        //Finds all the relevent object in the level
        FindObjects();
        #endregion
        #region External Object Initialization
        //Sets the current level based on scene name
        GameDirector.LevelManager.CurrentLevelID = GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentLevelSceneName);
        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();
        //Updates the level manager that this level is the current one
        GameDirector.LevelManager.CurrentLevel = this;
        #endregion
    }
	void Start ()
    {
        //Spawns the starting orb at the start of the level
        SpawnOrb();

        //Initialize a new level with the orb counter
        GameDirector.LevelManager.levelUIController.orbCounter.InitializeNewLevel(GameDirector.LevelManager.CurrentLevel);
    }
	void Update ()
    {
        //Updates the reload input check if reloading.
        if(reloading)
        {
            CheckForReload();
        }

        //Updates the ui progress bar, while in a level
        GameDirector.LevelManager.levelUIController.orbCounter.UpdateProgressBars();
    }

    /// <summary>
    /// Placeholder for an event system trigger for when an orb is shot in game
    /// </summary>
    public void OrbShot()
    {
        //Start the reload process
        reloading = true;
    }

    /// <summary>
    /// Alters the current levels OrbsUsed variable to track progress, also triggers end of game if the orbs used reaches 0.
    /// </summary>
    /// <param name="_OrbsUsed"></param>
    public void AdjustOrbsUsed(int _OrbsUsed)
    {
        orbsUsed += _OrbsUsed;

        //If the orbs used reaches the allowed amount end the game
        if(orbsUsed >= passScore)
        {
            if (LevelOver == false)
                StartCoroutine(EndLevel());
        }
    }

    /// <summary>
    /// Placeholder for an event system: Triggered when any destructible object is destroyed
    /// </summary>
    /// <param name="_NumObjects"></param>
    public void ObjectsDestroyed(int _NumObjects)
    {
        //Reduce the remaining amount of Destroyable objects by 1
        DestructibleObjectCount -= _NumObjects;

        //If you reach 0, trigger the end of the level
        if (DestructibleObjectCount <= 0)
        {
            if (LevelOver == false)
                StartCoroutine(EndLevel());
        }
    }

    /// <summary>
    /// Checks for player inpot to spawn a new orb
    /// </summary>
    void CheckForReload()
    {
        if(InputController.LeftMouseButtonDown && !InputController.IsPointerOverUIObject())
        {
            //Updates the used orbs by 1
            AdjustOrbsUsed(1);
            //Spawns the orb
            SpawnOrb();
            //Sets reloading to false
            reloading = false;
        }
    }

    /// <summary>
    /// Loads info from an external level data object
    /// </summary>
    void InitializeFromLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentLevelSceneName));
        //levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID);

        LevelName = levelData.LevelName;
        LevelID = levelData.LevelID;
        passScore = levelData.PassScore;
        perfectScore = levelData.PerfectScore;
        bestScore = levelData.BestScore;
    }

    /// <summary>
    /// Creats an orb at the anchor
    /// </summary>
    void SpawnOrb()
    {
        //Creating a new orb from the GameResources object
        GameObject newOrb = Instantiate<GameObject>(PrefabList.PlayerOrb);
        //Initializes the orb and feeds through level information
        newOrb.GetComponent<PlayerOrb>().Initialize(this, Anchor);
    }

    /// <summary>
    /// Finds all the relevent objects in the current scene
    /// </summary>
    void FindObjects()
    {
        Anchor = GameObject.FindGameObjectWithTag("Anchor").transform;
        DestructibleObjectCount = GameObject.FindGameObjectsWithTag("Destructible").Length;
    }

    /// <summary>
    /// Placeholder for event system: Triggered when the player has destroyed all the objects
    /// </summary>
    /// <returns></returns>
    IEnumerator EndLevel()
    {
        LevelOver = true;

        //Slow Time TODO change hardcoded value
        Time.timeScale = 0.1f;

        #region Stall while in slow mo
        //Set a timer
        float StallTimer = EndOfLevelStall * Time.timeScale;

        //Stall while timer counts down
        while(StallTimer > 0 )
        {
            yield return null;
            StallTimer -= Time.deltaTime;
        }
        #endregion

        #region End of level proceadures
        //Reset the time scale from the slow mo
        Time.timeScale = 1;
        UpdateBestScore();
        SaveLevelData();

        //Replace this with the UI panel lerping upwards
        GameDirector.LevelManager.levelUIController.StartLevelClosingTransition();
        //GameDirector.SceneManager.ChangeScene(GD_SceneManager.SceneList.LevelComplete);
        #endregion
    }

    /// <summary>
    /// Saves the level data to the external save file
    /// </summary>
    public void SaveLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentLevelSceneName));

        //Locating level data object in global list
        //levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID);

        //Update level stats
        levelData.BestScore = bestScore;

        //Save to external file
        GameDirector.LevelManager.SaveLevelData();
    }

    /// <summary>
    /// Checks current orbs used towards the best score
    /// </summary>
    public void UpdateBestScore()
    {
        //Updating best score if it is better
        if (orbsUsed < bestScore)
        {
            bestScore = orbsUsed;
        }

        //Check if the level is completed, and is so unlock the next one
        if(bestScore < passScore)
        {
            //Unloacks the next level
            GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID + 1).Unlocked = true;
        }
    }
    
    /// <summary>
    /// Placeholder for audio system: Plays an audio clip
    /// </summary>
    /// <param name="_clip"></param>
    public void PlaySound(AudioClip _clip)
    {
        GetComponent<AudioSource>().PlayOneShot(_clip);
    }

}


