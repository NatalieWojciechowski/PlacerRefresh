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
    private List<string> enemiesAlive;
    private int enemiesSpawnedCount = 0;

    public bool AllSpawned { get => allSpawned || checkAllSpawned(); }
    private bool finished = false;
    private bool allSpawned = false;

    private bool defeated = false;
    public bool Defeated { get => defeated; }

    private bool checkAllDefeated()
    {
        //TD_EnemyData enemiesRemaining = enemiesToSpawn.Find((enemy) => { return enemy != null; });
        return allSpawned; // && enemiesRemaining;
    }

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
        EventManager.current.WaveStarted(waveIndex);
    }

    public void WaveSpawningComplete()
    {
        //Debug.Log("WAVE SPAWNING COMPLETE");
        allSpawned = true;
        //completeTime = Time.time;
    }

    public void EndWave()
    {
        if (finished == true) return;
        Debug.Log("END WAVE");
        finished = true;
        defeated = true;
        completeTime = Time.time;
        // TODO: Invoke event?
        EventManager.current.WaveFinished(waveIndex);
    }

    public TD_EnemyData GetEnemy(int enemyIndex)
    {
        // TODO: better handling around out of range exception here 
        if (enemyIndex < enemiesToSpawn.Count)
        {
            enemiesSpawnedCount++;
            return enemiesToSpawn[enemyIndex];
        }
        return null;
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
