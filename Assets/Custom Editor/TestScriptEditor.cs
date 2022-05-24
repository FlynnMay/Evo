using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MonoBehaviour), true)]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, "m_Script");

        if (GUILayout.Button("stuff"))
        {
            Type type = target.GetType();
            type.GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, null);
        }

        EditorGUILayout.HelpBox("Help Box", MessageType.Info);
    }
}
