using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle_OrbCollision : MonoBehaviour
{
	void Start ()
    {
        //Just destorys the particle system after its duration has expired
        GameObject.Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);	
	}
}
