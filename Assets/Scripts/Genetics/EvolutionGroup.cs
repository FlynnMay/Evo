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

    public DNAValueGenerator valueGenerator;

    Dictionary<Genome, EvolutionAgent> genomeAgentPair = new Dictionary<Genome, EvolutionAgent>();

    [SerializeField]
    [Tooltip("Determines the chance of a agent to be mutated")]
    float mutationRate = 0.01f;
    [SerializeField]
    [Tooltip("Determines the number of surperior agents which live on to the next generation")]
    int eliteCount = 2;

    
    [SerializeField]
    [Tooltip("Used for adding agents to the heirarchy. \nIt is still possible to add agents manually, but this might make things easier")]
    GameObject agentPrefab;
    
    [SerializeField]
    [Tooltip("Used to allow custom DNA types")]
    DNA agentDNAType;

    //[SerializeField]
    //[Tooltip("If true, will use a default fitness function. \nIt is recommended to use your own custom fitness function!")]
    //bool useDefaultFitnessFunction = false;

    //[SerializeField]
    //[Tooltip("If true, will use a random value generator for the agents genomes. \nIt is recommended to use your own custom value generator!")]
    //bool useDefaultRandomValue = false;
    
    GeneticAlgorithm geneticAlgorithm;

    Random random;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        CustomRandomFunction = () => 
        {
            return valueGenerator.GetType().GetMethod("GetValue").Invoke(valueGenerator, null); 
        };

        random = new Random();

        LoadAgents();

        foreach (EvolutionAgent agent in agents)
        {
            agent.Init(genomeSize, random, agentDNAType, this);
            genomeAgentPair.Add(agent.DNA, agent);
        }

        List<Genome> genomes = agents.Select(a => a.DNA).ToList();

        geneticAlgorithm = new GeneticAlgorithm(genomes, genomeSize, random, this, mutationRate, eliteCount);
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

    public float EvolutionFitnessFunction(Genome genome)
        => Mathf.Clamp01(CustomFitnessFunction(genome));

    public object GetEvolutionRandomValue()
        => CustomRandomFunction();

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

    public float CalculateMutationRate()
    {
        return 1.0f / agents.Length;
    }
    
    public void AssignMutationRateToCalculatedRate()
    {
        mutationRate = CalculateMutationRate();
    }

    public void InstantiateNewAgents(int addAgentCount)
    {
        for (int i = 0; i < addAgentCount; i++)
        {
            Instantiate(agentPrefab, transform);
        }
    }
}
