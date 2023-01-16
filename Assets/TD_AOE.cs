using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_AOE : TD_Projectile
{
    [SerializeField]
    protected TD_AOEData td_AOEData;
    [SerializeField]
    protected Collider _collider;
    private TD_Building _owningBuilding;
    private TD_Enemy _parentTarget;

    /// <summary>
    /// Represents the region that remains active while enemies are in range. 
    /// </summary>
    #region AOE Region Activity
    protected bool _isActiveEffect = false;
    protected float _aoeStartTime = 0f;
    protected float _aoeActiveDuration = 0f;
    #endregion

    /// <summary>
    /// Pulse represents the timeframe when enemes take damage if still within range. 
    /// </summary>
    #region Pulse
    private float _lastPulse;
    private float _pulseDelay;
    private bool _pulseActive;
    private object inRangeEffects;
    #endregion 

    protected override void Start()
    {
        _collider = GetComponent<Collider>();
        _owningBuilding = myParent.GetComponent<TD_Building>();
        _pulseDelay = td_AOEData.pulseDelay;
        AdjustRange(_owningBuilding.GetStats().AttackRange);
        base.Start();
    }

    private void OnEnable()
    {
        if(!_collider) _collider = GetComponent<Collider>();
        if(!_owningBuilding) _owningBuilding = myParent.GetComponent<TD_Building>();
        AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

    protected override void Update()
    {
        if (!_owningBuilding) return;
        
        if (_owningBuilding != null && _owningBuilding.IsInRange)
        {
            Debug.Break();
        }

        // If we have activated AOE, check if we should turn it off, then reset timers for it OR skip rest
        if (_isActiveEffect)
        {
            if (!_owningBuilding.BuildingTarget) EndAOE();
            if (PulseReady()) Pulse();
            //if (PulseStillGoing()) return;
            //EndAOE();
        }
        else if (!_isActiveEffect && _owningBuilding.BuildingTarget)
        {
            StartAOE();
        }

        base.Update();
    }

    public override void InitProjectile(TD_Building tD_Building, TD_Enemy buildingTarget)
    {
        // Base uses a specific target
        //base.InitProjectile(tD_Building, buildingTarget);
        //_owningBuilding = tD_Building;
        //AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

    public void AdjustRange(float bDataRange)
    {
        //effectAreaRange.transform.localScale = Vector3.one * _sBuildingData.AttackRange;
        // Roughly inside tangent of range
        GetComponent<CapsuleCollider>().radius = bDataRange * 0.75f;
    }

    #region AOE Region
    private void StartAOE()
    {
        Debug.Log("Start AOE");
        _isActiveEffect = true;
        _aoeStartTime = Time.time;
    }

    private void EndAOE()
    {
        Debug.Log("END AOE");
        _isActiveEffect = false;
        _aoeStartTime = 0f;
        EndPulse();
        gameObject.SetActive(false);
    }
    #endregion

    #region Pulse
    private void Pulse()
    {
        Debug.Log("Pulse");
        _collider.enabled = true;
    }

    private void EndPulse()
    {
        Debug.Log("End Pulse");
        _lastPulse = Time.time;
        _collider.enabled = false;
    }

    private bool PulseReady()
    {
        return (!_isActiveEffect && _lastPulse - Time.time > _pulseDelay);
    }

    private bool PulseStillGoing()
    {
        return Time.time - _aoeStartTime < _aoeActiveDuration;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        if (!PulseReady()) return;
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            cEnemy.TakeDamage(projectileDamage);
        }
        Debug.Log("Collision enter dmg");
        EndPulse();
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (!PulseReady()) return;
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            cEnemy.TakeDamage(projectileDamage);
        }
        Debug.Log("Collision stay dmg");
        EndPulse();
    }
    #endregion
}
