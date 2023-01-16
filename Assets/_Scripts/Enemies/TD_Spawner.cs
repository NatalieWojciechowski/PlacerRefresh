using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Spawner : Spawner
{
    public WaypointRoute ForRoute;
    public List<SpawnWave> Waves;
    public bool SpawnAllowed = false;
    private int _nextWaveIndex = 0;
    private int _nextEnemyIndex = 0;
    private List<WaveEnemyData> _enemiesToSpawn;
    [SerializeField]
    private Transform SpawnPosition;
    /// <summary>
    ///  How far apart the entities should be before spawning next
    /// </summary>
    private float spawnDensity = 0.5f;

    private void Awake()
    {
        //EventManager.OnWaveStart += (waveIndx) => SpawnNextWave();
    }

    private void OnDisable()
    {
        EventManager.OnWaveStart -= (waveIndx) => SpawnNextWave();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_enemiesToSpawn == null) _enemiesToSpawn = new();
        if (!SpawnPosition) SpawnPosition = transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (_enemiesToSpawn.Count > 0 && spawnedEntities.Count != _enemiesToSpawn.Count)
        {
            if (spawnedEntities.Count < _enemiesToSpawn.Count) Spawn();
            else EventManager.WaveFinished(_nextWaveIndex - 1);
        }
        else if(SpawnAllowed) SpawnNextWave();
        base.Update();
    }

    protected override void Spawn()
    {
        if (_nextEnemyIndex < _enemiesToSpawn.Count) SpawnEnemy(_enemiesToSpawn[_nextEnemyIndex]);
    }

    protected override bool SpawnPlacementValid()
    {
        bool occupied = false;
        spawnedEntities.ForEach(entity => {
            if (entity)
                occupied |= Vector3.Distance(entity.transform.position, AdjustedSpawnPosition()) < spawnDensity;
        });
        return !occupied;
    }

    protected void SpawnEnemy(WaveEnemyData waveEnemyData)
    {
        if (waveEnemyData && waveEnemyData.spawnPrefab && TimerComplete() && SpawnPlacementValid())
        {
            GameObject lastSpawned = Instantiate<GameObject>(waveEnemyData.spawnPrefab, AdjustedSpawnPosition(), Quaternion.identity);
            TD_Enemy enemyControl = lastSpawned.GetComponent<TD_Enemy>();
            enemyControl.SetStats(waveEnemyData);
            enemyControl.fullRoute = ForRoute;
            enemyControl.SetPreviousWaypoint(SpawnPosition);
            lastSpawned.transform.SetParent(transform);
            spawnedEntities.Add(lastSpawned);
            _nextEnemyIndex++;
            lastSpawnTime = 0;
        }
    }

    public void SpawnNextWave()
    {
        AddWave(GetWaveEnemies(_nextWaveIndex), true);
    }


    /// <summary>
    /// Wrapper for actual access to the wave data
    /// </summary>
    /// <param name="waveIndex"></param>
    /// <returns></returns>
    private List<WaveEnemyData> GetWaveEnemies(int waveIndex)
    {
        if (waveIndex >= Waves.Count) return null;
        return Waves[waveIndex]?.waveContents;
        //if (_waveEnemies.Count < 1)
        //    return null;
        //return _enemiesToSpawn
        // TODO:
        // Send event for wave start
    }

    /// <summary>
    /// Whether a brand new wave or adding (early start) To the existing wave, add to spawn queue.
    /// </summary>
    /// <param name="tD_Enemies"></param>
    /// <param name="clearPrevious">Whether or not to clear out the tracking lists</param>
    private void AddWave(List<WaveEnemyData> tD_Enemies, bool clearPrevious = true)
    {
        if (tD_Enemies == null || tD_Enemies.Count < 1) return;
        if (clearPrevious)
        {
            _nextEnemyIndex = 0;
            _enemiesToSpawn.Clear();
            spawnedEntities.Clear();
        }
        _enemiesToSpawn.AddRange(tD_Enemies);
        EventManager.WaveStarted(_nextWaveIndex);
        _nextWaveIndex++;
    }

    protected override Vector3 AdjustedSpawnPosition()
    {
        // TODO: Add offset
        // TODO: Check that the space is available
        //Debug.Log("Adjusted Spawn Position:" + SpawnPosition.position);
        return SpawnPosition.position;
    }
}
