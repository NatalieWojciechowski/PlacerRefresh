using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TDEnemyManager : MonoBehaviour
{
    List<TD_Enemy> _enemies;
    List<TD_Spawner> _spawners;
    WaypointRoute _waypointRoute;

    public WaypointRoute WaypointRoute { get => _waypointRoute; set => _waypointRoute = value; }

    public TDEnemyManager(WaypointRoute waypointRoute)
    {
        _waypointRoute = waypointRoute;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_spawners == null) _spawners = new();
        if (_enemies == null) _enemies = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_waypointRoute) return;
        if (_spawners.Count > 0) CheckSpawners();
        if (_enemies.Count > 0) CheckEnemyState();
    }

    private void CheckSpawners()
    {
        foreach (Spawner spawner in _spawners)
        {
            Debug.Log(spawner);
        }

    }

    //public static TDEnemyManager FindOrNewForCircuit(WaypointCircuit waypointCircuit)
    //{
    //    TDEnemyManager.all
    //}

    private void CheckEnemyState()
    {
        // Enable/ start animations / give buffs
    }

//    public TD_Enemy SpawnEnemy(Vector3 spawnPosition)
//    {
////        _Enemies.Add(Instantiate())

//    }
}
