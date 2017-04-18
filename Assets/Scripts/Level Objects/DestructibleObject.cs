using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : ColouredObject
{
    #region Tweaking Variables
    public float FadeTime;
    public float yPositionBoundary;
    #endregion

    #region Tracking Variables
    bool Fading = false;
    public bool Active = true;
    #endregion

    #region Component References
    public AudioClip DeathNoise;
    #endregion

    void Update()
    {
        CheckIfBelowBoundary();
    }

    /// <summary>
    /// Checks if the block is below the yBoundary set and destroys it if it is
    /// </summary>
    void CheckIfBelowBoundary()
    {
        //Checks if the block is not lower than a certain cut off point, and if it is, Fade/Destroy it
        if (gameObject.transform.position.y <= yPositionBoundary)
        {
            //Check if your not already dead
            if (!Fading)
            {
                //Destroy the object
                StartCoroutine(FadeOutDestroy());

                //Set orbs used to lots
                GameDirector.LevelManager.CurrentLevel.AdjustOrbsUsed(99);

                GameDirector.LevelManager.CurrentLevel.ObjectsDestroyed(99);
            }
        }
    }

    void OnCollisionEnter(Collision _collision)
    {
        //If it collides with another coloured object
        if (_collision.gameObject.GetComponent<ColouredObject>() != null)
        {
            //If they are the same colour
            if (_collision.gameObject.GetComponent<ColouredObject>().objectColour == objectColour)
            {
                //Trigger the object fade
                if (!Fading)
                {
                    //Activte the fade
                    StartCoroutine(FadeOutDestroy());
                }
            }
        }
    }

    void OnCollisionStay(Collision _collision)
    {
        //If it collides with another coloured object
        if (_collision.gameObject.GetComponent<ColouredObject>() != null)
        {
            //If they are the same colour
            if (_collision.gameObject.GetComponent<ColouredObject>().objectColour == objectColour)
            {
                //Trigger the object fade
                if (!Fading)
                {
                    //Activte the fade
                    StartCoroutine(FadeOutDestroy());
                }
            }
        }
    }

    IEnumerator FadeOutDestroy()
    {
        //Activate sound
        GameDirector.audioController.PlayEffectClip(GameDirector.audioController.AudioCollection.DestructibleObjectDeath);

        //Set to fading, to stop double death situations
        Fading = true;

        //Set the object to be destroyed TODO make this an event system
        GameDirector.LevelManager.CurrentLevel.ObjectsDestroyed(1);

        //Stores the initial alpha of the object to lerp from
        float startignAlpha = GetComponent<MeshRenderer>().material.color.a;
        //Start a timer
        float timeTracker = FadeTime;

        while(timeTracker > 0)
        {
            //Store a temp colour to allow us to modify only the alpha
            Color tempColor = GetComponent<MeshRenderer>().material.color;
            //Lerp the alpha of the temp colour in time with the percentage of fade
            tempColor.a = Mathf.Lerp(0, startignAlpha, timeTracker / FadeTime);
            //Apply the temp colour back to the real material
            GetComponent<MeshRenderer>().material.color = tempColor;

            //Increase the completion by delta time
            timeTracker -= Time.smoothDeltaTime;

            yield return null;
        }
        Destroy(this.gameObject);
    }
}
