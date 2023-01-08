using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="MooStudios/SpawnWave")]
public class SpawnWave : ScriptableObject
{
    public List<WaveEnemyData> waveContents;
    public float spawnInterval = 10f;
}
