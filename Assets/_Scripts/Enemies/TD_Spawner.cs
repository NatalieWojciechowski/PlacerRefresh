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

    private void ToggleSpawnerEffects(int waveIndx)
    {
        Debug.Log("ToggleSpawnerEffects"+ waveIndx.ToString());
        if (!ActiveSpawnerEffects) return;
        if (waveHelpers.Count > waveIndx)
        {
            bool currentlyActive = ActiveSpawnerEffects.activeSelf;
            if (waveHelpers[waveIndx]?.WaveDetails?.waveContents?.Count < 0) ActiveSpawnerEffects.SetActive(false);
            else if (!CurrentWaveComplete) ActiveSpawnerEffects.SetActive(true);
        }
        else ActiveSpawnerEffects.SetActive(false);
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
        if (!TD_GameManager.instance) return;
        if (waveHelpers.Count == 0 && !SetupWaveHelpers()) return;
        if (TD_GameManager.instance.CurrentWaveIndex >= waveHelpers.Count) return;


        TD_Wave currentWave = waveHelpers[TD_GameManager.instance.CurrentWaveIndex];
        if (currentWave == null || !SpawnAllowed) return;
        CheckEnemies();
        CurrentWaveComplete = currentWave.Defeated;

        // If still have initialized with enemies & we havent spawned them all
        if (currentWave.AllSpawned)
        {
            ActiveSpawnerEffects?.SetActive(false);
            currentWave.WaveSpawningComplete();
            //Debug.Log(_enemiesAlive.Count);
            _enemiesAlive.RemoveAll((enemy) => enemy == null);
            //Debug.Log(_enemiesAlive.Count);
            if (_enemiesAlive.Count <= 0)
            {
                currentEnemyIndex = 0;
                CurrentWaveComplete = true;
                spawnedEntities.RemoveAll((entity) => (entity == null));
                currentWave.EndWave();
                // Reset this for the next iteration in case we are at the end 
            }
        }
        else if (IsDelayTimerMet() && SpawnPlacementValid()) SpawnEnemy(currentWave.GetEnemy(currentEnemyIndex));
        //else if (spawnedEntities.Count >= _enemiesToSpawn.Count && SpawnAllowed) AddWave(GetWaveEnemies(_nextWaveIndex), true);
        //else Debug.DebugBreak();
    }

    private void CheckEnemies()
    {
        _enemiesAlive.RemoveAll((enemy) => (enemy == null));
        //foreach(TD_Enemy enemy in _enemiesAlive)
        //{
        //    if (enemy == null) _enemiesAlive.RemoveAll()
        //}
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
}
