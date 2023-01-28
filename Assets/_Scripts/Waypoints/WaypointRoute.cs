using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRoute : MonoBehaviour
{
    [SerializeField]
    List<Transform> transforms;
    private TD_Spawner tdSpawner;

    public List<Transform> Waypoints { get => transforms; set => transforms = value; }
    public TD_Spawner TdSpawner { get => tdSpawner; set => tdSpawner = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (transforms.Count == 0) WaypointsFromChildren();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public bool SetSpawner(TD_Spawner fromSpawner)
    //{
    //    // TODO: Validation
    //    tdSpawner = fromSpawner;
    //    return tdSpawner != null;
    //}

    public Transform NextWaypoint(int fromIdx)
    {
        if (fromIdx >= transforms.Count - 1) return null;
        // TODO: guard clausing 
        return transforms[fromIdx + 1];
    }

    private void WaypointsFromChildren()
    {
        if (transforms.Count == 0 && gameObject.transform.childCount > 0)
            // TODO: will include route if done automatically atm
            transforms.AddRange(GetComponentsInChildren<Transform>());
        else transforms = new();
    }

    //internal void GetSpawner()
    //{
    //    return tdSpawner;
    //}
}
