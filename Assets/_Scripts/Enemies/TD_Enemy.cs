using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class TD_Enemy : MonoBehaviour, I_TDEnemySaveCoordinator
{
    [SerializeField]
    public WaypointRoute fullRoute;
    protected Transform prevWaypoint;
    [SerializeField]
    protected Transform nextWaypoint;
    public int DmgToCore = 1;
    private string _displayName;
    [SerializeField]
    private float _moveSpeed = 1f;
    private float _currentHealth;
    private float _maxHealth = 1f;
    private int _deathReward;
    private Rigidbody _rigidbody;
    [SerializeField]
    private Animator _animator;

    public int currentWaypointIndx = -1;
    [SerializeField]
    private float _turnSpeed = 0.5f;
    private float _lastWaypointSwitchTime;

    public GameObject HealthBarPrefab;
    public GameObject HealthBar;

    public GameObject DeathEffects;
    private GameObject CorpseSpawnPrefab;

    protected Coroutine stateTransition;

    private Vector3 deathPosition;

    public Guid EnemyUUID { get; private set; }
    public float Health { get => _currentHealth; }

    protected enum EnemyState
    {
        Spawn,
        Idle,
        Move,
        SelfDestructing,
        Damaged,
        Die
    }
    [SerializeField]
    protected EnemyState enemyState;
    private bool shielded;
    private GameObject shieldObj;

    protected virtual private void Awake()
    {
        EnemyUUID = Guid.NewGuid();
    }

    public void AnimateState()
    {
        TryChangeState(enemyState);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        enemyState = EnemyState.Spawn;
        // TODO: With multi spawners, need find closest
        if (fullRoute == null) fullRoute = TD_EnemyManager.instance.WaypointRoute;
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (!prevWaypoint) prevWaypoint = this.transform;

        if (!HealthBar) HealthBar = Instantiate(HealthBarPrefab, TD_UIManager.instance.HealthBarContainer.transform);
        HealthBar.transform.localScale = Vector3.one;

        InitEnemy();
    }

    private void InitEnemy()
    {
        UpdateNextWaypoint(false);
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (enemyState == EnemyState.SelfDestructing || enemyState == EnemyState.Die) return;
        if (ShouldDie()) TryChangeState(EnemyState.SelfDestructing);
        if (nextWaypoint) MoveToWaypoint();
        HealthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(-30, -20, 0);
    }

    private bool ShouldDie()
    {
        // TODO: Check invulnerable / undying? 
        return _currentHealth <= 0;
    }

    private void Expire()
    {

        // TODO: Probability/ flag on drops?;
        if (CorpseSpawnPrefab)
        {
            GameObject drop = Instantiate(CorpseSpawnPrefab, TD_GameManager.instance.EffectsBin?.transform);
            drop.transform.position = transform.position;
        }
        DeathEffects?.SetActive(true);
        // In case enemy got to end vs being destroyed
        if (_currentHealth <= 0) TD_GameManager.instance.AddCoins(_deathReward);

        if (shieldObj) Destroy(shieldObj);
        Destroy(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
        Destroy(HealthBar);
    }

    public void SetPreviousWaypoint(Transform tLocation)
    {
        prevWaypoint = tLocation;
    }

    internal void TakeDamage(float projectileDamage)
    {
        if (_currentHealth <= 0) return; // Dont interrupt our die sequence
        if (shielded) RemoveShield();
        else
        {
            TryChangeState(EnemyState.Damaged);
            _currentHealth -= projectileDamage;
            // Make sure the bar does not distort for overdamage
            if (_currentHealth < 0) _currentHealth = -.025f;
            HealthBar.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(_currentHealth / _maxHealth, 1, 1);

            // Setup transition back to moving
            SafeTransition(EnemyState.Move, 0.5f);
        }
    }

    private void RemoveShield()
    {
        if (!shielded || !shieldObj) return;
        shielded = false;
        shieldObj.SetActive(false);
        // TODO: Animation?
    }

    protected void SafeTransition(EnemyState nextState, float delay)
    {
        // Setup transition back to moving
        if (stateTransition != null) { StopCoroutine(stateTransition); stateTransition = null; }
        stateTransition = StartCoroutine(StateTransition(nextState, delay));
    }

    protected IEnumerator StateTransition(EnemyState nextState, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Now allow state change");
        TryChangeState(nextState);
    }



    private void MoveToWaypoint()
    {
        if (ReachedPoint())
        {
            //_animator.SetBool("IsMoving", false);
            UpdateNextWaypoint();
        }
        if (!nextWaypoint) return;
        transform.position = GetLerpPosition();
        transform.LookAt(nextWaypoint.position);
    }

    private Vector3 GetLerpPosition()
    {
        float pathLength = Vector3.Distance(prevWaypoint.position, nextWaypoint.position);
        float totalTimeForPath = pathLength / _moveSpeed;
        float currentTimeOnPath = Time.time - _lastWaypointSwitchTime;
        return Vector3.Lerp(prevWaypoint.position, nextWaypoint.position, currentTimeOnPath / totalTimeForPath);
    }

    internal void SetStats(TD_EnemyData waveEnemyData, WaypointRoute forRoute)
    {
        fullRoute = forRoute;
        _displayName = waveEnemyData.displayName;
        _moveSpeed = waveEnemyData.moveSpeed;
        _maxHealth = waveEnemyData.health;
        _deathReward = waveEnemyData.reward;
        DmgToCore = waveEnemyData.dmgToCore;
        CorpseSpawnPrefab = waveEnemyData.corpseSpawnPrefab;
        TryChangeState(EnemyState.Idle);
    }

    public void GiveShield(TD_AOEData _AOEData) 
    {
        if (shielded) return;
        GameObject shieldPrefab = _AOEData.effectPrefab;
        // TOODO: replace with shield or prevent?
        // we may wish to shield with different effects 
        if (shieldObj != null && shieldObj?.name != shieldPrefab.name) Destroy(shieldObj);
        shieldObj = Instantiate<GameObject>(shieldPrefab, transform);
        if (shieldObj)
        {
            //shieldObj.transform.localScale = shieldObj.transform.localScale * transform.localScale.magnitude;
            shielded = true;
            shieldObj?.SetActive(true);
        }
    }

    private bool ReachedPoint()
    {
        if (Vector3.Distance(transform.position, nextWaypoint.position) < 0.1f)
        {
            // Stop animations? 
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.MovePosition(nextWaypoint.position);
            return true;
        }
        return false;
    }

    private void UpdateNextWaypoint(bool setPrevious = true)
    {
        if (!fullRoute) return;
        Transform nextLocation = fullRoute.NextWaypoint(currentWaypointIndx);
        // If not at the end
        if (nextLocation != null)
        {
            TryChangeState(EnemyState.Move);
            if (setPrevious) prevWaypoint = nextWaypoint;
            nextWaypoint = nextLocation;
            currentWaypointIndx++;
            _lastWaypointSwitchTime = Time.time;
            //_animator.SetBool("IsMoving", true);
        }
        else OnReachEnd();
    }

    public void OnReachEnd()
    {
        // Play animation? 
        EventManager.EnemyPassedCore(DmgToCore);
        TryChangeState(EnemyState.SelfDestructing);
    }

    private void TryChangeState(EnemyState toState = EnemyState.Idle)
    {
        _animator.SetInteger("animation", (int)toState);
        enemyState = toState;

        switch(toState)
        {
            case EnemyState.Die:
            Expire();
            break;
            case EnemyState.SelfDestructing:
            this.GetComponent<Collider>().enabled = false;
            DeathEffects?.SetActive(true);
            SafeTransition(EnemyState.Die, 0.5f);
            break;
        }
    }

    public void InitFromData(SaveData.EnemySaveData enemySaveData)
    {
        EnemyUUID = enemySaveData.Guid;
        nextWaypoint.SetPositionAndRotation(enemySaveData.NextWaypoint, Quaternion.identity);
        transform.SetPositionAndRotation(enemySaveData.position, Quaternion.identity);
    }

    public void AddToSaveData(ref SaveData saveData)
    {
        SaveData.EnemySaveData EnemySaveData = new SaveData.EnemySaveData();
        EnemySaveData.Guid = EnemyUUID;
        EnemySaveData.Health = _currentHealth;
        EnemySaveData.position = transform.position;
        EnemySaveData.NextWaypoint = nextWaypoint.position;
        saveData.currentEnemies.Add(EnemySaveData);
    }
}
