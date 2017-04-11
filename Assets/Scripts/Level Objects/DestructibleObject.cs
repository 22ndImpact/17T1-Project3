using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : ColouredObject
{
    public float FadeTime;
    public float yPositionBoundary;
    bool Fading = false;

    void Update()
    {
        //Checks if the block is not lower than a certain cut off point, and if it is, Fade/Destroy it
        if(gameObject.transform.position.y <= yPositionBoundary)
        {
            //Check if your not already dead
            if (!Fading)
            {
                StartCoroutine(FadeOutDestroy());
            }
        }
    }

    void OnCollisionEnter(Collision _collision)
    {
        //If it collides with another coloured object
        if(_collision.gameObject.GetComponent<ColouredObject>() != null)
        {
            //If they are the same colour
            if(_collision.gameObject.GetComponent<ColouredObject>().objectColour == objectColour)
            {
                //Trigger the object fade
                if (!Fading)
                {
                    StartCoroutine(FadeOutDestroy());
                }
                //TODO Trigger the object fade to desctuction
                //TODO when the level ends, slow-mo into fad out
            }
        }
    }

    IEnumerator FadeOutDestroy()
    {
        //Set to fading, to stop double death situations
        Fading = true;

        //Set the object to be destroyed
        GameDirector.LevelManager.CurrentLevel.ObjectDestroyed(this);

        //Stores the initial alpha of the object to lerp from
        float startignAlpha = GetComponent<MeshRenderer>().material.color.a;
        //Start a timer
        float timeTracker = FadeTime;

        while(timeTracker > 0)
        {
            Debug.Log("Fading: " + timeTracker / FadeTime);
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

        Debug.Log("faded");

        Destroy(this.gameObject);
        //Destroy the object and let the game director know


    }
}
