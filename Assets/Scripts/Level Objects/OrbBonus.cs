using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbBonus : MonoBehaviour
{
    #region Tweaking Variables
    public float FadeTime;
    public float yPositionBoundary;
    #endregion

    #region Tracking Variables
    public bool Active = true;
    #endregion

    #region Component References
    #endregion

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider _other)
    {
        if(_other.gameObject.tag == "Player" && Active)
        {
            GameDirector.LevelManager.CurrentLevel.orbsUsed = Mathf.Clamp(GameDirector.LevelManager.CurrentLevel.orbsUsed - 1, 0, 99);
            Active = false;
            StartCoroutine(FadeOutDestroy());
        }
    }

    IEnumerator FadeOutDestroy()
    {
        //Activate sound
        GameDirector.audioController.PlayEffectClip(GameDirector.audioController.AudioCollection.DestructibleObjectDeath);

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
        Destroy(this.gameObject);
    }
}
