using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour {

    #region Tracking Variables
    public float startingAlpha;
    #endregion

    #region Tweaking Variables
    public float FadeTime;
    #endregion

    void Start()
    {
        //Stores the initial alpha of the object to lerp from
        startingAlpha = GetComponent<MeshRenderer>().material.color.a;
    }

    public void TurnOff()
    {
        //Store a temp colour to allow us to modify only the alpha
        Color tempColor = GetComponent<MeshRenderer>().material.color;
        //Lerp the alpha of the temp colour in time with the percentage of fade
        tempColor.a = 0;
        //Apply the temp colour back to the real material
        GetComponent<MeshRenderer>().material.color = tempColor;
    }

    public IEnumerator FadeIn()
    {
        //Start a timer
        float timeTracker = FadeTime;

        while (timeTracker > 0)
        {
            Debug.Log("Fading: " + (1 - timeTracker / FadeTime));

            //Store a temp colour to allow us to modify only the alpha
            Color tempColor = GetComponent<MeshRenderer>().material.color;
            //Lerp the alpha of the temp colour in time with the percentage of fade
            tempColor.a = Mathf.Lerp(0, startingAlpha, 1- (timeTracker / FadeTime));
            //Apply the temp colour back to the real material
            GetComponent<MeshRenderer>().material.color = tempColor;

            //Increase the completion by delta time
            timeTracker -= Time.smoothDeltaTime;

            yield return null;
        }
    }
}
