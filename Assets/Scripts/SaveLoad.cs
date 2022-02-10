using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad 
{
    public static void SaveData(Level _level)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Levels/Level_" + _level.id.ToString() + ".csc";

        FileStream stream = new FileStream(path, FileMode.Create);

        LevelData charData = new LevelData(_level);

        formatter.Serialize(stream, charData);
        stream.Close();
    }

    public static LevelData LoadData(int _id)
    {
        string path = Application.persistentDataPath + "/Levels/Level_" + _id.ToString();

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            LevelData data = formatter.Deserialize(stream) as LevelData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Error: Save file not found in " + path);
            return null;
        }
    }
}
