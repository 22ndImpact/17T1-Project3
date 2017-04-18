using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource Effect_AudioSource;
    public AudioSource Effect_PitchLoop_Source;
    public AudioSource Music_AudioSource;
    public AudioAssetCollection AudioCollection;

    #region Tweaking Variables
    public float PitchIncrement;
    #endregion

    #region Tracking Variables
    public float PitchBase;
    public float PitchMin;
    public float PitchMax;
    #endregion

    void Awake()
    {
        //Connecting the audio controller to the game director
        GameDirector.audioController = this;
    }

    void Start()
    {
        //Setting the base pitch
        PitchBase = Effect_PitchLoop_Source.pitch;
    }

    public void PlayEffectClip(AudioClip _AudioClip)
    {
        Effect_AudioSource.PlayOneShot(_AudioClip);
    }

    public void PlayEffectPitchLoop(AudioClip _AudioClip)
    {
        //Adjust the pitch
        Effect_PitchLoop_Source.pitch = Mathf.Clamp(Effect_PitchLoop_Source.pitch + PitchIncrement, PitchMin, PitchMax);
        
        //Play clip
        Effect_PitchLoop_Source.PlayOneShot(_AudioClip);
    }

    public void ResetPitchLoop()
    {
        Effect_PitchLoop_Source.pitch = PitchBase;
    }
}
