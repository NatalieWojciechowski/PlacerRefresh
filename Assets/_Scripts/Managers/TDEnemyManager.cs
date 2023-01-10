using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TDEnemyManager : MonoBehaviour
{
    [SerializeField]
    WaypointRoute _waypointRoute;
    List<TD_Enemy> _enemies;
    [SerializeField]
    List<TD_Spawner> _spawners;
    float timeRemaining = -10f;
    public float WaveIntervalDelay = 2f;
    private int _requestedWaveIndx;
    private bool _waveActive = false;

    public WaypointRoute WaypointRoute { get => _waypointRoute; set => _waypointRoute = value; }

    public TDEnemyManager(WaypointRoute waypointRoute)
    {
        _waypointRoute = waypointRoute;
    }

    private void Awake()
    {
        EventManager.OnWaveFinish += (waveIndx) => StartWaveInterval(waveIndx);
        EventManager.OnWaveStart += (waveIndx) => enableWave();
    }

    private void OnDisable()
    {
        EventManager.OnWaveFinish -= (waveIndx) => StartWaveInterval(waveIndx);
        EventManager.OnWaveStart -= (waveIndx) => enableWave();
    }

    private void enableWave()
    {
        _waveActive = true;
    }

    private void StartWaveInterval(int waveIndx)
    {
        _requestedWaveIndx = waveIndx;
        // TODO: Scale based on how far into the waves?
        timeRemaining = WaveIntervalDelay;
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
        timeRemaining -= Time.deltaTime;
        //if (timeRemaining <= 0) CheckStartSpawers();
        if (_waveActive)
        {
            if (_spawners.Count > 0) CheckSpawners();
            if (_enemies.Count > 0) CheckEnemyState();
        }
    }

    //private void CheckStartSpawers()
    //{
    //    if _requestedWaveIndx;
    //    throw new NotImplementedException();
    //}

    private void CheckSpawners()
    {
        foreach (TD_Spawner spawner in _spawners)
        {
            Debug.Log(spawner);
            spawner.SpawnAllowed = true;
            //if (spawner.Waves.Count > _requestedWaveIndx) spawner.SpawnNextWave();
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
