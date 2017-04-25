using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingAnchor : MonoBehaviour
{

    Anchor levelAnchor;
    LineRenderer tether;

	// Use this for initialization
	void Start ()
    {
        levelAnchor = GameObject.Find("Anchor").GetComponent<Anchor>();
        tether = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if (levelAnchor.CurrentOrb != null)
        {
            tether.SetPosition(0, transform.position);
            tether.SetPosition(1, levelAnchor.CurrentOrb.transform.position);
        }
        else
        {
            tether.SetPosition(0, transform.position);
            tether.SetPosition(1, levelAnchor.transform.position);
        }

	}
}
