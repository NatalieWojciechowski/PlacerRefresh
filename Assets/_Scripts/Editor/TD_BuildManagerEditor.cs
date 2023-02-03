using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TD_BuildManager))]
public class TD_BuildManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TD_BuildManager myTarget = (TD_BuildManager)target;

        //myTarget.Pieces;

        DrawDefaultInspector();

        //foreach (TD_Building buliding in collection)
        //{

        //}

        //if (GUILayout.Button("RestartWave"))
        //{
        //    TD_GameManager.current.RestartWaves();
        //}
    }
}
