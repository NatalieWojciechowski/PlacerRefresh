using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_Building : MonoBehaviour
{
    [SerializeField]
    protected TD_BuildingData _buildingData;

    protected float _attackRange = 0.75f;
    [SerializeField]
    protected TD_Enemy _buildingTarget;

    protected TargetingType targetingType;

    public Animator bAnimator;
    public GameObject inRangeEffects;

    // TODO: Change this back
    public bool IsRunning = true;
    public bool IsInRange = false;

    protected float _lastAction = 0f;

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

    protected virtual void BulidingInit()
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
        BulidingInit();
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (_buildingData == null) _buildingData = GetComponent<TD_BuildingData>();
        if (!bAnimator) bAnimator = GetComponent<Animator>();
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
        if (IsInRange) ToggleEffects(true);
    }

    private void ToggleEffects(bool shouldShow)
    {
        if (bAnimator) bAnimator.SetBool("InRange", shouldShow);
        if (inRangeEffects) inRangeEffects.SetActive(shouldShow);
    }

    protected virtual void CheckTargets()
    {
        // TODO: Method to grab all enemies from a manager to iterate over rather than searching
        TD_Enemy plannedEnemy = null;
        TD_Enemy[] enemies = FindObjectsOfType<TD_Enemy>();
        foreach (TD_Enemy enemy in enemies)
        {
            float _distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (_distance < _attackRange) {
                plannedEnemy = enemy;
                break;
            }
        }
        SetTarget(plannedEnemy);
    }

    private void SetTarget(TD_Enemy enemy)
    {
        IsInRange = enemy != null;
        // Losing the target
        if (_buildingTarget && !IsInRange) ToggleEffects(false);
        _buildingTarget = enemy;
        // TODO: any animation / visualization here? 
    }

    protected virtual void ActOnTarget()
    {
        if (_buildingData.projectilePrefab && _buildingData.projectileOffset != null)
        {
            SpawnProjectile();
        }
    }

    protected virtual void SpawnProjectile()
    {
        if (Time.time - _lastAction > _buildingData.projectileDelay)
        {
            GameObject lastProjectile = Instantiate(_buildingData.projectilePrefab, transform);
            lastProjectile.transform.Translate(_buildingData.projectileOffset);
            lastProjectile.transform.LookAt(_buildingTarget.transform.position);
            transform.LookAt(_buildingTarget.transform.position);
            // TODO: assign owner / target? 
            TD_Projectile td_projectile = lastProjectile.GetComponent<TD_Projectile>();
            td_projectile.InitProjectile(this, _buildingTarget);
            _lastAction = Time.time;
        }
    }
}
