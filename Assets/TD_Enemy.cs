using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TD_Enemy : MonoBehaviour
{
    [SerializeField]
    protected WaypointRoute fullRoute;
    protected Transform prevWaypoint;
    [SerializeField]
    protected Transform nextWaypoint;
    private string _displayName;
    private float _moveSpeed = 1f;
    private float _maxHealth = 1f;
    private Rigidbody _rigidbody;
    private Animator _animator;

    public int currentWaypointIndx = -1;
    [SerializeField]
    private float _turnSpeed = 0.5f;
    private float _lastWaypointSwitchTime;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO: With multi spawners, need find closest
        if (fullRoute == null) fullRoute = WaypointManager.current.GetRoute();
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (!prevWaypoint) prevWaypoint = transform;
        //WaypointManager.current.AddEnemyToCircuit(fullRoute);
        UpdateNextWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (nextWaypoint) MoveToWaypoint();

    }

    private void MoveToWaypoint()
    {
        if (ReachedPoint())
        {
            _animator.SetBool("IsMoving", false);
            Debug.Log("reached next point");
            UpdateNextWaypoint();
            Debug.Log($"Got Next Point: {nextWaypoint}");
        }
        if (!nextWaypoint) return;
        transform.position = GetLerpPosition();
        //transform.position = lerpPosition();
        //Vector3 nextLookPosition = Vector3.Lerp(transform.position, nextWaypoint.position, 0.15f);
        //transform.LookAt(nextLookPosition);
        //_rigidbody?.AddRelativeForce(Vector3.forward * _moveSpeed, ForceMode.Force);
    }

    private Vector3 GetLerpPosition()
    {
        float pathLength = Vector3.Distance(prevWaypoint.position, nextWaypoint.position);
        float totalTimeForPath = pathLength / _moveSpeed;
        float currentTimeOnPath = Time.time - _lastWaypointSwitchTime;
        return Vector3.Lerp(prevWaypoint.position, nextWaypoint.position, currentTimeOnPath / totalTimeForPath);
    }

    internal void SetStats(WaveEnemyData waveEnemyData)
    {
        _displayName = waveEnemyData.displayName;
        _moveSpeed = waveEnemyData.moveSpeed;
        _maxHealth = waveEnemyData.health;
    }

    private bool ReachedPoint()
    {
        if (Vector3.Distance(transform.position, nextWaypoint.position) < 0.05f)
        {
            // Stop animations? 
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            return true;
        }
        return false;
    }

    private void UpdateNextWaypoint()
    {
        if (!fullRoute) return;
        nextWaypoint = fullRoute.NextWaypoint(currentWaypointIndx);
        if (nextWaypoint != null)
        {
            currentWaypointIndx++;
            _lastWaypointSwitchTime = Time.time;
            _animator.SetBool("IsMoving", true);
        }
        else OnReachEnd();

        //nextWaypoint = WaypointManager.NextWaypoint(fullRoute, currentWaypointIndx);
        //if (nextWaypoint != null) currentWaypointIndx++;
        //nextWaypoint = WaypointManager.NextWaypoint(fullRoute, transform);
    }

    private void OnReachEnd()
    {
        // Damage Total health
        // Destroy self 
        Destroy(this.gameObject);
    }
}