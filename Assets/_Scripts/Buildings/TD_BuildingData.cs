using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_BuildingData")]
public class TD_BuildingData : ScriptableObject
{
    public string displayName = "Turret";
    public float health = 2f;
    public float attackSpeed = 1f;
    public float attackRange = 0.5f;
    public GameObject spawnPrefab;
    // TODO: maybe different upgrade paths etc -- currently unusede
    public TD_BuildingData upgradesTo;
    public Sprite icon;
    public string description;
    public string category;
    public GameObject projectilePrefab;
    public Vector3 projectileOffset;
    public float projectileDelay = 0.5f;
    public float baseDamage = 1f;
    public float aoeActiveDuration = 0.5f;
}