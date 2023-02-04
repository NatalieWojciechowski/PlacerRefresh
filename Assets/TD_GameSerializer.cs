using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
public class SaveData
{
    // Game State
    public int currentWaveIndex;
    public int playerMoney;

    // Core State 
    public int coreHealth;

    // Tower State


    public int savedInt;
    public float savedFloat;
    public bool savedBool;

}

public class TD_GameSerializer : MonoBehaviour
{
    public static TD_GameSerializer instance;
    string saveFileName = "td_game_save.dat";

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null) Destroy(this);
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SaveDataExists()
    {
        return File.Exists(Application.persistentDataPath + $"/{saveFileName}");
    }

    public static void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
                     + $"/{instance.saveFileName}");
        SaveData data = new SaveData();
        data.playerMoney = TD_GameManager.current.CurrentCurrency;
        data.currentWaveIndex = TD_GameManager.current.CurrentWaveIndex;
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    public static void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                       + $"/{instance.saveFileName}"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + "/td_game_save.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);

            TD_GameManager.current.InitFromData(data);

            file.Close();
            data.playerMoney = TD_GameManager.current.CurrentCurrency;
            data.currentWaveIndex = TD_GameManager.current.CurrentWaveIndex;
            Debug.Log("Game data loaded!");
        }
        else
            Debug.LogError("There is no save data!");
    }

    void ResetData()
    {
        if (File.Exists(Application.persistentDataPath
                      + $"/{saveFileName}"))
        {
            File.Delete(Application.persistentDataPath
                              + $"/{saveFileName}");
            Debug.Log("Data reset complete!");
        }
        else
            Debug.LogError("No save data to delete.");
    }
}
