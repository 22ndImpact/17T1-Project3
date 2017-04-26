using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLevelPanels : MonoBehaviour
{
    public List<int> WorldsList;

    public GameObject WorldPanel;

    void Start()
    {
        InitializeWorldPanels();
    }

    public void InitializeWorldPanels()
    {
        for (int i = 0; i < GameDirector.LevelManager.LevelDataList.Count; i++)
        {
            if (!WorldsList.Contains(GameDirector.LevelManager.LevelDataList[i].LevelWorld))
            {
                WorldsList.Add(GameDirector.LevelManager.LevelDataList[i].LevelWorld);

                //Create the worl dpanel object
                GameObject newWorldPanel = GameObject.Instantiate(WorldPanel);
                //Set the position of the buttons
                newWorldPanel.transform.SetParent(this.transform, false);

                newWorldPanel.GetComponent<WorldPanel>().InitializeWorldPanel(GameDirector.LevelManager.LevelDataList[i].LevelWorld, "World " + GameDirector.LevelManager.LevelDataList[i].LevelWorld.ToString());
            }
        }

        GameDirector.LevelManager.Worlds = WorldsList;
        GameDirector.LevelManager.ChangeWorld(1);
    }
}
