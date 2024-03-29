using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDamageAOE : AreaEffect
{
    //private AreaTower _owningBuilding;
    private TD_Enemy _parentTarget;
    [SerializeField] private CapsuleCollider _capsuleCollider;

    /// <summary>
    /// Represents the region that remains active while enemies are in range. 
    /// </summary>


    /// <summary>
    /// Pulse represents the timeframe when enemes take damage if still within range. 
    /// </summary>

    protected override void Start()
    {
        //_owningBuilding = GetComponentInParent<AreaTower>();
        if (!_capsuleCollider) _capsuleCollider = GetComponent<CapsuleCollider>();
        //AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //if(!_owningBuilding) _owningBuilding = GetComponentInParent<AreaTower>();
        if(_capsuleCollider) AdjustInRangeVisualSize();
        //AdjustRange(_owningBuilding.GetStats().AttackRange);
    }

    public void AdjustInRangeVisualSize()
    {
        //if (!_owningBuilding) return;
        transform.localScale = Vector3.one;
        if (_capsuleCollider) _capsuleCollider.radius = td_AOEData.aoeRange;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (_owningBuilding.BuildingTarget && _isPulseActive) Gizmos.DrawWireSphere(transform.position, _collider.bounds.max.magnitude);
        if (_isPulseActive) Gizmos.DrawLine(transform.position, transform.position+Vector3.one*td_AOEData.aoeRange);
    }
#endif

    protected override void Update()
    {
        ////if (!_owningBuilding) return;

        //if (_isAOEActive) { EndAOE(); return; }
        //else if (!_isAOEActive) StartAOE();
       
        base.Update();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        //if (!PulseStillGoing() && !PulseReady()) return;
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            cEnemy.TakeDamage(td_AOEData.pulseEffectAmount);
        }
        Debug.Log("Collision enter dmg");
        //if (!PulseStillGoing()) EndPulse();
    }

    protected override void OnCollisionStay(Collision collision)
    {
        //if (!PulseStillGoing() && !PulseReady()) return;
        // Base uses a specific target
        //base.OnCollisionEnter(collision);
        TD_Enemy cEnemy;
        if (collision.gameObject.TryGetComponent<TD_Enemy>(out cEnemy))
        {
            Debug.Log("collision stay skipped");
            //cEnemy.TakeDamage(td_AOEData.pulseEffectAmount);
        }
        Debug.Log("Collision stay dmg");
        //if (!PulseStillGoing()) EndPulse();
    }
}
