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
    private bool waveSpawned = false;
    private bool waveEnded = false;


    public bool AllSpawned { get => allSpawned; }
    private bool finished = false;
    private bool allSpawned = false;

    private bool defeated = false;
    public bool Defeated { get => defeated; }
    public float StartTime { get => startTime; set => startTime = value; }
    public float CompleteTime { get => completeTime; set => completeTime = value; }

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
        if (defeated) return;
        waveEnded = true;
        Debug.Log("END WAVE");
        defeated = true;
        completeTime = Time.time;
        // TODO: Invoke event?
        EventManager.instance.WaveFinished(waveIndex);
    }
    public void WaveSpawningComplete()
    {
        if (allSpawned == true) return;
        allSpawned = true;
        spawner.OnAllWaveSpawned(this);
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
        if (!waveEnded && checkAllSpawned()) WaveSpawningComplete();
        return null;
    }

    private bool checkAllSpawned()
    {
        return (enemiesSpawnedCount > 0 && enemiesSpawnedCount == enemiesToSpawn.Count);
    }
}
