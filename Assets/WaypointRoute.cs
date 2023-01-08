using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointRoute : MonoBehaviour
{
    [SerializeField]
    List<Transform> transforms;

    public List<Transform> Waypoints { get => transforms; set => transforms = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (transforms.Count == 0) WaypointsFromChildren();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

}
