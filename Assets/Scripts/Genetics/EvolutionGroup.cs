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

    [Tooltip("Used to generate custom random values for genetic algorithm")]
    public DNAValueGenerator valueGenerator;

    [Tooltip("Used to calclulate fitness for genetic algorithm")]
    public FitnessFunction fitnessFunction;

    [SerializeField]
    [Tooltip("Determines the chance of a agent to be mutated")]
    float mutationRate = 0.01f;

    [SerializeField]
    [Tooltip("Determines the number of surperior agents which live on to the next generation")]
    int eliteCount = 3;

    [SerializeField]
    [Tooltip("Used for adding agents to the heirarchy. \nIt is still possible to add agents manually, but this might make things easier")]
    GameObject agentPrefab;

    [SerializeField]
    [Tooltip("Used to allow custom DNA types")]
    DNA agentDNAType;

    [SerializeField] bool useTimer = true;
    [Range(0f, 100f)] [SerializeField] float timeScale = 1.0f;
    public float timerMax = 30.0f;
    [ReadOnly] [SerializeField] float timer = 0.0f;

    EvoGeneticAlgorithm geneticAlgorithm;
    Dictionary<Genome, EvolutionAgent> genomeAgentPair = new Dictionary<Genome, EvolutionAgent>();
    Random random;

    IEnumerator Start()
    {
        timer = timerMax;

        if (GeneticTime.instance == null)
            gameObject.AddComponent<GeneticTime>();

        yield return new WaitForEndOfFrame();

        random = new Random();

        LoadAgents();

        foreach (EvolutionAgent agent in agents)
        {
            agent.Init(genomeSize, random, agentDNAType, this);
            genomeAgentPair.Add(agent.DNA, agent);
        }

        List<Genome> genomes = agents.Select(a => a.DNA).ToList();

        geneticAlgorithm = new EvoGeneticAlgorithm(genomes, genomeSize, random, this, mutationRate, eliteCount);
    }

    void Update()
    {
        GeneticTime.timeScale = timeScale;
        timer -= GeneticTime.deltaTime;

        if (agents.Length <= 0)
            return;

        Debug.Log($"There are \"{agents.Where(a => a.IsElite).Count()}\" elites");

        if (agents.All(a => !a.IsAlive) || (timer <= 0.0f && useTimer))
        {
            timer = timerMax;
            EvolveGeneration();

            foreach (var agent in agents)
                agent.Reset();
        }
    }

    public void EvolveGeneration()
    {
        if (geneticAlgorithm == null)
            return;

        geneticAlgorithm.NewGeneration();
    }

    public float EvolutionFitnessFunction(Genome genome)
    {
        EvolutionAgent agent = GetAgentFromDNA(genome);

        float value = (float)fitnessFunction.GetType()
            .GetMethod("GetValue")
            .Invoke(fitnessFunction, new object[] { agent });

        value = agent.CalculateRewardPenalties(value);

        return value;
    }

    public object GetEvolutionRandomValue()
    {
        return valueGenerator.GetType().GetMethod("GetValue").Invoke(valueGenerator, null);
    }

    public EvolutionAgent GetAgentFromDNA(Genome dna)
    {
        return dna.agent;
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
