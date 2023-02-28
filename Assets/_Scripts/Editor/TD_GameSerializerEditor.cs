using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TD_GameSerializer))]
public class TD_GameSerializerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TD_GameSerializer myTarget = (TD_GameSerializer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reset Data"))
        {
            TD_GameSerializer.ResetData();
        }

        if (GUILayout.Button("Save Data"))
        {
            TD_GameSerializer.SaveGame();
        }

        if (GUILayout.Button("Load Game"))
        {
            TD_GameSerializer.LoadGame();
        }
    }
}
