using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;
public class EvolutionGroup : MonoBehaviour, IEvolutionInstructions
{
    public int genomeSize = 1;
    public EvolutionAgent[] agents;

    // Used to apply custom calculations to the fitness result, the returned value will be clamped between 0 and 1
    public Func<Genome, float> CustomFitnessFunction { get; set; }

    // Used to generate custom random values for genetic algorithm
    public Func<object> CustomRandomFunction { get; set; }

    Dictionary<Genome, EvolutionAgent> genomeAgentPair = new Dictionary<Genome, EvolutionAgent>();

    [SerializeField]
    [Tooltip("Determines the chance of a agent to be mutated")]
    float mutationRate = 0.01f;
    [SerializeField]
    [Tooltip("Determines the number of surperior agents which live on to the next generation")]
    int eliteCount = 2;
    [SerializeField]
    [Tooltip("If true, the group will start evolving on Start, and stop once it has found the best soloution")]
    bool evolveUntilResolved = false;

    [SerializeField]
    EvolutionValueType evolutionType;

    GeneticAlgorithm geneticAlgorithm;

    Random random;
    public EvolutionValueType EvoType { get { return evolutionType; } set { evolutionType = value; } }

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        random = new Random();
        LoadAgents();
        
        Array.ForEach(agents, a =>
        {
            a.Init(genomeSize, random, this);
            genomeAgentPair.Add(a.DNA, a);
        });

        List<Genome> genomes = agents.Select(a => a.DNA).ToList();

        geneticAlgorithm = new GeneticAlgorithm(genomes, genomeSize, random, this, mutationRate, eliteCount);

        if (evolveUntilResolved)
            StartCoroutine(Evolve());
    }

    public void EvolveGeneration()
    {
        if (geneticAlgorithm == null)
            return;

        geneticAlgorithm.NewGeneration();

        genomeAgentPair.Clear();
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].DNA = geneticAlgorithm.Population[i];
            genomeAgentPair.Add(geneticAlgorithm.Population[i], agents[i]);
        }
    }

    public IEnumerator Evolve()
    {
        while (geneticAlgorithm.BestFitness < 1)
        {
            EvolveGeneration();
            yield return null;
        }

        string genes = "";
        int total = 0;
        for (int i = 0; i < geneticAlgorithm.BestGenes.Length; i++)
        {
            int gene = geneticAlgorithm.BestGenes[i].GetEvolutionValue<int>();
            genes += gene + ", ";
            total += gene;
        }
        Debug.Log($"Generation {geneticAlgorithm.Generation}, Genes {genes} Total {total}");
    }

    public float EvolutionFitnessFunction(Genome genome)
        => Mathf.Clamp01(CustomFitnessFunction(genome));

    public EvolutionValue GetEvolutionRandomValue()
        => new EvolutionValue(CustomRandomFunction(), EvoType);

    public EvolutionAgent GetAgentFromDNA(Genome dna)
    {
        return genomeAgentPair.ContainsKey(dna) ? genomeAgentPair[dna] : null;
    }

    public void LoadAgents()
    {
        agents = GetComponentsInChildren<EvolutionAgent>();
    }
    
    public void ClearAgents()
    {
        agents = new EvolutionAgent[0];
    }

    public int GetGeneration()
    {
        return geneticAlgorithm != null ? geneticAlgorithm.Generation : 0;
    }

    public float GetBestFitness()
    {
        return geneticAlgorithm != null ? geneticAlgorithm.BestFitness : 0;
    }
}
