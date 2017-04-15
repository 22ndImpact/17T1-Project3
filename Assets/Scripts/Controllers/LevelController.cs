using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelController : MonoBehaviour
{
    //Object References
    public LevelData levelData;
    [SerializeField] private PrefabList PrefabList;
    private Transform Anchor;
    public LevelUIController levelUIController;
    

    //Level Data Values
    [SerializeField] private float reloadTime;
    public string LevelName;
    public int LevelID;
    public int passScore;
    public int perfectScore;
    public int bestScore;

    //Tracking Values
    public int orbsUsed;
    public float reloadTimer;
    public bool reloading;
    public int DestructibleObjectCount;
    public bool LevelOver;

    //The time it takes to end the level
    float EndOfLevelStall = 1f;

    #region Mono Behaviour Events
    void Awake()
    {
        //Set the level over to false so the game doesnt think the previous level is still over
        LevelOver = false;

        //Resets the time scale back to 1, this is a fail safe incase of restarts during slow mo
        Time.timeScale = 1;

        //Sets the current level based on scene name
        GameDirector.LevelManager.CurrentLevelID = GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrenLevelSceneName);

        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();

        //Updates the level manager that this level is the current one
        GameDirector.LevelManager.CurrentLevel = this;

        //Takes the values form the scriptable object and applies them to the level
        InitializeFromLevelData();

        //Finds all the relevent object in the level
        FindObjects();

        //Attempt to find the level UI
        if(GameObject.FindGameObjectWithTag("LevelUI") != null)
        {
            levelUIController = GameObject.FindGameObjectWithTag("LevelUI").GetComponent<LevelUIController>();
        }

        
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
        //updates the reload input check if reloading.
        if(reloading)
        {
            CheckForReload();
        }
    }
    #endregion

    public void StopLevel()
    {

    }

    //Triggered by the orb when it fires
    public void OrbShot()
    {
        //Start the reload process
        reloading = true;
    }

    //TODO Make event system
    public void AdjustOrbsUsed(int _OrbsUsed)
    {
        orbsUsed += _OrbsUsed;


        if(orbsUsed >= passScore)
        {
            if (LevelOver == false)
                StartCoroutine(EndLevel());
        }
    }

    //TODO make event system 
    //Triggered when any destructible object is destroyed
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

    //Check for player inpotu to spawn an orb
    void CheckForReload()
    {
        if(InputController.LeftMouseButtonDown && !InputController.IsPointerOverUIObject())
        {
            //Updates the used orbs by 1
            AdjustOrbsUsed(1);
            SpawnOrb();
            reloading = false;
        }
    }

    //Copies info from the external level data object
    void InitializeFromLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrenLevelSceneName));
        //levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID);

        LevelName = levelData.LevelName;
        LevelID = levelData.LevelID;
        passScore = levelData.PassScore;
        perfectScore = levelData.PerfectScore;
        bestScore = levelData.BestScore;
    }

    //Creats an orb at the anchor
    void SpawnOrb()
    {
        //Creating a new orb from the GameResources object
        GameObject newOrb = Instantiate<GameObject>(PrefabList.PlayerOrb);
        //Initializes the orb and feeds through level information
        newOrb.GetComponent<PlayerOrb>().Initialize(this, Anchor);
    }

    //Finds all the relevent objects in the current scene
    void FindObjects()
    {
        Anchor = ObjectFinder.Anchor;
        DestructibleObjectCount = GameObject.FindGameObjectsWithTag("Destructible").Length;
        levelUIController = ObjectFinder.levelUIController;
    }

    //Triggered when the player has destroyed all the objects
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

    //Saves the level data to the external save file
    public void SaveLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrenLevelSceneName));

        //Locating level data object in global list
        //levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID);

        //Update level stats
        levelData.BestScore = bestScore;

        //Save to external file
        GameDirector.LevelManager.SaveLevelData();
    }

    //Checks current orbs used towards the best score
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
            Debug.Log("unlock next level");
            UnlockNextLevel();
        }
    }

    public void UnlockNextLevel()
    {
        GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID + 1).Unlocked = true;
    }

    public void PlaySound(AudioClip _clip)
    {
        GetComponent<AudioSource>().PlayOneShot(_clip);
    }

}


