using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class DataManager
{
    public bool SaveDataFound
    {
        get
        {
            return File.Exists(Application.dataPath + "//LevelData.xml");
        }
    }

    public void SaveLevelData(List<LevelData> _LevelData)
    {
        //Create the serializer
        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));
        //Create the stream
        FileStream stream = new FileStream(Application.dataPath + "//LevelData.xml", FileMode.Create);
        //Saves the data
        serializer.Serialize(stream, _LevelData);
        //Closes the stream
        stream.Close();

        Debug.Log("Saved level data to: " + Application.dataPath);
    }

    public List<LevelData> LoadLevelData()
    {
        //Create the serializer
        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));
        //Create the stream
        FileStream stream = new FileStream(Application.dataPath + "//LevelData.xml", FileMode.Open);
        //Loads the data and applies it to the given leveldata
        List<LevelData> LevelData = serializer.Deserialize(stream) as List<LevelData>;
        //Cloase the stream
        stream.Close();
        //Returns the new level data
        return LevelData;

        Debug.Log("loaded stuff");
    }
}
