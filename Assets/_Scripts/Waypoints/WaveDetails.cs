using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName ="MooStudios/SpawnWave")]
public class WaveDetails : ScriptableObject
{
    public List<TD_EnemyData> waveContents;
    public float spawnInterval = 10f;
}
