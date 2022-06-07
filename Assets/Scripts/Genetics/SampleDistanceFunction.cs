using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sample Distance Function", menuName = "EvoTools/Testing/SampleDistanceFunction")]
public class SampleDistanceFunction : FitnessFunction
{
    public Transform target;
    public override float GetValue(EvolutionAgent agent)
    {
        Vector3 pos = agent.transform.position;
        float dist = Mathf.Abs(pos.x - target.position.x) + Mathf.Abs(pos.z - target.position.z);
        return 1.0f / dist;
    }
}

