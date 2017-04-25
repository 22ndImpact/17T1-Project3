using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBlock : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnCollisionEnter(Collision _collision)
    {
        if(_collision.contacts[0].otherCollider.gameObject.tag == "Player")
        {
            //Destroy the orb
            _collision.contacts[0].otherCollider.gameObject.GetComponent<PlayerOrb>().StartCoroutine(_collision.contacts[0].otherCollider.gameObject.GetComponent<PlayerOrb>().FadeOutDestroy());
        }
    }
}
