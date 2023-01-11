using System;
using UnityEngine;

public class AreaTower : TD_Building
{
    [SerializeField]
    GameObject effectAreaRange;

    protected float _aoeStartTime = 0f;
    protected bool _isActiveEffect = false;

    public AreaTower(TD_BuildingData buildingData) : base(buildingData)
    {

    }

    protected override void SpawnProjectile()
    {
        // If we have activated AOE, check if we should turn it off, then reset timers for it OR skip rest
        if (_isActiveEffect) {
            if (Time.time - _aoeStartTime < _buildingData.aoeActiveDuration) return;
            EndAOE();
        } else if (!_isActiveEffect && Time.time - _lastAction > _buildingData.projectileDelay) {
            StartAOE();
        }
    }

    private void StartAOE()
    {
        effectAreaRange.gameObject.SetActive(true);
        inRangeEffects.SetActive(true);
        _isActiveEffect = true;
        _aoeStartTime = Time.time;
    }

    private void EndAOE()
    {
        effectAreaRange.gameObject.SetActive(false);
        inRangeEffects.SetActive(false);
        _isActiveEffect = false;
        _lastAction = Time.time;
        _aoeStartTime = 0f;
    }

    /// <summary>
    /// Called after SetStats, allowing some post-loading adjustments 
    /// </summary>
    protected override void BulidingInit()
    {
        if (effectAreaRange) AdjustRange();
        base.BulidingInit();
    }

    private void AdjustRange()
    {
        effectAreaRange.transform.localScale = Vector3.one * _attackRange;
        // Roughly inside tangent of range
        effectAreaRange.GetComponent<CapsuleCollider>().radius = _buildingData.attackRange * 0.75f;
    }

    // TODO: create an AOE equivalent of the projectile; this wont trigger naturally atm
    private void OnCollisionEnter(Collision collision)
    {
        TD_Enemy inRange = collision.gameObject.GetComponent<TD_Enemy>();
        if (inRange)
        {
            inRange.TakeDamage(_buildingData.baseDamage);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // "Tick"
        if (_isActiveEffect && _aoeStartTime % 5 == 0)
        {
            TD_Enemy inRange = collision.gameObject.GetComponent<TD_Enemy>();
            if (inRange)
            {
                inRange.TakeDamage(_buildingData.baseDamage);
            }
        }
    }
}
