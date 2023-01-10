using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Template Class WIP
    //public class EnemySpawner<T>
    //{
    //    T item;

    //    public void UpdateItem(T newItem)
    //    {
    //        item = newItem;
    //    }
    //}
    //public T GenericMethod<T>(T param)
    //{
    //    return param;
    //}

    //// Start is called before the first frame update
    //void Start()
    //{
    //    EnemySpawner<Spawner> _spawnFactory = new EnemySpawner<Spawner>();
    //    _spawnFactory.UpdateItem(_spawnPrefab);
    //    GenericClass<int> myClass = new GenericClass<int>();

    //    myClass.UpdateItem(5);
    //}

    #endregion

    public GameObject SpawnPrefab;
    protected List<GameObject> spawnedEntities = new List<GameObject>();
    public float spawnInterval = 10f;
    public int spawnMax = 0;

    private int spawnedCount = 0;
    protected float lastSpawnTime = -1f;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (SpawnerActive()) Spawn();
        lastSpawnTime += Time.deltaTime;
    }

    protected virtual void Spawn()
    {
        if (SpawnPrefab && TimerComplete() && SpawnPlacementValid())
        {
            GameObject lastSpawned = Instantiate<GameObject>(SpawnPrefab, AdjustedSpawnPosition(), Quaternion.identity);
            spawnedEntities.Add(lastSpawned);
            lastSpawnTime = 0;
        }
    }

    protected virtual bool SpawnPlacementValid()
    {
        bool occupied = false;
        spawnedEntities.ForEach(entity => {
            occupied |= Vector3.Distance(entity.transform.position, AdjustedSpawnPosition()) < 1f;
        });
        return !occupied;
    }

    #region Spawn Helpers
    protected virtual bool SpawnerActive()
    {
        // TODO: Create another toggle for active state
        return spawnedEntities.Count <= spawnMax;
    }

    protected virtual Vector3 AdjustedSpawnPosition()
    {
        // TODO: Add offset
        // TODO: Check that the space is available
        return transform.position;
    }

    protected virtual bool TimerComplete()
    {
        return lastSpawnTime > spawnInterval;
    }
    #endregion
}
