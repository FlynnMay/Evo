using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EvoDNA", menuName = "EvoTools/EvoDNA")]
public class DNA<T> : ScriptableObject
{
    protected EvolutionValueType evoType;
    public T[] genes;
}

[CreateAssetMenu(fileName = "EvoFloatDNA", menuName = "EvoTools/EvoFloatDNA")]
public class DNAFloat : DNA<float>
{
    public DNAFloat()
    {
        evoType = EvolutionValueType.EvoFloat;
    }
}