using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EvolutionAgent))]
public class EvolutionAgentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EvolutionAgent agent = target as EvolutionAgent;

        if (GUILayout.Button(new GUIContent("Export DNA", "Allows you to save the DNA of an agent")))
        {
            agent.ExportDNA();
            AssetDatabase.Refresh();
        }
    }
}
