using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TD_EnemyManager : MonoBehaviour
{
    public static TD_EnemyManager current { get; private set; }

    [SerializeField]
    WaypointRoute _waypointRoute;
    [SerializeField]
    List<WaypointRoute> Routes;
    List<TD_Enemy> _enemies;
    [SerializeField]
    List<TD_Spawner> _spawners;
    float timeRemaining = -10f;
    public float WaveIntervalDelay = 2f;
    private int _requestedWaveIndx;
    private bool _waveActive = false;
    public bool WaveActive { get => _waveActive; }


    public int CurrentWave = 0;
    private int _totalWaves;
    public int TotalWaves { get => GetTotalWaves(); }


    public WaypointRoute WaypointRoute { get => _waypointRoute; set => _waypointRoute = value; }

    private void OnEnable()
    {
        if (current != null) Destroy(this);
        current = this;
        if (_spawners == null) _spawners = new();

        timeRemaining = WaveIntervalDelay;
        RefreshSpawners();
            //else _spawners.Clear();
        EventManager.OnWaveStart += enableWave;
    }

    private void OnDisable()
    {
        EventManager.OnWaveStart -= enableWave;
    }

    private void enableWave(int waveIndex)
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
        DontDestroyOnLoad(gameObject);
        //if (_enemies == null) _enemies = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (Routes == null || Routes.Count < 1 || !TD_GameManager.current) return;
        if (_spawners.Count < 1) RefreshSpawners();
        //!TD_GameManager.current.HasStarted) return;
        if (TD_GameManager.current.HasStarted)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0) TryStartSpawers();
        }
        if (_waveActive)
        {
            CheckEnemyState();
            if (IsCurrentWaveComplete()) OnCurrentWaveComplete();
        }
    }

    private void OnCurrentWaveComplete()
    {
        Debug.Log("Wave Complete for all Spawners");
        _waveActive = false;
        StartWaveInterval(TD_GameManager.current.CurrentWaveIndex);
        EventManager.OnWaveFinish(TD_GameManager.current.CurrentWaveIndex);
    }

    private void TryStartSpawers()
    {
        RefreshSpawners();
        if (TD_GameManager.current.CurrentWaveIndex < TotalWaves)
        {
            ToggleSpawners(true);
            _waveActive = true;
            //Debug.Log("This may have been start spawn initially");
            //EventManager.current.WaveFinished(TD_GameManager.current.CurrentWaveIndex);
        }
    }

    private void RefreshSpawners()
    {
        if (_spawners == null) _spawners = new();
        else _spawners.Clear();
        foreach (WaypointRoute route in Routes)
        {
            if (route.TdSpawner != null) _spawners.Add(route.TdSpawner);
        }
    }

    private void ToggleSpawners(bool isEnabled)
    {
        foreach (TD_Spawner spawner in _spawners)
        {
            spawner.SpawnAllowed = isEnabled;
        }
    }

    private void CheckEnemyState()
    {

    }

    /// <summary>
    /// Will return the max wave count for all Spawners.
    /// </summary>
    /// <returns></returns>
    public int GetTotalWaves()
    {
        int largestWaveCount = 0;
        foreach(TD_Spawner _spawner in _spawners)
        {
            if (_spawner.Waves.Count > largestWaveCount) largestWaveCount = _spawner.Waves.Count;
        }
        return largestWaveCount;
    }

    public bool IsCurrentWaveComplete()
    {
        bool anyInProgress = false;
        foreach (TD_Spawner spawner in _spawners)
        {
            if (!spawner.CurrentWaveComplete) anyInProgress = true;
        }
        return !anyInProgress;
    }
}
