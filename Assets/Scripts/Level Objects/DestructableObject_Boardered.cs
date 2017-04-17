using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject_Boardered : MonoBehaviour
{
    #region Tweaking Variables
    public float FadeTime;
    #endregion

    #region Tracking Variables
    bool Fading = false;
    #endregion

    #region Object References
    public DestructibleObject innerObject;
    #endregion

    #region Component References
    public AudioClip DeathNoise;
    #endregion

    void Start()
    {
        DisableInnerObject();
    }

    void DisableInnerObject()
    {
        innerObject.RB.useGravity = false;
        innerObject.GetComponent<Collider>().enabled = false;
    }

    void EnableInnerObject()
    {
        innerObject.RB.useGravity = true;
        innerObject.GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision _collision)
    {
        //If it collides with the orb
        if (_collision.gameObject.tag == "Player")
        {
            //Trigger the object fade
            if (!Fading)
            {
                //Activte the fade
                StartCoroutine(FadeOutDestroy());
            }
        }
    }

    IEnumerator FadeOutDestroy()
    {
        //Activate sound TODO replace with own sound
        GameDirector.LevelManager.CurrentLevel.PlaySound(innerObject.DeathNoise);

        //Set to fading, to stop double death situations
        Fading = true;

        //Stores the initial alpha of the object to lerp from
        float startignAlpha = GetComponent<MeshRenderer>().material.color.a;
        //Start a timer
        float timeTracker = FadeTime;

        while (timeTracker > 0)
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
        //Turn on the inner object before destroying the boarder
        EnableInnerObject();
        //Destroy the boarder object
        Destroy(this.gameObject);
    }
}
