using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Object References
    public LevelData levelData;
    [SerializeField] private PrefabList PrefabList;
    private Transform Anchor;
    

    //Level Data Values
    [SerializeField] private float reloadTime;
    public int passScore;
    public int perfectScore;
    public int bestScore;

    //Tracking Values
    public int orbsUsed;
    public float reloadTimer;
    public bool reloading;
    public int DestructibleObjectCount;

    //The time it takes to end the level
    float EndOfLevelStall = 1f;

    #region Mono Behaviour Events
    void Awake()
    {
        //Resets the time scale back to 1, this is a fail safe incase of restarts during slow mo
        Time.timeScale = 1;

        //Sets the current level based on scene name
        GameDirector.LevelManager.CurrentLevelID = GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentSceneName);

        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();

        //Updates the level manager that this level is the current one
        GameDirector.LevelManager.CurrentLevel = this;



        //Takes the values form the scriptable object and applies them to the level
        InitializeFromLevelData();

        //Finds all the relevent object in the level
        FindObjects();
    }
	void Start ()
    {
        //Spawns the starting orb at the start of the level
        SpawnOrb();
	}
	void Update ()
    {
        //updates the reload timer if reloading.
        UpdateReloadTimer();
    }
    #endregion

    //Triggered by the orb when it fires
    public void OrbShot()
    {
        //Start the reload process
        Reload();

        //Increase the Orbs used by 1
        AdjustOrbsUsed(1);
    }

    public void AdjustOrbsUsed(int _OrbsUsed)
    {
        orbsUsed += _OrbsUsed;
    }

    //Triggered when any destructible object is destroyed
    public void ObjectDestroyed(DestructibleObject _Object)
    {
        //Reduce the remaining amount of Destroyable objects by 1
        DestructibleObjectCount -= 1;

        //If you reach 0, trigger the end of the level
        if (DestructibleObjectCount == 0)
        {
            StartCoroutine(EndLevel());
        }
    }

    //Resets the timer and sets reloading to True
    public void Reload()
    {
        //Reset the timer back to 0
        reloadTimer = reloadTime;
        //Notify the class you are reloading
        reloading = true;
    }

    //Counts the Reload Timer down, and spawns an orb after completion
    void UpdateReloadTimer()
    {
        if (reloading)
        {
            //If you are still havent reached 0 keep counting
            if (reloadTimer > 0)
            {
                reloadTimer -= Time.deltaTime;
            }
            //Once you get to 0, spawn the orb and turn off reloading
            else
            {
                SpawnOrb();
                reloading = false;
            }
                
        }
    }

    //Copies info from the external level data object
    void InitializeFromLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentSceneName));
        //levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.CurrentLevelID);

        reloadTime = levelData.ReloadTime;
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
    }

    //Triggered when the player has destroyed all the objects
    IEnumerator EndLevel()
    {
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
        GameDirector.SceneManager.ChangeScene(GD_SceneManager.SceneList.LevelComplete);
        #endregion
    }

    //Saves the level data to the external save file
    public void SaveLevelData()
    {
        //Debug line that gets the level ID from the scene name itself instead of the level manager. This is so the level can run before the "Levelload" fucntion has been called
        levelData = GameDirector.LevelManager.GetLevelData(GameDirector.LevelManager.GetLevelIDFromScene(GameDirector.SceneManager.CurrentSceneName));

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
        if(bestScore <= passScore)
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


