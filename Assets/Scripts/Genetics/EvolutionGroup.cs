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

    [SerializeField]
    [Tooltip("Determines the chance of a agent to be mutated")]
    float mutationRate = 0.01f;
    [SerializeField]
    [Tooltip("Determines the number of surperior agents which live on to the next generation")]
    int eliteCount = 2;

    [SerializeField]
    EvolutionValueType evolutionType;

    GeneticAlgorithm geneticAlgorithm;

    Random random;
    public EvolutionValueType EvoType { get { return evolutionType; } set { evolutionType = value; } }

    void Start()
    {
        random = new Random();
        agents = agents.Length <= 0 ? transform.GetComponentsInChildren<EvolutionAgent>() : agents;
        CustomFitnessFunction = (genome) =>
        {
            int target = 100;
            int[] genes = genome.Genes.Select(g => g.GetEvolutionValue<int>()).ToArray();
            float sum = genes.Sum();
            float error = Mathf.Abs(sum - target);

            return 1 - error / target;
        };

        CustomRandomFunction = () => random.Next(0, 100);
        Array.ForEach(agents, a => a.Init(genomeSize, random, this));

        List<Genome> genomes = agents.Select(a => a.DNA).ToList();

        geneticAlgorithm = new GeneticAlgorithm(genomes, genomeSize, random, this, mutationRate, eliteCount);

        StartCoroutine(Evolve());
    }

    public void EvolveGeneration()
    {
        geneticAlgorithm.NewGeneration();

        for (int i = 0; i < agents.Length; i++)
            agents[i].DNA = geneticAlgorithm.Population[i];
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
}
