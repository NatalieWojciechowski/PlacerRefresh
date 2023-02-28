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
        public Vector3 position;
        public Vector3 NextWaypoint;
    }

    [Serializable] public struct TowerSaveData
    {
        public Guid Guid;
        public TD_BuildingData TD_BuildingData;
        public Vector3 position;
        public bool isRunning;
        public int currentTier;
        public TD_Building.BuildingState buildingState;
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
    static string saveFileName = "td_game_save.json";
    private string _fullSavePath;

    private void OnEnable()
    {
        _fullSavePath = $"{Application.persistentDataPath}/{saveFileName}";
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else Destroy(this);
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

    public static string PersistentDataPath()
    {
        return $"{Application.persistentDataPath}/{saveFileName}";
    }

    public static void SaveGame()
    {
        var file = File.Open(PersistentDataPath(), FileMode.OpenOrCreate);
        if (file.CanWrite) {
            file.Close();
            SaveData data = new SaveData();

            TD_GameManager.instance.AddToSaveData(ref data);
            TD_BuildManager.instance.AddToSaveData(ref data);
            string jsonData = JsonUtility.ToJson(data, true);

            File.WriteAllTextAsync(PersistentDataPath(), jsonData);
        }
        Debug.Log("Game data saved!");
    }

    public static bool LoadGame()
    {
        if (File.Exists(PersistentDataPath()))
        {
            string dataText = File.ReadAllText(PersistentDataPath());
            if (dataText.Length > 1)
            {
                SaveData data = JsonUtility.FromJson<SaveData>(dataText);

                if (TD_GameManager.instance) TD_GameManager.instance.InitFromData(data);
                if (TD_BuildManager.instance) TD_BuildManager.instance.InitFromData(data);
                Debug.Log("Game data loaded!");

                return true;
            }
        }
        else
            TD_GameManager.instance.useSaveData = false;
            Debug.Log("There is no save data!");
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
        TD_GameManager.instance.useSaveData = false;
    }
}
