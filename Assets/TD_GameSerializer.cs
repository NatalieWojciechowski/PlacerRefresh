using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

[Serializable]
public class SaveData
{
    [Serializable] public struct EnemySaveData
    {
        public Guid Guid;
        public float Health;
        public Transform Transform;
        public Transform NextWaypoint;
    }

    [Serializable] public struct TowerSaveData
    {
        public Guid Guid;
        public TD_BuildingData TD_BuildingData;
        public Transform Transform;
    }

    // Game State
    public int currentWaveIndex;
    public int playerMoney;
    public List<EnemySaveData> currentEnemies;

    // Core State 
    public int coreHealth;

    // Tower State
    public List<TowerSaveData> constructedBuildings;

    public int savedInt;
    public float savedFloat;
    public bool savedBool;

    public SaveData()
    {
        currentEnemies = new();
        constructedBuildings = new();
        //constructedBuildings = new();
    }
}

public class TD_GameSerializer : MonoBehaviour
{
    public static TD_GameSerializer instance { get; private set; }
    static string saveFileName = "td_game_save.dat";
    private string _fullSavePath;

    private void OnEnable()
    {
        if (instance != null) Destroy(this);
        instance = this;
        _fullSavePath = $"{Application.persistentDataPath}/{saveFileName}";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool SaveDataExists()
    {
        Debug.Log(Application.persistentDataPath + $"/{saveFileName}");
        return File.Exists(Application.persistentDataPath + $"/{saveFileName}");
    }

    public static void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var file = File.Open($"{Application.persistentDataPath}/{saveFileName}", FileMode.OpenOrCreate))
        {
            //bf.Serialize(filePin, data);
            //FileStream file = File.Create(Application.persistentDataPath
            //             + $"/{instance.saveFileName}");
            SaveData data = new SaveData();
            TD_GameManager.current.AddToSaveData(ref data);
            bf.Serialize(file, data);
        }
        Debug.Log("Game data saved!");
    }

    public static bool LoadGame()
    {
        if (File.Exists(Application.persistentDataPath
                       + $"/{saveFileName}"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var file = File.Open($"{Application.persistentDataPath}/{saveFileName}", FileMode.OpenOrCreate))
            {
                SaveData data = (SaveData)bf.Deserialize(file);
                TD_GameManager.current.InitFromData(data);
                TD_BuildManager.current.InitFromData(data);
                Debug.Log("Game data loaded!");
                return true;
            }
        }
        else
            Debug.LogError("There is no save data!");
        return false;
    }

    public static void ResetData()
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
