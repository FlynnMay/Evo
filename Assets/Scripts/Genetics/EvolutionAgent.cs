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

    List<float> penalties = new List<float>();
    List<float> rewards = new List<float>();

    [Tooltip("Overwrites default DNA")]
    public DNA defaultDNA = null;
    public Genome DNA { get; set; }
    public bool IsElite { get { return DNA.IsElite; } }
    public bool IsKing { get { return DNA.IsKing; } }
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

    [HideInInspector] public EvolutionGroup group;
    public int Score { get; private set; }

    public void Init(int size, System.Random random, DNA _DNAType, EvolutionGroup _group)
    {
        group = _group;
        DNA = new Genome(size, random, _group);
        DNAType = _DNAType;

        if (defaultDNA != null)
        {
            Type type = defaultDNA.GetType();
            FieldInfo info = type.GetField("genes");
            object genes = info.GetValue(defaultDNA);
            DNA.Genes = ((IEnumerable)genes).Cast<object>().ToArray();
        }

        //DNA.Genes = importDNA ? importDNA.genes : DNA.Genes;
    }

    public void Reset()
    {
        Score = 0;
        IsAlive = true;
        penalties.Clear();
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

    public void Penalise()
    {
        Score--;
    }

    public void Reward()
    {
        Score++;
    }

    public float CalculateRewardPenalties(float value)
    {
        int threshold = 2;
        //return Score < threshold ? 0 : value;
        //return Score < threshold ? value / (threshold / Score) : value;
        int scoreTemp = Mathf.Clamp(Score, int.MinValue, threshold);
        float num = value * (scoreTemp / threshold);
        return (Mathf.Clamp(num, -1, 1) + 1) / 2;
    }
}