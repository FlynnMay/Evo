using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionAgent : MonoBehaviour
{
    public Genome DNA { get; set; }

    public void Init(int size, System.Random random, IEvolutionInstructions instructions)
    {
        DNA = new Genome(size, random, instructions);
    }
}