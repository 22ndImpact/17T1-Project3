using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    //Object References
    [SerializeField] private LevelData LevelData;
    [SerializeField] private PrefabList PrefabList;
    private Transform Anchor;

    //Level Data Values
    [SerializeField] private float reloadTime;
    public int passScore;
    public int perfectScore;

    //Tracking Values
    public float orbsUsed;
    public float reloadTimer;
    public bool reloading;

    #region Mono Behaviour Events
    void Awake()
    {
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
        Reload();
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
        reloadTime = LevelData.ReloadTime;
        passScore = LevelData.PassScore;
        perfectScore = LevelData.PerfectScore;
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
    }
}
