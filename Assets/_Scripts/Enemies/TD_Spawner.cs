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
    private List<TD_EnemyData> _enemiesToSpawn;
    private List<TD_Enemy> _enemiesAlive;
    private int currentEnemyIndex = 0;
    //private List<TD_Wave> _waves;
    private List<GameObject> spawnedEntities;
    private List<TD_Wave> waveHelpers;
    public bool CurrentWaveComplete = false;

    [SerializeField]
    private Transform SpawnPosition;
    /// <summary>
    ///  How far apart the entities should be before spawning next
    /// </summary>
    private float spawnDensity = 0.5f;
    private int lastSpawnTime;

    private void Awake()
    {

    }

    //private void OnEnable()
    //{
    //    //int _waveIndex = 0;
    //    //foreach (WaveDetails waveDetail in Waves)
    //    //{
    //    //    waveHelpers.Add(new TD_Wave(_waveIndex, this, waveDetail));
    //    //    _waveIndex++;
    //    //}
    //    //EventManager.OnWaveStart += (waveIndx) => SpawnNextWave();            
    //}


    //private void OnDisable()
    //{
    //    waveHelpers.Clear();
    //    //EventManager.OnWaveStart -= (waveIndx) => SpawnNextWave();
    //}

    // Start is called before the first frame update
    void Start()
    {
        if (waveHelpers == null) waveHelpers = new();
        //if (_waves == null) _waves = new();
        if (spawnedEntities == null) spawnedEntities = new();
        if (_enemiesToSpawn == null) _enemiesToSpawn = new();
        if (_enemiesAlive == null) _enemiesAlive = new();
        if (!SpawnPosition) SpawnPosition = transform;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!TD_GameManager.current) return;
        if (waveHelpers.Count == 0 && !SetupWaveHelpers()) return;
        if (TD_GameManager.current.CurrentWaveIndex >= waveHelpers.Count) return;


        TD_Wave currentWave = waveHelpers[TD_GameManager.current.CurrentWaveIndex];
        if (currentWave == null || !SpawnAllowed) return;
        CurrentWaveComplete = currentWave.Defeated;
        //Debug.Log(currentWave);
        //Debug.Log(IsDelayTimerMet());
        // If still have initialized with enemies & we havent spawned them all
        if (currentWave.AllSpawned)
        {
            currentWave.WaveSpawningComplete();
            //Debug.Log(_enemiesAlive.Count);
            _enemiesAlive.RemoveAll((enemy) => enemy == null);
            //Debug.Log(_enemiesAlive.Count);
            if (_enemiesAlive.Count <= 0)
            {
                currentWave.EndWave();
                // Reset this for the next iteration in case we are at the end 
                CurrentWaveComplete = true;
                currentEnemyIndex = 0;
            }
        }
        else if (IsDelayTimerMet() && SpawnPlacementValid()) SpawnEnemy(currentWave.GetEnemy(currentEnemyIndex));
        //else if (spawnedEntities.Count >= _enemiesToSpawn.Count && SpawnAllowed) AddWave(GetWaveEnemies(_nextWaveIndex), true);
        //else Debug.DebugBreak();
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

    //protected void Spawn(TD_Wave currentWave)
    //{
    //    if (currentWave.AllSpawned)
    //    {
    //        currentWave.EndWave();
    //    } else
    //    {
    //        SpawnEnemy(currentWave.GetEnemy(currentEnemyIndex));
    //    }
    //    // TODO: change this to use something like IEnemy...
    //    //if (current < _enemiesToSpawn.Count) SpawnEnemy(_enemiesToSpawn[_nextEnemyIndex]);
    //}


    //IEnumerable<TD_EnemyData> EnemyQueue()
    //{
    //    foreach (TD_EnemyData enemy  in _enemiesToSpawn)
    //    {
    //        yield return enemy;
    //    }
    //    yield return ;
    //}

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
            lastSpawnTime = 0;
        }
    }

    ///// <summary>
    ///// Wrapper for actual access to the wave data
    ///// </summary>
    ///// <param name="waveIndex"></param>
    ///// <returns></returns>
    //private List<TD_EnemyData> GetWaveEnemies(int waveIndex)
    //{
    //    if (waveIndex >= Waves.Count) return null;
    //    return Waves[waveIndex]?.waveContents;
    //    //if (_waveEnemies.Count < 1)
    //    //    return null;
    //    //return _enemiesToSpawn
    //    // TODO:
    //    // Send event for wave start
    //}

    ///// <summary>
    ///// Whether a brand new wave or adding (early start) To the existing wave, add to spawn queue.
    ///// </summary>
    ///// <param name="tD_Enemies"></param>
    ///// <param name="clearPrevious">Whether or not to clear out the tracking lists</param>
    //private void AddWave(List<TD_EnemyData> tD_Enemies, bool clearPrevious = true)
    //{
    //    if (tD_Enemies == null || tD_Enemies.Count < 1) return;
    //    if (clearPrevious)
    //    {
    //        currentEnemyIndex = 0;
    //        _enemiesToSpawn.Clear();
    //        spawnedEntities.Clear();
    //    }
    //    _enemiesToSpawn.AddRange(tD_Enemies);
    //    //EventManager.WaveStarted(_nextWaveIndex);
    //    //currentWaveIndex++;
    //}

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
        return (Time.time - lastSpawnTime > Waves[TD_GameManager.current.CurrentWaveIndex].spawnInterval);
    }
}
