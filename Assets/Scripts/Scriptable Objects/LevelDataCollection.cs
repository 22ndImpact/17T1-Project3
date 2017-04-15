using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelDataCollection : ScriptableObject
{
    [SerializeField]
    public List<LevelData> LevelList;
}

[System.Serializable]
public class LevelData
{
    public string LevelName;
    public int LevelID;
    public int PassScore;
    public int PerfectScore;
    public int BestScore;
    public bool Unlocked;
}
