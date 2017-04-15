using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    //Tweaking Values
    public float TransitionTime;
    public AnimationCurve TransitionOutCurve;
    public AnimationCurve TransitionInCurve;

    //Object references
    public GameObject GameUIPanel;
    public Button_LevelCompleteContinue LevelCompleteButton;
    public CompletionImage LevelCompletionImage;
    public CompletionText LevelCompletionText;
    public OrbCounter orbCounter;

    //The top and bottom values of the end game panel at the start of the level
    public Vector3 UIStartingPosition;

    //Tracking Values
    public float TransitionTimer;
    public bool TransitioningOut = false;
    public bool TransitioningIn = false;

    public bool MenuUp = false;

    void Awake()
    {
        //Set this level ui to be level UI of the current level
        GameDirector.LevelManager.levelUIController = this;
    }

    void Start()
    {
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

    public void StartLevelClosingTransition()
    {
        //Set the starting position
        //-UIStartingPosition = GameUIPanel.transform.localPosition;

        TransitionTimer = TransitionTime;
        TransitioningOut = true;

        LevelCompleteButton.UpdateState();
        LevelCompletionImage.UpdateState();
        LevelCompletionText.UpdateState();
    }

    public void StartLevelOpeningTransition()
    {
        //Set the starting position
        //UIStartingPosition = GameUIPanel.transform.localPosition;

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

    public void SetMenuToDown()
    {
        GameUIPanel.transform.localPosition = new Vector3(UIStartingPosition.x,
                                                          UIStartingPosition.y,
                                                          UIStartingPosition.z);

        MenuUp = false;
    }

    public void SetMenuToUp()
    {
        GameUIPanel.transform.localPosition = Vector3.zero;
        MenuUp = true;
    }

}
