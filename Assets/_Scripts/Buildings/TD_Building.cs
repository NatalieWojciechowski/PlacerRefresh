using EasyBuildSystem.Features.Scripts.Core.Base.Piece;
using ModelShark;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class TD_Building : MonoBehaviour, I_TDBulidingSaveCoordinator
{
    [SerializeField] protected TD_BuildingData _baseBuildingData;
    // TODO:
    /*
     * What if the buidings had entities to keep fed/ resourced instead of the money for the towers.
     * - upkeep for them like little residents and tower strength based off their population
     */

    [SerializeField] protected BuildingData _sBuildingData;

    [SerializeField]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip hitSound;

    //protected float _attackRange = 0.75f;
    //private float _baseDamage;
    private TD_Enemy _buildingTarget;

    [SerializeField] protected TargetingType targetingType;
    protected BuildingState buildingState;
    protected BuildingAttackState attackState;

    public Animator bAnimator;
    public GameObject inRangeEffects;

    public bool IsRunning = true;
    public bool IsInRange = false;

    protected float _lastAction = 0f;
    private float effectToggleDelay = .25f;
    private float effectLastToggle = 0;
    private bool isSelected = false;
    protected float myAttackDelay = 1f;

    public int EnemyKillCount { get; internal set; }
    [SerializeField] public Guid BuildingUUID { get; protected set; }
    public TD_Enemy BuildingTarget { get => _buildingTarget; }
    public Transform ProjectileStart;
    public GameObject RangeIndicator;

    public GameObject LevelUI;

    protected enum TargetingType
    {
        Nearest,
        First,
        Last,
        Furthest,
        Strongest,
        Weakest,
    }

    protected enum BuildingState
    {
        Blueprint,
        Idle,
        Attacking,
        OnCooldown,
        Reloading,
        Broken,
        Upgrading
    }

    protected enum BuildingAttackState
    {
        Attacking,
        Ready,
        Reloading,
        Cooldown
    }

    public TD_Building(TD_BuildingData bData)
    {
        SetStats(bData);
        IsRunning = false;
        TryBuildingState(BuildingState.Blueprint);
    }

    protected virtual void BuildingInit(TD_BuildingData sourceBuildingData = null)
    {
        // Any adjustments to make with building data now that we have the base? 
        if (!sourceBuildingData || !_baseBuildingData || !TD_GameManager.instance) return;
        SetStats(sourceBuildingData);
        SetupHelpers();
    }

    public void SetStats(TD_BuildingData bData)
    {
        if (!bData) return;
        _baseBuildingData = bData;
        _sBuildingData = new BuildingData(bData);
        myAttackDelay = _sBuildingData.AdjustedAttackDelay();

        // callbacks / Powerup animations/ etc
        // TODO: fix this weirdness between here and building init
        BuildingInit(null);
    }

    internal BuildingData GetStats()
    {
        // TODO: Any adjustments like upgrades to make here before returning; 
        // Copy the stats to new struct? 
        if (!_sBuildingData.RawBuildingData) BuildingInit(_baseBuildingData);
        return this._sBuildingData;
        //return this._baseBuildingData;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (!ProjectileStart) ProjectileStart = this.transform;
        BuildingUUID = Guid.NewGuid();
        //if (_buildingData == null) _buildingData = GetComponent<TD_BuildingData>();
        if (!bAnimator) bAnimator = GetComponent<Animator>();
        if (IsRunning == false) SetStats(_baseBuildingData);
        //IsRunning = true;
        Debug.Log("Building Data: " + _baseBuildingData);

    }

    protected virtual void OnEnable()
    {
        this.buildingState = BuildingState.Blueprint;
        EventManager.OnTowerSelect += OnTowerSelection;
        EventManager.OnTowerDeselect += OnTowerDeselection;
    }

    protected virtual void OnDisable()
    {
        EventManager.OnTowerSelect -= OnTowerSelection;
        EventManager.OnTowerDeselect -= OnTowerDeselection;
    }


    protected void OnTowerSelection(TD_Building selectedTower)
    {
        if (selectedTower.BuildingUUID == BuildingUUID) Select();
        else if (isSelected) Deselect();
    }

    protected void Select()
    {
        ToggleSelection(true);
    }

    protected void OnTowerDeselection()
    {
        if (isSelected) Deselect();
    }

    protected void Deselect()
    {
        ToggleSelection(false);
    }

    private void ToggleSelection(bool toState)
    {
        //TD_SelectionManager.instance.DeselectCurrent();
        isSelected = toState;
        //if (LevelUI) LevelUI.SetActive(toState);
        //if (RangeIndicator) RangeIndicator.SetActive(toState);
        UpdateHelpers();
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
        if (buildingState == BuildingState.Blueprint) return;
        bool beganWithTarget = (_buildingTarget?.EnemyUUID != null);
        // Validation
        if (IsRunning) FindPlannedTarget();
        if (_buildingTarget)
        {
            // Dont restart effets
            if (!beganWithTarget) EnteredRange();
            ActOnTarget();
        }
        else if (!_buildingTarget && IsInRange) ExitedRange();

        //// Visual like tracking or "warm up" animation
        //if (IsInRange) ToggleEffects(true);
    }

    protected virtual void EnteredRange()
    {
        //Debug.Log("JUST went INTO range");
        ToggleRangeEffects(true);
    }

    protected virtual void ExitedRange()
    {
        //Debug.Log("JUST went out of range");
        ToggleRangeEffects(false);
    }

    private void ToggleRangeEffects(bool shouldShow)
    {
        if (Time.time - effectLastToggle < effectToggleDelay) return;
        //Debug.Log("Toggle Effects: " + shouldShow);
        if (bAnimator) bAnimator.SetBool("InRange", shouldShow);
        if (inRangeEffects) inRangeEffects.SetActive(shouldShow);
        effectLastToggle = Time.time;
    }

    protected virtual void FindPlannedTarget()
    {
        TD_Enemy startingEnemy = _buildingTarget;
        // TODO: Method to grab all enemies from a manager to iterate over rather than searching
        TD_Enemy plannedEnemy = null;
        TD_Enemy[] enemies = FindObjectsOfType<TD_Enemy>();
        float closestDistance = _sBuildingData.AttackRange;
        // Will Get the closest of enemies; break in if block if want ANY enemy 
        foreach (TD_Enemy enemy in enemies)
        {
            float _distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (_distance < closestDistance && enemy.Health > 0) {
                closestDistance = _distance;
                plannedEnemy = enemy;
            }
        }
        if (!plannedEnemy && !_buildingTarget || plannedEnemy?.EnemyUUID == _buildingTarget?.EnemyUUID) return;
        SetTarget(plannedEnemy);

        Physics.queriesHitTriggers = true;
    }

    internal bool TryUpgrade()
    {
        if (!TD_GameManager.instance.CanAfford(_sBuildingData.UpgradeCost) ||
            _sBuildingData.Level >= _sBuildingData.MaxLevel) return false;
        _sBuildingData.LevelUp();
        TD_GameManager.instance.SpendMoney(_sBuildingData.UpgradeCost);
        TryBuildingState(BuildingState.Upgrading);
        // TODO: handle case of building ugprade to another?
        return true;
    }

    internal bool TrySell()
    {
        // May have conditions like "immovable" or corrupted, etc
        if (_sBuildingData.CanSell)
        {
            TD_GameManager.instance.AddCoins(_sBuildingData.SellValue());
            EventManager.instance.TowerDeselected();
            // TODO: remove from BuildManager
            Destroy(this.gameObject);
        }
        return true;
    }

    private void SetTarget(TD_Enemy enemy)
    {
        IsInRange = enemy != null;
        // Losing the target
        if (_buildingTarget && !IsInRange) ExitedRange();
        _buildingTarget = enemy;
        // TODO: any animation / visualization here? 
    }

    /// <summary>
    /// We keep most of our high level logic for the building while it has a target
    /// </summary>
    protected virtual void ActOnTarget()
    {
        transform.LookAt(_buildingTarget.transform.position);
        if (IsInRange && ProjectileReady()) attackState = BuildingAttackState.Ready;

        if (_baseBuildingData.projectilePrefab && attackState == BuildingAttackState.Ready)
        {
            SpawnProjectile();
        }
    }

    // TODO: Clearly define the difference between the reloading and cooldown states
    private bool ProjectileReady()
    {
        return (Time.time - _lastAction > myAttackDelay);
    }

    protected virtual void SpawnProjectile()
    {
        if (ProjectileReady())
        {
            TryBuildingState(BuildingState.Attacking);
            GameObject lastProjectile = Instantiate(_baseBuildingData.projectilePrefab, transform);
            lastProjectile.transform.position = ProjectileStart.position;
            lastProjectile.transform.LookAt(_buildingTarget.transform.position);

            // TODO: have only the rotating part of building move toward enemy

            // TODO: assign owner / target? 
            TD_Projectile td_projectile = lastProjectile.GetComponent<TD_Projectile>();
            td_projectile.InitProjectile(this, _buildingTarget);
            _lastAction = Time.time;
            TryBuildingState(BuildingState.OnCooldown);
        }
    }

    protected virtual void TryBuildingState(BuildingState toState = BuildingState.Idle)
    {
        switch (toState) {
            case BuildingState.Idle:
            bAnimator.SetBool("InRange", false);
            bAnimator.SetBool("IsReloading", false);
            bAnimator.SetBool("IsAttacking", false);
            break;

            case BuildingState.Attacking:
            if (fireSound) TD_AudioManager.instance.PlayClip(fireSound, transform.position);
            bAnimator.SetBool("IsAttacking", true);
            attackState = BuildingAttackState.Attacking;
            break;

            case BuildingState.OnCooldown:
            if (reloadSound) TD_AudioManager.instance.PlayClip(reloadSound, transform.position);
            bAnimator.SetBool("IsReloading", true);
            attackState = BuildingAttackState.Cooldown;
            break;

            case BuildingState.Reloading:
            bAnimator.SetBool("IsReloading", true);
            attackState = BuildingAttackState.Reloading;
            break;

            case BuildingState.Upgrading:
            UpdateHelpers();
            break;

            default:
            bAnimator.SetBool("InRange", false);
            bAnimator.SetBool("IsReloading", false);
            bAnimator.SetBool("IsAttacking", false);
            break;
        };
        // TODO: any sort of validation here? 
        buildingState = toState;
    }

    public void OnPlacementConfirm(Transform tPlacement)
    {
        transform.SetPositionAndRotation(tPlacement.position, tPlacement.rotation);
        TryBuildingState(BuildingState.Idle);
        IsRunning = true;
        RangeIndicator?.SetActive(false);
        SetupLevelIndicators();
        EventManager.instance.MoneySpent(_sBuildingData.PurchaseCost);
        // TODO: Might have an extra call on this ATM
        EventManager.instance.TowerPlaced(this);
    }

    private void SetupLevelIndicators()
    {
        if (!LevelUI) return;
        LevelUI.GetComponent<LevelIndicator>()?.InitIndicator(this);
        // Not activating here because we havent selected yet
    }

    private void SetupHelpers()
    {
        if (RangeIndicator)
        {
            RangeIndicator?.SetActive(true);
        }
        if (LevelUI)
        {
            //SpriteRenderer[] levelPips = LevelIndicator.GetComponentsInChildren<SpriteRenderer>();
            //LevelUI.GetComponent<LevelIndicator>()?.InitIndicator(this);
        }
        if (inRangeEffects)
        {
            // TODO: ?
        }
        UpdateHelpers();
    }

    private void UpdateHelpers()
    {
        if (RangeIndicator)
        {
            float attackRange = _sBuildingData.AttackRange;
            RangeIndicator.GetComponentInChildren<SpriteRenderer>().size = new Vector2(attackRange, attackRange);
            RangeIndicator.SetActive(isSelected);
        }
        if (LevelUI)
        {
            LevelUI.GetComponent<LevelIndicator>().RefreshLevels();
            LevelUI.SetActive(isSelected);
        }
    }

    public Button ConfigureButton(ref Button buttonObj)
    {
        buttonObj.transform.GetChild(0).GetComponent<Image>().sprite = _baseBuildingData.icon;
        buttonObj.transform.GetChild(0).GetComponent<Image>().preserveAspect = true;

        //buttonObj.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = _baseBuildingData.displayName;
        buttonObj.onClick.AddListener(delegate { TryPurchase(_baseBuildingData.PurchaseCost); });

        GameObject tooltip = buttonObj.GetComponentInChildren<TooltipTrigger>()?.gameObject;
        if (tooltip)
        {
            TooltipTrigger tooltipTrigger = tooltip.GetComponent<TooltipTrigger>();
            tooltipTrigger.SetText("DisplayName", _sBuildingData.DisplayName);
            tooltipTrigger.SetText("PurchaseCost", _sBuildingData.PurchaseCost.ToString());
        }

        buttonObj.gameObject.SetActive(true);
        return buttonObj;
    }

    private bool TryPurchase(int spend)
    {
        //int spend = _baseBuildingData.PurchaseCost;
        bool _canAfford = TD_GameManager.instance.CanAfford(spend);
        if (_canAfford)
            TD_BuildManager.StartPlacement(gameObject);
        return _canAfford;
    }

    public void InitFromData(SaveData.TowerSaveData saveData)
    {
        BuildingUUID = saveData.Guid;
        _baseBuildingData = saveData.TD_BuildingData;
        transform.SetPositionAndRotation(saveData.Transform.position, saveData.Transform.rotation);
    }

    public void AddToSaveData(ref SaveData saveData)
    {
        SaveData.TowerSaveData TowerSaveData = new SaveData.TowerSaveData();
        TowerSaveData.Guid = BuildingUUID;
        TowerSaveData.TD_BuildingData = _baseBuildingData;
        TowerSaveData.Transform = transform;
        saveData.constructedBuildings.Add(TowerSaveData);
        Debug.Log("AddToSaveData", this);
    }
}
