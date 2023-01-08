using UnityEngine;

[CreateAssetMenu(menuName = "MooStudios/WaveEnemyData")]
public class WaveEnemyData : ScriptableObject
{
    public string displayName = "Enemy";
    public float health = 2f;
    public float moveSpeed = 1f;
    public GameObject spawnPrefab;
}