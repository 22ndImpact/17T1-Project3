using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbCounter : MonoBehaviour
{
    public GameObject NotchPrefab;

    public GameObject PerfectBar;
    public GameObject PassBar;
    public GameObject CompletionBar;
    public RectTransform Notches;
    public GameObject TrackerIcon;

    public List<GameObject> NotchList;

    public Vector3 CompletionTargetScale;
    public Vector3 TrackerTargetPosition;

    public float BarLerpSpeed;

    public Color ColourPerfect;
    public Color ColourPass;
    public Color ColourFail;
   

	// Use this for initialization
	void Start ()
    {
        //Set default targets
        CompletionTargetScale = CompletionBar.GetComponent<RectTransform>().localScale;
        TrackerTargetPosition = TrackerIcon.GetComponent<RectTransform>().localPosition;
	}

    // Update is called once per frame
    void Update()
    {
        //Update Progress bars
    }

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

            NotchList.Add(newNotch);
        }
    }
	
    void UpdateProgressBars()
    {
        //Determine Target 
        CompletionTargetScale.x = Mathf.Clamp01((float)GameDirector.LevelManager.CurrentLevel.orbsUsed / (float)GameDirector.LevelManager.CurrentLevel.passScore);
        //Lerp To Target
        CompletionBar.transform.localScale = Vector3.Lerp(CompletionBar.GetComponent<RectTransform>().localScale, CompletionTargetScale, BarLerpSpeed);

        //Determine Target
        TrackerTargetPosition.x = CompletionBar.GetComponent<RectTransform>().localPosition.x + (CompletionBar.GetComponent<RectTransform>().rect.width * CompletionBar.GetComponent<RectTransform>().localScale.x);
        //Adjustng the position to be in the middle of the block, not above a notch

        TrackerTargetPosition.x += Notches.rect.width * ((1 / (float)GameDirector.LevelManager.CurrentLevel.passScore)) / 2;

        //Follow lerped target
        TrackerIcon.GetComponent<RectTransform>().localPosition = TrackerTargetPosition;

        UpdateColour();
    }

    void UpdateColour()
    {
        if(GameDirector.LevelManager.CurrentLevel.orbsUsed < GameDirector.LevelManager.CurrentLevel.perfectScore)
        {
            TrackerIcon.GetComponent<Image>().color = ColourPerfect;
        }
        else if(GameDirector.LevelManager.CurrentLevel.orbsUsed < GameDirector.LevelManager.CurrentLevel.passScore)
        {
            TrackerIcon.GetComponent<Image>().color = ColourPass;
        }
        else
        {
            TrackerIcon.GetComponent<Image>().color = ColourFail;
        }
    }
}
