using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EvolutionAgent : MonoBehaviour
{
    public Genome DNA { get; set; }
    public UnityEvent onResetEvent;
    
    [Header("Debug")]
    [ReadOnly]
    [SerializeField] bool isAlive = true;
    public bool IsAlive { get { return isAlive; } set { isAlive = value; } }

    public void Init(int size, System.Random random, IEvolutionInstructions instructions)
    {
        DNA = new Genome(size, random, instructions);
    }

    public void Reset()
    {
        onResetEvent?.Invoke();
    }
}