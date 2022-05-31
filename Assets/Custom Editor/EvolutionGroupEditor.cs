using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EvolutionGroup))]
public class EvolutionGroupEditor : Editor
{
    bool foldout = false;
    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, "agents");
        
        EvolutionGroup evolutionGroup = target as EvolutionGroup;

        GUILayout.Space(5);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("agents"));
        
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        
        if (GUILayout.Button(new GUIContent("Assign Agents", "Loads all agents from the children")))
            evolutionGroup.LoadAgents();
        if (GUILayout.Button(new GUIContent("Clear Agents", "Clears the agents array")))
            evolutionGroup.ClearAgents();
        
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        foldout = EditorGUILayout.Foldout(foldout, new GUIContent("Debug", "Useful Info"), EditorStyles.foldoutHeader);
        if (foldout)
        {
            EditorGUILayout.HelpBox($"Generation: {evolutionGroup.GetGeneration()}\n" +
                $"Best Fitness {evolutionGroup.GetBestFitness()}", MessageType.None);
        }
    }
}
