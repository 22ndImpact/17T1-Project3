using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbCounter : MonoBehaviour
{

    #region Tweaking Variables
    public Color ColourPerfect;
    public Color ColourPass;
    public Color ColourFail;
    public float BarLerpSpeed;
    #endregion

    #region Tracking Variables
    public List<GameObject> NotchList;
    public Vector3 CompletionTargetScale;
    public Vector3 TrackerTargetPosition;
    #endregion

    #region Object References
    public GameObject NotchPrefab;
    public GameObject PerfectBar;
    public GameObject PassBar;
    public GameObject CompletionBar;
    public RectTransform Notches;
    public GameObject TrackerIcon;
    #endregion

	void Start ()
    {
        //Set default targets
        CompletionTargetScale = CompletionBar.GetComponent<RectTransform>().localScale;
        TrackerTargetPosition = TrackerIcon.GetComponent<RectTransform>().localPosition;
	}

    /// <summary>
    /// Used to set the length of the completion bars and the position of the notches
    /// </summary>
    /// <param name="levelToInitialize"></param>
    public void InitializeNewLevel(LevelController levelToInitialize)
    {
        //Destroys all the notches form the previous notch list
        foreach(GameObject notch in NotchList)
        {
            Destroy(notch);
        }

        //Clears the list of references
        NotchList.Clear();

        //Set the completion bar to 0 scale
        CompletionBar.transform.localScale = new Vector3(0, 1, 1);

        int PassScore = levelToInitialize.passScore;
        int PerfectScore = levelToInitialize.perfectScore;

        //Set the PassBar to the right scale based on pass score
        //Remainder of perfect score: 1 - PerfectScore/PassScore
        PassBar.transform.localScale = new Vector3(1 - (float)PerfectScore / (float)PassScore, 1, 1);

        //Create a bunch of notches to break up the bar based on pass score
        for (int i = 0; i < levelToInitialize.passScore + 1; i++)
        {
            GameObject newNotch = Instantiate(NotchPrefab);
            newNotch.GetComponent<RectTransform>().SetParent(Notches, false);

            //Adjust spacing based on total number of notches

            float XPosition = (Notches.rect.width * ((float)i / (float)PassScore)) - Notches.rect.width;

            newNotch.GetComponent<RectTransform>().localPosition = new Vector3(XPosition, 0, 0);

            //Update notch state
            if(i == PerfectScore)
            {
                newNotch.GetComponent<Image>().color = ColourPerfect;
                newNotch.GetComponent<RectTransform>().localScale = newNotch.GetComponent<RectTransform>().localScale * 1.2f;
            }

            NotchList.Add(newNotch);
        }
    }
	
    /// <summary>
    /// Updates the position of the progress bars and tracker based on level progress
    /// </summary>
    public void UpdateProgressBars()
    {
        //Determine Target 
        CompletionTargetScale.x = Mathf.Clamp01((float)GameDirector.LevelManager.CurrentLevel.orbsUsed / (float)GameDirector.LevelManager.CurrentLevel.passScore);
        //Lerp To Target
        CompletionBar.transform.localScale = Vector3.Lerp(CompletionBar.GetComponent<RectTransform>().localScale, CompletionTargetScale, BarLerpSpeed);

        //Determine Target
        TrackerTargetPosition.x = CompletionBar.GetComponent<RectTransform>().localPosition.x + (CompletionBar.GetComponent<RectTransform>().rect.width * CompletionBar.GetComponent<RectTransform>().localScale.x);
        //Adjustng the position to be in the middle of the block, not above a notch

        //TrackerTargetPosition.x += Notches.rect.width * ((1 / (float)GameDirector.LevelManager.CurrentLevel.passScore)) / 2;

        //Follow lerped target
        TrackerIcon.GetComponent<RectTransform>().localPosition = TrackerTargetPosition;

        UpdateColour();
    }

    /// <summary>
    /// Updates the colour of the tracker based on level progress
    /// </summary>
    void UpdateColour()
    {
        if (GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.perfectScore)
        {
            TrackerIcon.GetComponent<Image>().color = ColourPerfect;
        }
        else if (GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.passScore)
        {
            TrackerIcon.GetComponent<Image>().color = ColourPass;
        }
        else
        {
            TrackerIcon.GetComponent<Image>().color = ColourFail;
        }
    }
}
