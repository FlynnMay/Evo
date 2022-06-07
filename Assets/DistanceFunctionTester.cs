using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFunctionTester : MonoBehaviour
{
    public SampleDistanceFunction distanceFunction;

    void Start()
    {
        distanceFunction.target = transform;
    }
}
