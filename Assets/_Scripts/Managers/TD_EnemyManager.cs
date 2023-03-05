using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TD_EnemyManager : MonoBehaviour
{
    public static TD_EnemyManager instance { get; private set; }

    [SerializeField] WaypointRoute _waypointRoute;
    [SerializeField] List<WaypointRoute> Routes;
    [SerializeField] List<TD_Spawner> _spawners;

    /// <summary>
    /// The time between individual waves; not between individual spawns
    /// </summary>
    public float WaveIntervalDelay = 2f;
    float waveIntervalRemaining = -10f;
    public bool AutoStart = true;
    
    private bool _waveActive = false;
    public bool WaveActive { get => _waveActive; }

    public int TotalWaves { get => GetTotalWaves(); }


    public WaypointRoute WaypointRoute { get => _waypointRoute; set => _waypointRoute = value; }

    #region Lifecycle
    private void OnEnable()
    {
        if (_spawners == null) _spawners = new();
        waveIntervalRemaining = WaveIntervalDelay;
        RefreshSpawners();
        EventManager.OnWaveStart += enableWave;
    }

    private void OnDisable()
    {
        EventManager.OnWaveStart -= enableWave;
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
        //if (_enemies == null) _enemies = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (!TD_GameManager.instance) return;
        if (_spawners.Count < 1) RefreshSpawners();

        // TODO: Change this to a coroutine
        if (_waveActive)
        {
            if (IsCurrentWaveComplete()) NotifyCurrentWaveComplete();
        } else if (!_waveActive) {
            // If not active we should the countdown for autostart waves
            waveIntervalRemaining -= Time.deltaTime;

            //
            if (TD_GameManager.instance.PlayerReady || ShouldAutoStart())
            {
                TryStartWave();
            }
        }
    }

    #endregion

    #region Events
    private void NotifyCurrentWaveComplete()
    {
        Debug.Log("Wave Complete for all Spawners");
        _waveActive = false;
        RestartWaveInterval();

        EventManager.OnWaveFinish(TD_GameManager.instance.CurrentWaveIndex);
    }

    #endregion

    #region Public
    /// <summary>
    /// Will return the max wave count for all Spawners.
    /// </summary>
    /// <returns></returns>
    public int GetTotalWaves()
    {
        int largestWaveCount = 0;
        foreach (TD_Spawner _spawner in _spawners)
        {
            if (_spawner.Waves.Count > largestWaveCount) largestWaveCount = _spawner.Waves.Count;
        }
        return largestWaveCount;
    }

    /// <summary>
    /// Check if ALL spawners in the manager have completed their current wave
    /// </summary>
    /// <returns></returns>
    public bool IsCurrentWaveComplete()
    {
        bool anyInProgress = false;
        foreach (TD_Spawner spawner in _spawners)
        {
            if (!spawner.CurrentWaveComplete) anyInProgress = true;
        }
        return !anyInProgress;
    }
    #endregion

    #region Private

    private void enableWave(int waveIndex)
    {
        TryStartWave();
    }

    /// <summary>
    /// Start timer for next autostart
    /// </summary>
    private void RestartWaveInterval()
    {
        waveIntervalRemaining = WaveIntervalDelay;
    }

    private void TryStartWave()
    {
        RefreshSpawners();
        if (TD_GameManager.instance.CurrentWaveIndex < TotalWaves)
        {
            ToggleSpawners(true);
            _waveActive = true;
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

    /// <summary>
    /// Either autostart not enabled or timer expired
    /// </summary>
    /// <returns></returns>
    private bool ShouldAutoStart()
    {
        return AutoStart && IsCurrentWaveComplete() && waveIntervalRemaining <= 0;
    }
    #endregion
}
