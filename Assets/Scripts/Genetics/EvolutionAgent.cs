using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EvolutionAgent : MonoBehaviour
{
    public UnityEvent onResetEvent;
    [Header("Debug")]
    [ReadOnly] [SerializeField] bool isAlive = true;

    //List<float> penalties = new List<float>();
    //List<float> rewards = new List<float>();

    public DNA<float> importDNA;
    public Genome DNA { get; set; }
    public bool IsElite { get { return DNA.IsElite; } }
    public bool IsKing { get { return DNA.IsKing; } }
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

    public void Init(int size, System.Random random, IEvolutionInstructions instructions)
    {
        DNA = new Genome(size, random, instructions);
        //DNA.Genes = importDNA ? importDNA.genes : DNA.Genes;
    }

    public void Reset()
    {
        onResetEvent?.Invoke();
    }

    public void ExportDNA()
    {
        switch (DNA.EvoType)
        {
            case EvolutionValueType.EvoFloat:
                ExportDNA<float>();
                break;
            case EvolutionValueType.EvoInt:
                ExportDNA<int>();
                break;
            case EvolutionValueType.EvoBool:
                ExportDNA<bool>();
                break;
            //case EvolutionValueType.EvoChar:
            //    ExportDNA<char>();
            //    break;
            //case EvolutionValueType.EvoByte:
            //    ExportDNA<byte>();
            //    break;
            //case EvolutionValueType.EvoDecimal:
            //    ExportDNA<decimal>();
            //    break;
            //case EvolutionValueType.EvoDouble:
            //    ExportDNA<double>();
            //    break;
            default:
                break;
        }       
    }

    private void ExportDNA<T>()
    {
        if (DNA.Genes.Any(g => !(g.value is T)))
            throw new Exception("Evolution Type is not the same as the value");

        string jsonString = DNA.Genes.Select(g => (T)g.value).ToArray().ToJson(true);
        jsonString += JsonUtility.ToJson(DNA.EvoType);
        string path = Application.dataPath + $"/Agents/{name}.json";
        StreamWriter writer = new StreamWriter(path);
        writer.Write(jsonString);
        writer.Close();
    }
}