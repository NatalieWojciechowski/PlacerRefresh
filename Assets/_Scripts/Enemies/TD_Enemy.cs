using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

public class TD_Enemy : MonoBehaviour
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

    public Guid EnemyUUID { get; private set; }

    enum EnemyState
    {
        Spawn,
        Idle,
        Move,
        Attack,
        Damage,
        Die
    }
    EnemyState enemyState;

    private void Awake()
    {
        EnemyUUID = Guid.NewGuid();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyState = EnemyState.Spawn;
        // TODO: With multi spawners, need find closest
        if (fullRoute == null) fullRoute = FindObjectOfType<TD_EnemyManager>().WaypointRoute;
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (!prevWaypoint) prevWaypoint = this.transform;

        if (!HealthBar) HealthBar = Instantiate(HealthBarPrefab, TD_UIManager.current.transform);
        HealthBar.transform.localScale = Vector3.one;

        InitEnemy();
    }

    private void InitEnemy()
    {
        UpdateNextWaypoint(false);
        _currentHealth = _maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldDie()) Expire();
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
        // TODO: Play animation?
        DeathEffects?.SetActive(true);
        TD_GameManager.current.AddCoins(_deathReward);

        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        Destroy(HealthBar);
    }

    public void SetPreviousWaypoint(Transform tLocation)
    {
        prevWaypoint = tLocation;
    }

    internal void TakeDamage(float projectileDamage)
    {
        TryChangeState(EnemyState.Damage);
        _animator.SetInteger("animation", (int)enemyState);
        _currentHealth -= projectileDamage;
        // TODO: Update floating bar?
        HealthBar.GetComponentsInChildren<Image>()[1].transform.localScale = new Vector3(_currentHealth / _maxHealth, 1, 1);
    }

    private void AllowStateTime(EnemyState nextState)
    {
        //float clipLength = _animator.GetCurrentAnimatorClipInfo(0)[(int)nextState].clip.length;
        //Invoke(, clipLength);
    }

    private void MoveToWaypoint()
    {
        if (ReachedPoint())
        {
            _animator.SetBool("IsMoving", false);
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
        TryChangeState(EnemyState.Idle);
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
            if (setPrevious) prevWaypoint = nextWaypoint;
            nextWaypoint = nextLocation;
            currentWaypointIndx++;
            _lastWaypointSwitchTime = Time.time;
            _animator.SetBool("IsMoving", true);
        }
        else OnReachEnd();

        //nextWaypoint = WaypointManager.NextWaypoint(fullRoute, currentWaypointIndx);
        //if (nextWaypoint != null) currentWaypointIndx++;
        //nextWaypoint = WaypointManager.NextWaypoint(fullRoute, transform);
    }

    private void OnReachEnd()
    {
        // Damage Total health
        // Destroy self 
        //Destroy(this.gameObject);
        
        // This will get handled by the endpoint 
    }

    private void TryChangeState(EnemyState toState = EnemyState.Idle)
    {
        //switch (toState)
        //{
        //    case EnemyState.Idle:

        //    break;
        //    case EnemyState.Move:
        //    bAnimator.SetBool("IsAttacking", true);
        //    attackState = BuildingAttackState.Attacking;
        //    break;
        //    case EnemyState.Attack:
        //    bAnimator.SetBool("IsReloading", true);
        //    attackState = BuildingAttackState.Reloading;
        //    break;
        //    case EnemyState.Damage:
        //    bAnimator.SetBool("IsReloading", true);
        //    attackState = BuildingAttackState.Cooldown;
        //    break;
        //    case EnemyState.Die:
        //    bAnimator.SetBool("IsReloading", true);
        //    attackState = BuildingAttackState.Reloading;
        //    break;
        //    default:
        //    bAnimator.SetBool("InRange", false);
        //    bAnimator.SetBool("IsReloading", false);
        //    bAnimator.SetBool("IsAttacking", false);
        //    break;
        //};
        _animator.SetInteger("animation", (int)toState);
        // TODO: any sort of validation here? 
        enemyState = toState;
    }

}
