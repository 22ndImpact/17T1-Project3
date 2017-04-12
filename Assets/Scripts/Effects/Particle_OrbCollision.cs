using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_OrbCollision : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        GameObject.Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
