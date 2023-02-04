using System;
using UnityEngine;

public class AreaTower : TD_Building
{
    [SerializeField]
    TD_AOE effectAreaRange;

    protected float _aoeStartTime = 0f;
    protected bool _isActiveEffect = false;

    public float TickDamage = 0f;

    public AreaTower(TD_BuildingData buildingData) : base(buildingData)
    {
        SetStats(buildingData);
        IsRunning = false;
        TryBuildingState(BuildingState.Blueprint);
    }

    protected override void Start()
    {
        base.Start();
        SetStats(_baseBuildingData);
    }

    protected override void SpawnProjectile()
    {

    }

    protected override void ActOnTarget()
    {

        base.ActOnTarget();
    }

    protected override void ExitedRange()
    {
        effectAreaRange.gameObject.SetActive(false);
        inRangeEffects?.SetActive(false);
        base.ExitedRange();
    }

    protected override void EnteredRange()
    {
        effectAreaRange.AdjustInRangeVisualSize();
        effectAreaRange.gameObject.SetActive(true);
        inRangeEffects?.SetActive(true);
        base.EnteredRange();
    }


    //private void StartAOE()
    //{
    //    effectAreaRange.gameObject.SetActive(true);
    //    inRangeEffects.SetActive(true);
    //    _isActiveEffect = true;
    //    _aoeStartTime = Time.time;
    //}

    //private void EndAOE()
    //{
    //    effectAreaRange.gameObject.SetActive(false);
    //    inRangeEffects.SetActive(false);
    //    _isActiveEffect = false;
    //    _lastAction = Time.time;
    //    _aoeStartTime = 0f;
    //}

    /// <summary>
    /// Called after SetStats, allowing some post-loading adjustments 
    /// </summary>
    protected override void BuildingInit(TD_BuildingData sourceBuildingData = null)
    {
        base.BuildingInit(sourceBuildingData);
        TickDamage = _sBuildingData.Damage / _sBuildingData.AttackSpeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    //private void AdjustRange()
    //{
    //    //effectAreaRange.transform.localScale = Vector3.one * _sBuildingData.AttackRange;
    //    // Roughly inside tangent of range
    //    //effectAreaRange.GetComponent<TD_AOE>().AdjustRange(_sBuildingData.AttackRange);
    //}

    // TODO: create an AOE equivalent of the projectile; this wont trigger naturally atm
    //private void OnCollisionEnter(Collision collision)
    //{
    //    TD_Enemy inRange = collision.gameObject.GetComponent<TD_Enemy>();
    //    if (inRange)
    //    {
    //        inRange.TakeDamage(_sBuildingData.Damage);
    //    }
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    // "Tick"
    //    if (_isActiveEffect && _aoeStartTime % 5 == 0)
    //    {
    //        TD_Enemy inRange = collision.gameObject.GetComponent<TD_Enemy>();
    //        if (inRange)
    //        {
    //            inRange.TakeDamage(_sBuildingData.Damage);
    //        }
    //    }
    //}
}
