using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_BuildingData")]
public class TD_BuildingData : ScriptableObject
{
    #region Display
    public string displayName = "Turret";
    public GameObject spawnPrefab;
    public Sprite icon;
    public string description;
    public string category;
    #endregion

    #region Behavior
    public float health = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 0.5f;
    public GameObject projectilePrefab;
    public Vector3 projectileOffset;
    public float projectileDelay = 0.5f;
    public float baseDamage = 1f;
    public float aoeActiveDuration = 0.5f;
    #endregion

    #region Interactions
    // TODO: maybe different upgrade paths etc -- currently unusede
    public TD_BuildingData upgradesTo;
    public bool canSell = true;
    #endregion
}