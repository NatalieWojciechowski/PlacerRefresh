using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_BuildingData")]
[Serializable]
public class TD_BuildingData : ScriptableObject
{
    #region Display
    public string displayName = "Turret";
    [SerializeReference] public Sprite icon;
    public string description;
    public string category;
    #endregion

    #region Behavior
    public float health = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 0.5f;
    [SerializeReference] public GameObject buildingPrefab;
    [SerializeReference] public GameObject projectilePrefab;
    public float projectileDelay = 0.5f;
    public float baseDamage = 1f;
    public float aoeActiveDuration = 0.5f;
    public int CurrentTier = 1;
    public int MaxTier = 3;
    // TODO: Upgrade struct w/ costs per level & prefabs, etc.
    public int PurchaseCost = 12;
    #endregion

    #region Interactions
    // TODO: maybe different upgrade paths etc -- currently unusede
    [SerializeReference] public TD_BuildingData upgradesTo;
    public bool canSell = true;
    #endregion
}

/// <summary>
///  Represents the modified values of a particular tower. Damage or current level for an individual tower will be managed by this. 
/// </summary>
[Serializable]
public struct BuildingData
{

    private TD_BuildingData _BuildingData;
    private int _currentLevel;
    public BuildingData(TD_BuildingData baseData)
    {
        _BuildingData = ScriptableObject.Instantiate(baseData);
        _currentLevel = baseData.CurrentTier;
        DisplayName = baseData.displayName;
        MaxLevel = baseData.MaxTier;
        Damage = baseData.baseDamage;
        AttackRange = baseData.attackRange;
        AttackSpeed = baseData.attackSpeed;
        UpgradesTo = baseData.upgradesTo;
        CanSell = baseData.canSell;
        PurchaseCost = baseData.PurchaseCost;
        UpgradeCost = baseData.PurchaseCost / 2;
        if (baseData.upgradesTo) UpgradeCost = baseData.upgradesTo.PurchaseCost - this.PurchaseCost;
    }

    public TD_BuildingData RawBuildingData { get => _BuildingData; }
    public string DisplayName { get; }
    public int Level { get => _currentLevel; set => setLevel(value); }

    public int MaxLevel { get; }
    public float AttackRange { get; private set; }
    public float AttackSpeed { get; }
    public float Damage { get; set; }
    public bool CanSell { get; set; }
    public TD_BuildingData UpgradesTo { get; set; }
    public int PurchaseCost { get; }
    public int UpgradeCost { get; }

    /// <summary>
    /// Returns the value after max level taken into account
    /// </summary>
    public int LevelUp()
    {
        _currentLevel += 1;
        return EnforceLevelBounds();
    }

    private int EnforceLevelBounds()
    {
        if (_currentLevel > MaxLevel) _currentLevel = MaxLevel;
        else
        {
            //// Adjust Stats
            float lvlScale = _currentLevel * 0.25f;
            AttackRange += (float)Math.Round(_BuildingData.attackRange * (lvlScale / 5));
            Damage += (float)Math.Min(Math.Round(_BuildingData.baseDamage * (lvlScale)), 1);
        }
        return _currentLevel;
    }

    private void setLevel(int toLevel)
    {
        _currentLevel = toLevel;
        EnforceLevelBounds();
    }

    internal int SellValue()
    {
        // TODO: any modifiers to sell cost such as any upgrades / tiers?
        return Mathf.RoundToInt(this.PurchaseCost / 2);
    }

    public float AdjustedAttackDelay()
    {
        // Ex: 0.25 / 1 => .5
        return _BuildingData.projectileDelay / AttackSpeed;
    }
}