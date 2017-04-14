using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectFinder
{
    public static Transform Anchor
    {
        get
        {
            return GameObject.FindGameObjectWithTag("Anchor").transform;
        }
    }

    public static LevelUIController levelUIController
    {
        get
        {
            return GameObject.FindGameObjectWithTag("LevelUI").GetComponent<LevelUIController>();
        }
    }
}
