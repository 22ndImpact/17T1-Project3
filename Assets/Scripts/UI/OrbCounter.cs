using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbCounter : MonoBehaviour
{
    public GameObject NotchPrefab;

    public GameObject PerfectBar;
    public GameObject PassBar;
    public GameObject CompletionBar;
    public RectTransform Notches;

    public List<GameObject> NotchList;


	// Use this for initialization
	void Start ()
    {
        //Set the completion bar to 0 scale
        CompletionBar.transform.localScale = new Vector3(0,1,1);

        //Set the PassBar to the right scale based on pass score
        //Remainder of perfect score: 1 - PerfectScore/PassScore
        PassBar.transform.localScale = new Vector3(1 - (float)GameDirector.LevelManager.CurrentLevel.perfectScore / (float)GameDirector.LevelManager.CurrentLevel.passScore, 1, 1);

        //Create a bunch of notches to break up the bar based on pass score
        for (int i = 0; i < GameDirector.LevelManager.CurrentLevel.passScore + 1; i++)
        {
            GameObject newNotch = Instantiate(NotchPrefab);
            newNotch.GetComponent<RectTransform>().SetParent(Notches, false);

            //Adjust spacing based on total number of notches
            
            //float XPosition =  PerfectBar.GetComponent<RectTransform>().w (float)GameDirector.LevelManager.CurrentLevel.perfectScore / (float)GameDirector.LevelManager.CurrentLevel.passScore

            newNotch.transform.localPosition = new Vector3(0,0,0);


            NotchList.Add(newNotch);
        }

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
