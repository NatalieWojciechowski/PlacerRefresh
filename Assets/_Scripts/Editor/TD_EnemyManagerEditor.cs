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

        if (GUILayout.Button("RestartWave"))
        {
            TD_GameManager.current.RestartWaves();
        }
    }
}

