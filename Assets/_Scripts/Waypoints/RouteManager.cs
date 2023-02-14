using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class RouteManager : MonoBehaviour
{
    /// <summary>
    /// each map may have multiple Routes. Each route will have its own route manager
    /// </summary>
    [SerializeField]
    private WaypointRoute route;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Static
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
    #endregion
}
