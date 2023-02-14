using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TD_Enemy))]
public class LevelScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TD_Enemy myTarget = (TD_Enemy)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Send State To Animator"))
        {
            myTarget.AnimateState();
        }
    }
}

