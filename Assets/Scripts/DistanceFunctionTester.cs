using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

public class DistanceFunctionTester : MonoBehaviour
{
    public ManhattanDistanceFitnessFunction distanceFunction;

    void Start()
    {
        distanceFunction.target = transform;
    }
}
