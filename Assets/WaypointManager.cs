using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager current;

    [SerializeField]
    private List<WaypointRoute> routes;
    //private List<Transform> waypoints;
    //TODO: have group of routes

    //private WaypointCircuit _waypointCircuit;
    //private WaypointCircuit.WaypointList waypointList;

    private List<TD_Enemy> _enemies;
    TDEnemyManager tDEnemyManager;

    // Start is called before the first frame update
    void Start()
    {
        if (current == null) current = this;
        else Destroy(this);

        if (routes == null) routes = new();
        if (!tDEnemyManager) tDEnemyManager = FindObjectOfType<TDEnemyManager>();
    }

    internal WaypointRoute GetRoute()
    {
        // TODO: 
        if (routes.Count > 0) return routes[0];
        Debug.LogError("No Routes");
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (Transform wpTransform in _waypointCircuit.Waypoints)
        //{

        //}        
    }

    public static Transform NearestNodeFromPos(WaypointCircuit wpCircuit, Vector3 targetPosition)
    {
        float _closestDist = 1000000f;
        Transform _closestPointT = null;
        foreach (Transform wpTransform in wpCircuit.Waypoints)
        {
            if (Vector3.Distance(targetPosition, wpTransform.position) < _closestDist) _closestPointT = wpTransform;
        }
        return _closestPointT;
    }

    public static Transform NextWaypoint(WaypointRoute wpRoute, Transform fromWaypoint)
    {
        int foundIndex = -2;
        int itrIndex = -1;
        foreach (Transform wpNode in wpRoute.Waypoints)
        {
            itrIndex++;
            if (Vector3.Distance(fromWaypoint.position, wpNode.position) < 5f)
            {
                foundIndex = itrIndex;
                break;
            }
        }
        if (foundIndex != -2 && foundIndex < wpRoute.Waypoints.Count + 1) return wpRoute.Waypoints[foundIndex++];
        return null;
    }

    public static Transform NextWaypoint(WaypointRoute wpRoute, int fromWaypointIndx)
    {
        if (fromWaypointIndx < wpRoute.Waypoints.Count + 1) return wpRoute.Waypoints[fromWaypointIndx + 1];
        return null;
    }

    public void AddEnemyToCircuit(WaypointRoute wpRoute)
    {
        if (wpRoute) tDEnemyManager.WaypointRoute = wpRoute;
    }

}
