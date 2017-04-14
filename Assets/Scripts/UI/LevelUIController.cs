using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUIController : MonoBehaviour
{
    //Tweaking Values
    public float TransitionTime;
    public AnimationCurve TransitionCurve;

    //Object references
    public GameObject GameUIPanel;
    public Button_LevelCompleteContinue LevelCompleteButton;
    public CompletionImage LevelCompletionImage;
    public CompletionText LevelCompletionText;

    //The top and bottom values of the end game panel at the start of the level
    public Vector3 UIStartingPosition;

    //Tracking Values
    public float TransitionTimer;
    public bool Transitioning = false;

    void Start()
    {
        UIStartingPosition = GameUIPanel.transform.localPosition;
    }

    void Update()
    {
        if(Transitioning)
        {
            UpdateEndGameTransition();
        }
    }

    public void StartEndGameTrainsition()
    {
        TransitionTimer = TransitionTime;
        Transitioning = true;

        LevelCompleteButton.UpdateState();
        LevelCompletionImage.UpdateState();
        LevelCompletionText.UpdateState();
    }

    public void UpdateEndGameTransition()
    {

        //Reduce Transition Timer
        TransitionTimer -= Time.smoothDeltaTime;

        //Change position based on animation curve
        GameUIPanel.transform.localPosition = new Vector3(UIStartingPosition.x,
                                                          UIStartingPosition.y  - (UIStartingPosition.y * (TransitionCurve.Evaluate( 1- (TransitionTimer / TransitionTime)))),
                                                          UIStartingPosition.z);
    }


}
