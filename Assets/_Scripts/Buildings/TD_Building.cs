using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TD_Building : MonoBehaviour
{
    [SerializeField]
    protected TD_BuildingData _baseBuildingData;

    protected BuildingData _sBuildingData;

    //protected float _attackRange = 0.75f;
    //private float _baseDamage;
    [SerializeField]
    private TD_Enemy _buildingTarget;

    protected TargetingType targetingType;

    public Animator bAnimator;
    public GameObject inRangeEffects;

    // TODO: Change this back
    public bool IsRunning = true;
    public bool IsInRange = false;

    protected float _lastAction = 0f;
    private float effectToggleDelay = .25f;
    private float effectLastToggle = 0;

    //private int _currentTier = 1;
    //private int _maxTier = 1;

    public int EnemyKillCount { get; internal set; }
    public Guid BuildingUUID { get; private set; }
    public TD_Enemy BuildingTarget { get => _buildingTarget; }

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

    protected virtual void BuildingInit()
    {
        // Any adjustments to make with building data now that we have the base? 

    }

    public void SetStats(TD_BuildingData bData)
    {
        if (!bData) return;
        _baseBuildingData = bData;
        _sBuildingData = new BuildingData(bData);
        //_attackRange = _baseBuildingData.attackRange;
        //_baseDamage = _baseBuildingData.baseDamage;
        //_currentTier = _baseBuildingData.CurrentTier;
        //_maxTier = _baseBuildingData.MaxTier;

        PieceBehaviour pieceBehaviour = GetComponent<PieceBehaviour>();
        if (pieceBehaviour)
        {
            pieceBehaviour.Name = bData.displayName;
            pieceBehaviour.Icon = bData.icon;
            pieceBehaviour.Description = bData.description;
            pieceBehaviour.Category = bData.category;
        }
        // callbacks / Powerup animations/ etc
        BuildingInit();
    }


    internal BuildingData GetStats()
    {
        // TODO: Any adjustments like upgrades to make here before returning; 
        // Copy the stats to new struct? 
        return this._sBuildingData;
        //return this._baseBuildingData;
    }

    // Start is called before the first frame update
    void Start()
    {
        BuildingUUID = Guid.NewGuid();
        //if (_buildingData == null) _buildingData = GetComponent<TD_BuildingData>();
        if (!bAnimator) bAnimator = GetComponent<Animator>();
        if (IsRunning == false) SetStats(_baseBuildingData);
        IsRunning = true;

        Debug.Log("Building Data: " + _baseBuildingData);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _sBuildingData.AttackRange);
        if (_buildingTarget) Gizmos.DrawLine(transform.position, _buildingTarget.transform.position);
    }
#endif

    // Update is called once per frame
    void Update()
    {
        bool beganWithTarget = (_buildingTarget?.EnemyUUID != null);
        // Validation
        if (IsRunning) CheckTargets();
        if (_buildingTarget)
        {
            if (!beganWithTarget) EnteredRange();
            ActOnTarget();
        }
        else if (!_buildingTarget && IsInRange) ExitedRange();

        //// Visual
        //if (IsInRange) ToggleEffects(true);
    }

    protected virtual void EnteredRange()
    {
        Debug.Log("JUST went INTO range");
        ToggleEffects(true);
    }

    protected virtual void ExitedRange()
    {
        Debug.Log("JUST went out of range");
        ToggleEffects(false);
    }

    private void ToggleEffects(bool shouldShow)
    {
        if (Time.time - effectLastToggle < effectToggleDelay) return;
        Debug.Log("Toggle Effects: " + shouldShow);
        if (bAnimator) bAnimator.SetBool("InRange", shouldShow);
        if (inRangeEffects) inRangeEffects.SetActive(shouldShow);
        effectLastToggle = Time.time;
    }

    protected virtual void CheckTargets()
    {
        TD_Enemy startingEnemy = _buildingTarget;
        // TODO: Method to grab all enemies from a manager to iterate over rather than searching
        TD_Enemy plannedEnemy = null;
        TD_Enemy[] enemies = FindObjectsOfType<TD_Enemy>();
        foreach (TD_Enemy enemy in enemies)
        {
            float _distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (_distance < _sBuildingData.AttackRange) {
                plannedEnemy = enemy;
                break;
            }
        }
        if (!plannedEnemy && !_buildingTarget || plannedEnemy?.EnemyUUID == _buildingTarget?.EnemyUUID) return;
        SetTarget(plannedEnemy);
    }

    internal bool TryUpgrade()
    {
        if (_sBuildingData.Level >= _sBuildingData.MaxLevel) return false;
        //_currentTier++;
        _sBuildingData.LevelUp();
        _sBuildingData.Damage = (float)Math.Round(_sBuildingData.Damage * 1.5f);
        return true;
        //return _buildingData.upgradesTo != null;
    }

    internal bool TrySell()
    {
        // May have conditions like "immovable" or corrupted, etc
        Destroy(this.gameObject, 0.25f);
        return true;
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
        if (_baseBuildingData.projectilePrefab && _baseBuildingData.projectileOffset != null)
        {
            SpawnProjectile();
        }
    }

    protected virtual void SpawnProjectile()
    {
        if (Time.time - _lastAction > _baseBuildingData.projectileDelay)
        {
            GameObject lastProjectile = Instantiate(_baseBuildingData.projectilePrefab, transform);
            lastProjectile.transform.Translate(_baseBuildingData.projectileOffset);
            lastProjectile.transform.LookAt(_buildingTarget.transform.position);
            transform.LookAt(_buildingTarget.transform.position);
            // TODO: assign owner / target? 
            TD_Projectile td_projectile = lastProjectile.GetComponent<TD_Projectile>();
            td_projectile.InitProjectile(this, _buildingTarget);
            _lastAction = Time.time;
        }
    }

    private void OnMouseUp()
    {
        EventManager.OnTowerSelect(this);
    }
}
