using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
[CustomEditor(typeof(Tester), true)]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //DrawPropertiesExcluding(serializedObject, "m_Script");

        //if (GUILayout.Button("Neural Test"))
        //{
        //    Type type = target.GetType();
        //    type.GetMethod("NeuralTest", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, null);
        //}
        //EditorGUILayout.HelpBox("Use this to test the Neural Net with out launching the app!", MessageType.Info);
    }
}
#endif