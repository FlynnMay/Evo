using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Empty holder class for DNA<T>
public class DNA : ScriptableObject
{
}

public class DNA<T> : DNA
{
    public T[] genes;

    public void SetGenes(T[] genes)
    {
        this.genes = genes;
    }

    protected void SetGenesFromObject(object[] genes)
    {
        this.genes = new T[genes.Length];
        for (int i = 0; i < genes.Length; i++)
        {
            this.genes[i] = (T)genes[i];
        }
    }
}

