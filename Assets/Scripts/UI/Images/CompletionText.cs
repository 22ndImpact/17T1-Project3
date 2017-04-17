using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompletionText : MonoBehaviour
{
    #region Tweaking Variables
    public string TextFail;
    public string TextPass;
    public string TextPerfect;

    public Color ColourFail;
    public Color ColourPass;
    public Color ColourPerfect;
    #endregion

    #region Component References
    public Text text;
    #endregion

    /// <summary>
    /// Sets the values of the text based on level data
    /// </summary>
    public void UpdateState()
    {
        //Sets the colour of the background image based on current score
        if (GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.perfectScore)
        {
            text.text = TextPerfect;
            text.color = ColourPerfect;
        }
        else if (GameDirector.LevelManager.CurrentLevel.orbsUsed <= GameDirector.LevelManager.CurrentLevel.passScore)
        {
            text.text = TextPass;
            text.color = ColourPass;
        }
        else
        {
            text.text = TextFail;
            text.color = ColourFail;
        }
    }
}
