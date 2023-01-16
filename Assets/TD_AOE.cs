using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TD_AOE : MonoBehaviour
{
    [SerializeField]
    protected TD_AOEData td_AOEData;
    [SerializeField]
    protected Collider _collider;
    [SerializeField]
    private AreaTower _owningBuilding;
    private TD_Enemy _parentTarget;

    /*
     * AOE Region Active  - AOE REGION INACTIVE  -  AOE REGION ACTIVE
     * [Pulse]   [Pulse]                            [Pulse]   [Pulse]
     * 
     */
    /// <summary>
    /// Represents the region that remains active while enemies are in range. 
    /// </summary>
    #region AOE Region Activity
    protected bool _isAOEActive = false;
    protected float _aoeStartTime = 0f;
    protected float _aoeActiveDuration = 0f;
    #endregion

    /// <summary>
    /// Pulse represents the timeframe when enemes take damage if still within range. 
    /// </summary>
    #region Pulse
    private float _lastPulse;
    private float _pulseDelay;
    private bool _isPulseActive = false;
    protected float _pulseStartTime = 0f;
    protected float _pulseActiveDuration = 0f;
    private GameObject inRangeEffects;
    #endregion 

    protected void Start()
    {
        _collider = GetComponent<Collider>();
        _owningBuilding = GetComponentInParent<AreaTower>();
        _pulseDelay = td_AOEData.pulseDelay;
        AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

    private void OnEnable()
    {
        if(!_collider) _collider = GetComponent<Collider>();
        if(!_owningBuilding) _owningBuilding = GetComponentInParent<AreaTower>();
        AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (_owningBuilding.BuildingTarget && _isPulseActive) Gizmos.DrawWireSphere(transform.position, _collider.bounds.max.magnitude);
        if (_owningBuilding.BuildingTarget && _isPulseActive) Gizmos.DrawLine(transform.position, _owningBuilding.BuildingTarget.transform.position);
    }
#endif

    protected void Update()
    {
        if (!_owningBuilding) return;

        //if (_owningBuilding != null && _owningBuilding.IsInRange)
        //{
        //    Debug.Break();
        //}

        // If we have activated AOE, check if we should turn it off, then reset timers for it OR skip rest
        if (_isAOEActive)
        {
            if (!_owningBuilding.BuildingTarget) { EndAOE(); return; }
            else if (PulseReady()) { Pulse(); }
            else {
                if (PulseStillGoing()) return;
                else EndPulse();
            }
        }
        else if (!_isAOEActive && _owningBuilding.BuildingTarget)
        {
            StartAOE();
        }
    }

    public void AdjustRange(float bDataRange)
    {
        //effectAreaRange.transform.localScale = Vector3.one * _sBuildingData.AttackRange;
        // Roughly inside tangent of range
        transform.localScale = new Vector3(bDataRange, bDataRange, bDataRange);
        //GetComponent<CapsuleCollider>().radius = bDataRange * 0.75f;
    }

    #region AOE Region
    private void StartAOE()
    {
        Debug.Log("Start AOE");
        _isAOEActive = true;
        _aoeStartTime = Time.time;
    }

    private void EndAOE()
    {
        Debug.Log("END AOE");
        _isAOEActive = false;
        _aoeStartTime = 0f;
        EndPulse();
        gameObject.SetActive(false);
    }
    #endregion

    #region Pulse
    private void Pulse()
    {
        if (_isPulseActive) return;
        Debug.Log("Pulse");
        _isPulseActive = true;
        _collider.enabled = true;
        _pulseStartTime = Time.time;
    }

    private void EndPulse()
    {
        if (!_isPulseActive) return;
        inRangeEffects?.SetActive(false);
        Debug.Log("End Pulse");
        _isPulseActive = false;
        _lastPulse = Time.time;
        _collider.enabled = false;
    }

    private bool PulseReady()
    {
        return (_isAOEActive && !_isPulseActive && Time.time - _lastPulse > _pulseDelay);
    }

    private bool PulseStillGoing()
    {
        return _isPulseActive && Time.deltaTime < float.Epsilon;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        //if (!PulseStillGoing() && !PulseReady()) return;
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            cEnemy.TakeDamage(_owningBuilding.TickDamage);
        }
        Debug.Log("Collision enter dmg");
        //if (!PulseStillGoing()) EndPulse();
    }

    protected void OnCollisionStay(Collision collision)
    {
        //if (!PulseStillGoing() && !PulseReady()) return;
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            cEnemy.TakeDamage(_owningBuilding.TickDamage);
        }
        Debug.Log("Collision stay dmg");
        //if (!PulseStillGoing()) EndPulse();
    }
    #endregion
}
