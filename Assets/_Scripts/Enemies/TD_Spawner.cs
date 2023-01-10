using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Spawner : Spawner
{
    public List<SpawnWave> Waves;
    private int _nextWaveIndex = 0;
    private int _nextEnemyIndex = 0;
    private List<WaveEnemyData> _enemiesToSpawn;
    [SerializeField]
    private Transform SpawnPosition;
    private float spawnDensity = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemiesToSpawn == null) _enemiesToSpawn = new();
        if (!SpawnPosition) SpawnPosition = transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (_enemiesToSpawn.Count == 0) GetWave();
        if(spawnedEntities.Count < _enemiesToSpawn.Count)
        {
            Spawn();
        }
        //if (_enemiesLeft.Count > 0)
        //{
        //    Debug.Log("Spawn Enemies: " + _enemiesToSpawn);
        //}
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
            lastSpawned.GetComponent<TD_Enemy>().SetStats(waveEnemyData);
            lastSpawned.transform.SetParent(transform);
            spawnedEntities.Add(lastSpawned);
            _nextEnemyIndex++;
            lastSpawnTime = 0;
        }
    }

    private void GetWave()
    {
        _enemiesToSpawn = Waves[_nextWaveIndex]?.waveContents;
        if (_enemiesToSpawn.Count < 1)
            return;
        _nextWaveIndex++;
        spawnedEntities.Clear();
        // TODO:
        // Send event for wave start
    }

    protected override Vector3 AdjustedSpawnPosition()
    {
        // TODO: Add offset
        // TODO: Check that the space is available
        Debug.Log("Adjusted Spawn Position:" + SpawnPosition.position);
        return SpawnPosition.position;
    }
}
