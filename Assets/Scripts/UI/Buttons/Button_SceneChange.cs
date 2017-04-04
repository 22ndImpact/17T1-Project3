using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_SceneChange : MonoBehaviour
{
    //The scene that will be loaded after hitting this button
    public GD_SceneManager.SceneList SceneTarget;

    public void ChangeScene()
    {
        GameDirector.SceneManager.ChangeScene(SceneTarget);
    }
}
