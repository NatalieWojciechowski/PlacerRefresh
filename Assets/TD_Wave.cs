using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Wave
{
    float startTime;
    float completeTime;
    private int waveIndex;
    private TD_Spawner spawner;
    private WaveDetails waveDetails;
    private List<TD_EnemyData> enemiesToSpawn;
    private int enemiesSpawnedCount = 0;

    public bool AllSpawned { get => finished || checkAllSpawned(); }
    private bool finished = false;
    //public float waveCompletedDuration;

    public WaveDetails WaveDetails;

    public TD_Wave(int _waveIndex, TD_Spawner _spawner, WaveDetails _waveDetails) {
        // TODO: do these need to be unique across spawners? 
        waveIndex = _waveIndex;
        spawner = _spawner;
        waveDetails = _waveDetails;
        enemiesToSpawn = new();
        InitWave(waveDetails.waveContents);
    }

    public void StartWave()
    {
        startTime = Time.time;
        // TODO: Invoke Event?
    }

    public void EndWave()
    {
        finished = true;
        completeTime = Time.time;
        // TODO: Invoke event?
        EventManager.WaveFinished(waveIndex);
    }

    public TD_EnemyData GetEnemy(int enemyIndex)
    {
        enemiesSpawnedCount++;
        // TODO: better handling around out of range exception here 
        return enemiesToSpawn[enemyIndex];
    }

    private void InitWave(List<TD_EnemyData> enemies)
    {
        enemiesToSpawn.AddRange(enemies);
        startTime = Time.time;
    }
    private bool checkAllSpawned()
    {
        return (enemiesSpawnedCount > 0 && enemiesSpawnedCount == enemiesToSpawn.Count);
    }
}
