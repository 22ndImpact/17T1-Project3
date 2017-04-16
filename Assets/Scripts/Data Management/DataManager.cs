using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[Serializable]
public class DataManager
{
    /// <summary>
    /// Returns if a save file at the designated path exists
    /// </summary>
    public bool SaveDataFound
    {
        get
        {
            return File.Exists(Application.persistentDataPath + "//LevelData.xml");
        }
    }

    /// <summary>
    /// Saves level data to an external file
    /// </summary>
    /// <param name="_LevelData"></param>
    public void SaveLevelData(List<LevelData> _LevelData)
    {
        //Create the serializer
        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));
        //Create the stream
        FileStream stream = new FileStream(Application.persistentDataPath + "//LevelData.xml", FileMode.Create);
        //Saves the data
        serializer.Serialize(stream, _LevelData);
        //Closes the stream
        stream.Close();
    }

    /// <summary>
    /// Loads level data from an external file
    /// </summary>
    /// <returns></returns>
    public List<LevelData> LoadLevelData()
    {
        //Create the serializer
        XmlSerializer serializer = new XmlSerializer(typeof(List<LevelData>));
        //Create the stream
        FileStream stream = new FileStream(Application.persistentDataPath + "//LevelData.xml", FileMode.Open);
        //Loads the data and applies it to the given leveldata
        List<LevelData> LevelData = serializer.Deserialize(stream) as List<LevelData>;
        //Cloase the stream
        stream.Close();
        //Returns the new level data
        return LevelData;
    }
}
