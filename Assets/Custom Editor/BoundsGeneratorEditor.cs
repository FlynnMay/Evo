using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(BoundsGenerator))]
public class BoundsGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("Generate Bounds"))
        {
            BoundsGenerator generator = target as BoundsGenerator;
            typeof(BoundsGenerator).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(generator, null);
        }
    }
}
#endif
