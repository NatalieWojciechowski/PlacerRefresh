using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/TD_BuildingData")]
public class TD_BuildingData : ScriptableObject
{
    public string displayName = "Turret";
    public float health = 2f;
    public float attackSpeed = 1f;
    public GameObject spawnPrefab;
    // TODO: maybe different upgrade paths etc -- currently unusede
    public TD_BuildingData upgradesTo;
    public Sprite icon;
    public string description;
    public string category;
}