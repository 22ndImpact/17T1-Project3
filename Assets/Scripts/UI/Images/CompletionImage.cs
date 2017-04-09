using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionImage : MonoBehaviour
{
    public Color ColourFail;
    public Color ColourPass;
    public Color ColourPerfect;

    public Image image;

    void Start()
    {
        //Sets the colour of the background image based on current score
        if(GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.perfectScore)
        {
            image.color = ColourPerfect;
        }
        else if(GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.passScore)
        {
            image.color = ColourPass;
        }
        else
        {
            image.color = ColourFail;
        }
    }
}
