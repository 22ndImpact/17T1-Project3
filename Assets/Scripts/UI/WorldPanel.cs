using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldPanel : MonoBehaviour
{
    public Text WorldName;
    public LevelSelectButtons levelButtons;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void InitializeWorldPanel(int _WorldNumber, string _WorldName)
    {
        WorldName.text = _WorldName;
        levelButtons.InitializeLevelSelectButtons(_WorldNumber);
    }
}
