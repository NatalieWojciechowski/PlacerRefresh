using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TD_EnemyManager))]
public class TD_EnemyManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TD_EnemyManager myTarget = (TD_EnemyManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Restart Wave"))
        {
            TD_GameManager.instance.RestartWaves();
        }

        if (GUILayout.Button("Start Wave"))
        {
            EventManager.instance.WaveStarted(TD_GameManager.instance.CurrentWaveIndex);
            //TD_GameManager.instance.PlayerStart();
        }

        if (GUILayout.Button("Wipe Wave"))
        {
            TD_GameManager.instance.WipeWave();
        }
    }
}

