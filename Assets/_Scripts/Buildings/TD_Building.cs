using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Building : MonoBehaviour
{
    [SerializeField]
    TD_BuildingData _buildingData;

    private float _attackRange = 0.75f;
    [SerializeField]
    private TD_Enemy _buildingTarget;

    protected TargetingType targetingType;

    // TODO: Change this back
    public bool IsRunning = true;

    protected enum TargetingType
    {
        Nearest,
        First,
        Last,
        Furthest,
        Strongest,
        Weakest,
    }

    public TD_Building(TD_BuildingData bData)
    {
        SetStats(bData);
        IsRunning = true;
    }

    private void BulidingInit()
    {

    }

    public void SetStats(TD_BuildingData bData)
    {
        if (!bData) return;
        _buildingData = bData;
        _attackRange = _buildingData.attackRange;

        PieceBehaviour pieceBehaviour = GetComponent<PieceBehaviour>();
        if (pieceBehaviour)
        {
            pieceBehaviour.Name = bData.displayName;
            pieceBehaviour.Icon = bData.icon;
            pieceBehaviour.Description = bData.description;
            pieceBehaviour.Category = bData.category;
        }
        // callbacks / Powerup animations/ etc
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_buildingData == null) _buildingData = GetComponent<TD_BuildingData>();
        if (IsRunning == false) SetStats(_buildingData);
        IsRunning = true;

        Debug.Log("Building Data: " + _buildingData);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        if (_buildingTarget) Gizmos.DrawLine(transform.position, _buildingTarget.transform.position);
    }
#endif

    // Update is called once per frame
    void Update()
    {
        if (IsRunning) CheckTargets();
        if (_buildingTarget) ActOnTarget();
    }

    protected virtual void CheckTargets()
    {
        // TODO: Method to grab all enemies from a manager to iterate over rather than searching
        TD_Enemy[] enemies = FindObjectsOfType<TD_Enemy>();
        foreach (TD_Enemy enemy in enemies)
        {
            float _distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (_distance < _attackRange) {
                SetTarget(enemy);
                break;
            }
        }
    }

    private void SetTarget(TD_Enemy enemy)
    {
        _buildingTarget = enemy;
        // TODO: any animation / visualization here? 
    }

    protected virtual void ActOnTarget()
    {

    }

}
