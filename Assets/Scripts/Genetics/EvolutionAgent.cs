using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[Serializable]
public class EvolutionAgent : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent onResetEvent;

    [Header("Debug")]
    [ReadOnly] [SerializeField] bool isAlive = true;
    [ReadOnly] [SerializeField] DNA DNAType;

    //List<float> penalties = new List<float>();
    //List<float> rewards = new List<float>();

    public DNA importDNA = null;
    public Genome DNA { get; set; }
    public bool IsElite { get { return DNA.IsElite; } }
    public bool IsKing { get { return DNA.IsKing; } }
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

    public void Init(int size, System.Random random, DNA _DNAType, IEvolutionInstructions instructions)
    {
        DNA = new Genome(size, random, instructions);
        DNAType = _DNAType;

        if (importDNA != null)
        {
            Type type = importDNA.GetType();
            FieldInfo info = type.GetField("genes");
            object genes = info.GetValue(importDNA);
            DNA.Genes = ((IEnumerable)genes).Cast<object>().ToArray();
        }

        //DNA.Genes = importDNA ? importDNA.genes : DNA.Genes;
    }

    public void Reset()
    {
        onResetEvent?.Invoke();
    }

    public void ExportDNA()
    {
        Type type = DNAType.GetType();
        ScriptableObject exportObject = ScriptableObject.CreateInstance(type);

        type.GetMethod("SetGenesFromObject", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(exportObject, new object[] { DNA.Genes });

        string path = $"Assets/Agents/{name}.asset";
        AssetDatabase.CreateAsset(exportObject, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = exportObject;
    }
}