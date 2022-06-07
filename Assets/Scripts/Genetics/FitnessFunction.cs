using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitnessFunction : ScriptableObject
{
    public virtual float GetValue(EvolutionAgent agent)
    {
        return float.PositiveInfinity;
    }
}