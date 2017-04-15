using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public RectTransform MenuPanel;

    public bool TransitioningMenu = false;

    public float MenuTransitionTime;
  
    public AnimationCurve MenuTransitionCurve;

	// Use this for initialization
	void Awake ()
    {
        //Attaches the menu to the GameDirector
        GameDirector.menuController = this;

        //Attempts to populate the level list
        GameDirector.LevelManager.PopulateLevelList();
    }

    public void ActivateMainMenu()
    {
        if(!TransitioningMenu)
        {
            StartCoroutine(TransitionMenu(MenuPanel.localPosition, new Vector3(0, 0, 0)));
        }
        
    }

    public void ActivateLevelSelect()
    {
        if(!TransitioningMenu)
        {
            //Refreshes the level info on the buttons
            GameDirector.LevelManager.UpdateLevelButtonInfo();
            
            StartCoroutine(TransitionMenu(MenuPanel.localPosition, new Vector3(-1440, 0, 0)));
        }
    }

    public void ActivateGamePlay()
    {
        if(!TransitioningMenu)
        {
            StartCoroutine(TransitionMenu(MenuPanel.localPosition, new Vector3(-1440, 2560, 0)));
        }
    }

    IEnumerator TransitionMenu(Vector3 _startingPosition, Vector3 _targetPosition)
    {
        Debug.Log("StartingTransition");

        TransitioningMenu = true;
        MenuPanel.localPosition = _targetPosition;

        float MenuTransitionTimer = MenuTransitionTime;

        while (MenuTransitionTimer > 0)
        {
            Debug.Log("TransitioningMenu");
            //Change position based on animation curve
            MenuPanel.localPosition = Vector3.Lerp(_startingPosition, _targetPosition, MenuTransitionCurve.Evaluate(MenuTransitionTimer / MenuTransitionTime));

            //Decrease the timer
            MenuTransitionTimer -= Time.smoothDeltaTime;

            yield return null;
        }

        //Final snapping to target position to account for delta time inacuracies
        MenuPanel.localPosition = _targetPosition;

        TransitioningMenu = false;
    }
}
