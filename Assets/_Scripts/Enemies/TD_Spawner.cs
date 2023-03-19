using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Spawner : MonoBehaviour
{
    public WaypointRoute ForRoute;
    public List<WaveDetails> Waves;
    public bool SpawnAllowed = false;
    //private int currentWaveIndex = 0;
    [SerializeField] private List<TD_EnemyData> _enemiesToSpawn;
    [SerializeField] private List<TD_Enemy> _enemiesAlive;
    private int currentEnemyIndex = 0;
    //private List<TD_Wave> _waves;
    [SerializeField] private List<GameObject> spawnedEntities;
    private List<TD_Wave> waveHelpers;
    public bool CurrentWaveComplete = false;

    public GameObject ActiveSpawnerEffects;

    [SerializeField]
    private Transform SpawnPosition;
    /// <summary>
    ///  How far apart the entities should be before spawning next
    /// </summary>
    private float spawnDensity = 0.5f;
    private float lastSpawnTime;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        if (ForRoute) ForRoute.TdSpawner = this;
        EventManager.OnWaveStart += ToggleSpawnerEffects;
    }

    private void OnDisable()
    {
        EventManager.OnWaveStart -= ToggleSpawnerEffects;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (waveHelpers == null) waveHelpers = new();
        //if (_waves == null) _waves = new();
        if (spawnedEntities == null) spawnedEntities = new();
        if (_enemiesToSpawn == null) _enemiesToSpawn = new();
        if (_enemiesAlive == null) _enemiesAlive = new();
        if (Waves == null) Waves = new();
        if (!SpawnPosition) SpawnPosition = transform;
    }

    // Update is called once per frame
    protected void Update()
    {
        // Game State Checks
        if (!TD_GameManager.instance) return;
        if (waveHelpers.Count == 0 && !SetupWaveHelpers()) return;
        if (TD_GameManager.instance.CurrentWaveIndex >= waveHelpers.Count) return;

        // Wave State Checks
        TD_Wave currentWave = GetCurrentWave();
        if (currentWave == null) return;
        if (SpawnAllowed && IsDelayTimerMet() && SpawnPlacementValid()) SpawnEnemy(currentWave.GetEnemy(currentEnemyIndex));
        if (spawnedEntities.Count > 0) CheckWaveCompleted(currentWave);
    }

    private void CheckWaveCompleted(TD_Wave currentWave)
    {
        _enemiesAlive.RemoveAll((enemy) => enemy == null);
        if (_enemiesAlive.Count <= 0)
        {
            WaveCleanup(currentWave);
        }
    }

    public void OnAllWaveSpawned(TD_Wave currentWave)
    {
        // TODO: This toggle may need a split for indx vs bool passing
        ToggleSpawnerEffects(TD_GameManager.instance.CurrentWaveIndex);
        SpawnAllowed = false;
        ActiveSpawnerEffects.SetActive(false);

        //currentWave.WaveSpawningComplete();
        // Check for Enemies all defeatd / reached end before setting to next wave
        _enemiesAlive.RemoveAll((enemy) => enemy == null);

    }


    #region private

    private void ToggleSpawnerEffects(int waveIndx)
    {
        Debug.Log("ToggleSpawnerEffects" + waveIndx.ToString());
        if (!ActiveSpawnerEffects) return;
        bool toggleToState = false;
        if (waveHelpers.Count > waveIndx && waveHelpers[waveIndx] != null)
        {
            TD_Wave targetWave = waveHelpers[waveIndx];
            if (!targetWave.AllSpawned) toggleToState = true;
            // If current wave done spawning or if spawner has empty wave this round
            else if (targetWave.WaveDetails?.waveContents?.Count < 0 || (targetWave.AllSpawned)) toggleToState = false;
            else Debug.Log("TOGGLE SPAWNER EFFECTS IN STRANGE STATE: INDEX" + waveIndx);
        }
        else ActiveSpawnerEffects.SetActive(false);

        ActiveSpawnerEffects.SetActive(toggleToState);
    }

    public void DefeatCurrentWave()
    {
        _enemiesToSpawn.Clear();
        foreach (TD_Enemy enemy in _enemiesAlive)
        {
            enemy.TakeDamage(10000f);
        }
        WaveCleanup(GetCurrentWave());
    }

    private void WaveCleanup(TD_Wave currentWave)
    {
        currentEnemyIndex = 0;
        CurrentWaveComplete = true;
        spawnedEntities.RemoveAll((entity) => (entity == null));
        currentWave.EndWave();
    }

    private TD_Wave GetCurrentWave()
    {
        TD_Wave currentWave = null;
        if (waveHelpers != null && TD_GameManager.instance) currentWave = waveHelpers[TD_GameManager.instance.CurrentWaveIndex];
        return currentWave;
    }

    private bool SetupWaveHelpers()
    {
        int _waveIndex = 0;
        foreach (WaveDetails waveDetail in Waves)
        {
            waveHelpers.Add(new TD_Wave(_waveIndex, this, waveDetail));
            _waveIndex++;
        }
        return _waveIndex != 0;
    }

    protected bool SpawnPlacementValid()
    {
        bool occupied = false;
        spawnedEntities.ForEach(entity => {
            if (entity)
                occupied |= Vector3.Distance(entity.transform.position, AdjustedSpawnPosition()) < spawnDensity;
        });
        return !occupied;
    }

    /// <summary>
    /// Called once we have a wave, a valid position, and retrieved the next enemy data
    /// </summary>
    /// <param name="waveEnemyData"></param>
    protected void SpawnEnemy(TD_EnemyData waveEnemyData)
    {
        if (waveEnemyData && waveEnemyData.spawnPrefab)
        {
            GameObject lastSpawned = Instantiate<GameObject>(waveEnemyData.spawnPrefab, AdjustedSpawnPosition(), Quaternion.identity);
            lastSpawned.transform.SetParent(transform);

            TD_Enemy enemyControl = lastSpawned.GetComponent<TD_Enemy>();
            enemyControl.SetStats(waveEnemyData, ForRoute);
            enemyControl.SetPreviousWaypoint(SpawnPosition);

            spawnedEntities.Add(lastSpawned);
            _enemiesAlive.Add(enemyControl);
            currentEnemyIndex++;
            lastSpawnTime = Time.time;
        }
    }

    protected Vector3 AdjustedSpawnPosition()
    {
        // TODO: Add offset
        // TODO: Check that the space is available
        //Debug.Log("Adjusted Spawn Position:" + SpawnPosition.position);
        return SpawnPosition.position;
    }
    private bool IsDelayTimerMet()
    {
        // TODO: store this and update on change ; dont keep checking!
        return (Time.time - lastSpawnTime > Waves[TD_GameManager.instance.CurrentWaveIndex].spawnInterval);
    }
    #endregion
}
