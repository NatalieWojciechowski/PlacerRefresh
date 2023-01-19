using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_WaveManager : MonoBehaviour
{
    public static TD_WaveManager current;

    [SerializeField]
    List<WaypointRoute> routes;
    [SerializeField]
    List<TD_Spawner> spawners;

    [SerializeField]
    List<TD_EnemyManager> eManagers;

    // Start is called before the first frame update
    void Start()
    {

        if (routes == null) routes = new();
        if (spawners == null) spawners = new();
        if (eManagers == null) eManagers = new();
    }

    // Update is called once per frame
    void Update()
    {
        // Go through each eManager and check if it is complete


    }

    /// <summary>
    /// This will return if ALL the enemy managers report their wave being complete.
    /// </summary>
    /// <returns></returns>
    bool IsWaveComplete()
    {
        bool allComplete = false;
        eManagers.TrueForAll((eManager) =>
        {
            return false;
            //return eManager.
        });
        //}(TD_EnemyManager eMananger in eManagers)
        //{

        //}
        return false;
    }
}
