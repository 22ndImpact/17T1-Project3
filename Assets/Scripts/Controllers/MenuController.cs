using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    #region Tracking Variables
    public bool TransitioningMenu = false;
    #endregion

    #region Tweaking Variables
    public AnimationCurve MenuTransitionCurve;
    public float MenuTransitionTime;
    #endregion

    #region Object References
    public RectTransform MenuPanel;
    public RectTransform WorldsPanel;
    public Button NextWorldButton;
    public Button PreviousWorldButton;
    #endregion

	void Awake ()
    {
        //Attaches the menu to the GameDirector
        GameDirector.menuController = this;

        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();
    }

    //used to activate and shift between different menu objects
    #region Transition Functions

    public void ActivateMainMenu()
    {
        if(!TransitioningMenu)
        {
            //Set the position of the world panel to the current world
            //WorldsPanel.localPosition = new Vector3(0, 0, 0);

            //Transition the global menu to the main menu
            StartCoroutine(TransitionPanel(MenuPanel.localPosition, new Vector3(0, 0, 0), MenuPanel));

            
        }
        
    }

    public void ActivateLevelSelect()
    {
        if(!TransitioningMenu)
        {
            //Refreshes the level info on the buttons
            GameDirector.LevelManager.UpdateLevelButtonInfo();
            //change to switch statment if positions change
            StartCoroutine(TransitionPanel(MenuPanel.localPosition, new Vector3(-1440, 0, 0), MenuPanel));

            //Set the position of the world panel to the current world
            WorldsPanel.localPosition = new Vector3(-1440 * (GameDirector.LevelManager.CurrentWorld - 1), 0, 0);
        }
    }
    public void ActivateGamePlay()
    {
        if (!TransitioningMenu)
        {
            //Move the global menu up, to reveal the game play screen
            StartCoroutine(TransitionPanel(MenuPanel.localPosition, new Vector3(MenuPanel.localPosition.x, 2560, MenuPanel.localPosition.z), MenuPanel));
        }
    }

    public void NextWorld()
    {
        //Set the new world
        GameDirector.LevelManager.ChangeWorld(GameDirector.LevelManager.CurrentWorld + 1);
        //Transition the worlds panel to that new world
        StartCoroutine(TransitionPanel(WorldsPanel.localPosition, new Vector3(-1440 * (GameDirector.LevelManager.CurrentWorld - 1), WorldsPanel.localPosition.y, WorldsPanel.localPosition.z), WorldsPanel));
    }

    public void PrevoiusWorld()
    {
        GameDirector.LevelManager.ChangeWorld(GameDirector.LevelManager.CurrentWorld - 1);
        //Transition the worlds panel to that new world
        StartCoroutine(TransitionPanel(WorldsPanel.localPosition, new Vector3(-1440 * (GameDirector.LevelManager.CurrentWorld - 1), WorldsPanel.localPosition.y, WorldsPanel.localPosition.z), WorldsPanel));
    }

    IEnumerator TransitionPanel(Vector3 _startingPosition, Vector3 _targetPosition, RectTransform _PanelTransform)
    {
        TransitioningMenu = true;
        //MenuPanel.localPosition = _targetPosition;

        float MenuTransitionTimer = MenuTransitionTime;

        while (MenuTransitionTimer > 0)
        {
            //Change position based on animation curve
            _PanelTransform.localPosition = Vector3.Lerp(_startingPosition, _targetPosition, MenuTransitionCurve.Evaluate(MenuTransitionTimer / MenuTransitionTime));

            //Decrease the timer
            MenuTransitionTimer -= Time.smoothDeltaTime;

            yield return null;
        }

        //Final snapping to target position to account for delta time inacuracies
        _PanelTransform.localPosition = _targetPosition;

        TransitioningMenu = false;
    }
    #endregion

    public void UpdateWorldChangeButtons()
    {


        //Enable previous world if there is a world lower than this one
        if(GameDirector.LevelManager.Worlds.Contains(GameDirector.LevelManager.CurrentWorld -1))
        {
            PreviousWorldButton.interactable = true;
        }
        else
        {
            PreviousWorldButton.interactable = false;
        }

        //Enable previous world if there is a world lower than this one
        if (GameDirector.LevelManager.Worlds.Contains(GameDirector.LevelManager.CurrentWorld + 1))
        {
            NextWorldButton.interactable = true;
        }
        else
        {
            NextWorldButton.interactable = false;
        }
    }
}
