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

        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Save DNA", "Save the DNA of an agent")))
            agent.ExportDNA();


    }
}
