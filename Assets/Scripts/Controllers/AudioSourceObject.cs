using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceObject : MonoBehaviour
{
    //Sets the audiosource of the object to the game director to be access by everythign else
    void Awake()
    {
        //GameDirector.audioController = GetComponent<AudioSource>();
    }
}
