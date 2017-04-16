using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    #region Tracking Variables
    public float TransitionTimer;
    public bool TransitioningOut = false;
    public bool TransitioningIn = false;
    public bool MenuUp = false;
    public Vector3 UIStartingPosition;
    #endregion

    #region Tweaking Values
    float TransitionTime;
    public AnimationCurve TransitionOutCurve;
    public AnimationCurve TransitionInCurve;
    #endregion

    #region Object References
    public GameObject GameUIPanel;
    public Button_LevelCompleteContinue LevelCompleteButton;
    public CompletionImage LevelCompletionImage;
    public CompletionText LevelCompletionText;
    public OrbCounter orbCounter;
    #endregion

    void Awake()
    {
        //Link this level UI to the game director
        GameDirector.LevelManager.levelUIController = this;
    }

    void Start()
    {
        //Getting the menu transition timer from the game director
        TransitionTime = GameDirector.menuController.MenuTransitionTime;

        //Store the starting position of the menu
        UIStartingPosition = GameUIPanel.transform.localPosition;
    }

    void Update()
    {
        if(TransitioningOut)
        {
            UpdateClosingTransition();
        }

        if(TransitioningIn)
        {
            UpdateOpeningTransition();
        }
    }

    //An inelegent solution for transition the position of the menu panel up and down based on game state. Should be replace by unity animations or at least coroutines.
    #region menu Transition Code
    
    /// <summary>
    /// Triggered when finishing a level to make the menu slide upward
    /// </summary>
    public void StartLevelClosingTransition()
    {
        TransitionTimer = TransitionTime;
        TransitioningOut = true;

        LevelCompleteButton.UpdateState();
        LevelCompletionImage.UpdateState();
        LevelCompletionText.UpdateState();
    }

    /// <summary>
    /// Triggered when leaving the end level menu to make it slide downward
    /// </summary>
    public void StartLevelOpeningTransition()
    {
        TransitionTimer = TransitionTime;
        TransitioningIn = true;
    }

    public void UpdateClosingTransition()
    {

        //Reduce Transition Timer
        TransitionTimer -= Time.smoothDeltaTime;

        //Change position based on animation curve
        GameUIPanel.transform.localPosition = new Vector3(UIStartingPosition.x,
                                                          UIStartingPosition.y  - (UIStartingPosition.y * (TransitionOutCurve.Evaluate( 1- (TransitionTimer / TransitionTime)))),
                                                          UIStartingPosition.z);

        //checking if the transtiion is complete
        if(TransitionTimer <= 0)
        {
            MenuUp = true;
            TransitioningOut = false;
        }
    }

    public void UpdateOpeningTransition()
    {

        //Reduce Transition Timer
        TransitionTimer -= Time.smoothDeltaTime;

        //Change position based on animation curve
        GameUIPanel.transform.localPosition = new Vector3(UIStartingPosition.x,
                                                          UIStartingPosition.y - (UIStartingPosition.y * (TransitionInCurve.Evaluate(1 - (TransitionTimer / TransitionTime)))),
                                                          UIStartingPosition.z);

        //checking if the transtiion is complete
        if (TransitionTimer <= 0)
        {
            MenuUp = false;
            TransitioningIn = false;
        }
    }

    /// <summary>
    /// Used to instantly set the menu to the down position
    /// </summary>
    public void SetMenuToDown()
    {
        GameUIPanel.transform.localPosition = UIStartingPosition;

        MenuUp = false;
    }

    /// <summary>
    /// Used to instantly set the menu to the up position
    /// </summary>
    public void SetMenuToUp()
    {
        GameUIPanel.transform.localPosition = Vector3.zero;
        MenuUp = true;
    }

    #endregion
}
