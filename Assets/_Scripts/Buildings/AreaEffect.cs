using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    [SerializeField]
    protected TD_AOEData td_AOEData;
    [SerializeField]
    protected Collider _collider;

    private float maxTime;

    //protected delegate void OnAreaTrigger(Collider hit);
    //protected OnAreaTrigger onAreaTrigger;

    //protected delegate void OnAreaCollide(Collision hit);
    //protected OnAreaCollide onAreaCollide;

    #region AOE Region Activity
    protected bool _isAOEActive = false;
    protected float _aoeStartTime = 0f;
    protected float _aoeActiveDuration = 0f;
    #endregion

    #region Pulse
    protected bool _hasHit = false;
    protected float _lastPulse;
    protected float _pulseDelay;
    protected bool _isPulseActive = false;
    protected float _pulseStartTime = 0f;
    protected float _pulseActiveDuration = 0f;
    private GameObject inRangeEffects;
    private List <TD_Enemy> _interactList;
    #endregion


    /*
     * AOE Region Active  - AOE REGION INACTIVE  -  AOE REGION ACTIVE
     * [Pulse]   [Pulse]                            [Pulse]   [Pulse]
     * 
     */

    // Start is called before the first frame update
    protected virtual void Start()
    {
        InitAOE();
    }

    protected virtual void OnEnable()
    {
        InitAOE();
    }

    private void InitAOE()
    {
        _interactList = new();
        _aoeStartTime = Time.time;
        _collider = GetComponent<Collider>();
        _pulseDelay = td_AOEData.pulseDelay;
        AdjustRange(td_AOEData.aoeRange);
        maxTime = td_AOEData.maxLifetime;
        if (maxTime == 0) maxTime = 10000000f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_isAOEActive)
        {
            if (PulseReady()) { Pulse(); }
            else
            {
                if (PulseStillGoing()) return;
                else EndPulse();
            }
        }
        if (maxTime > 0 && Time.time - _aoeStartTime > maxTime) Destroy(this.gameObject, 0.15f);
    }

    private void LateUpdate()
    {
        if (_hasHit && this.td_AOEData.RemoveOnContact) Destroy(this.gameObject);
    }

    protected void AdjustRange(float _range)
    {
        transform.localScale = new Vector3(_range, _range, _range);
    }

    #region AOE Region
    protected void StartAOE()
    {
        Debug.Log("Start AOE");
        _interactList.Clear();
        _isAOEActive = true;
        _aoeStartTime = Time.time;
    }

    protected void EndAOE()
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
        _interactList.Clear();
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


    protected virtual void OnTriggerEnter(Collider other)
    {
        TD_Enemy hit = other.gameObject.GetComponent<TD_Enemy>();
        if (hit && td_AOEData.effectPrefab)
        {
            // Wait til next pulse to interact but not disable pu;se
            if (!_interactList.Contains(hit))
            {
                hit.GiveShield(td_AOEData);
                _interactList.Add(hit);
                //onAreaTrigger(other);
            }
        }
        else if (other.gameObject.CompareTag("Building"))
        {
            // perhaps debuff towers? or buff? 
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        TD_Enemy hit = collision.gameObject.GetComponent<TD_Enemy>();
        if (hit && td_AOEData.category == "Damage")
        {
            _hasHit = true;
            // TODO: have AOE dmg here instead of via projectile?
            // TODO: perhaps instead of doing pulse, etc -- we spawn the effect here
            // so projectile cleanup does not wipe this out 

            if (!_interactList.Contains(hit))
            {
                hit.TakeDamage(td_AOEData.pulseEffectAmount);
                _interactList.Add(hit);
                //onAreaTrigger(other);
            }
        }
        else if (collision.gameObject.CompareTag("Building"))
        {
            // perhaps debuff towers? or buff? 
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        
    }
    #endregion
}
