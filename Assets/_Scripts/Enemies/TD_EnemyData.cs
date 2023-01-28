using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/WaveEnemyData")]
public class TD_EnemyData : ScriptableObject
{
    public string displayName = "Enemy";
    public float health = 2f;
    public float moveSpeed = 1f;
    public GameObject spawnPrefab;
    public int dmgToCore = 1;
    public int reward = 1;
    public GameObject corpseSpawnPrefab;
}