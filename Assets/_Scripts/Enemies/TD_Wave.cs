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


    private bool waveStarted = false;
    private bool waveEnded = false;

    private bool allSpawned = false;
    public bool AllSpawned { get => checkAllSpawned(); }


    private bool allDefeated = false;
    public bool AllDefeated { get => checkAllDefeated(); }
    public float StartTime { get => startTime; set => startTime = value; }
    public float CompleteTime { get => completeTime; set => completeTime = value; }

    public WaveDetails WaveDetails;

    public TD_Wave(int _waveIndex, TD_Spawner _spawner, WaveDetails _waveDetails) {
        // TODO: do these need to be unique across spawners? 
        waveIndex = _waveIndex;
        spawner = _spawner;
        waveDetails = _waveDetails;
        enemiesToSpawn = new();
        enemiesToSpawn.AddRange(waveDetails.waveContents);
    }

    /// <summary>
    /// Trigger wave started event and update startTime. Passes index of THIS wave when triggering event.
    /// </summary>
    public void StartWave()
    {
        if (waveStarted) return;
        waveStarted = true;
        startTime = Time.time;
        // TODO: Invoke Event?
        EventManager.instance.WaveStarted(waveIndex);
    }

    public void EndWave()
    {
        if (allDefeated) return;
        waveEnded = true;
        Debug.Log("END WAVE");
        allDefeated = true;
        completeTime = Time.time;
        // TODO: Invoke event?
        EventManager.instance.WaveFinished(waveIndex);
    }

    public TD_EnemyData GetEnemy(int enemyIndex)
    {
        if (waveEnded) return null;
        if (!waveStarted) StartWave();
        // TODO: better handling around out of range exception here 
        if (enemyIndex < enemiesToSpawn.Count)
        {
            enemiesSpawnedCount++;
            return enemiesToSpawn[enemyIndex];
        }
        if (!waveEnded && checkAllSpawned()) onSpawningComplete();
        return null;
    }

    private bool checkAllSpawned()
    {
        return (enemiesSpawnedCount > 0 && enemiesSpawnedCount == enemiesToSpawn.Count);
    }
    private bool checkAllDefeated()
    {
        return allDefeated || spawner.IsCurrentWaveComplete; // && enemiesRemaining;
    }

    public void onSpawningComplete()
    {
        if (allSpawned == true) return;
        allSpawned = true;
        spawner.OnAllWaveSpawned(this);
    }
}
