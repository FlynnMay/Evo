using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Float01", menuName = "EvoTools/ValueGenerators/Float01")]
public class DNAFloatGenerator : DNAValueGenerator<float>
{
    public override float GetValue()
    {
        return Random.Range(-1.0f, 1.0f);
    }
}
