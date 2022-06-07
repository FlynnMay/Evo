using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    public List<Genome> Population { get; set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public object[] BestGenes { get; private set; }

    public float mutationRate;
    Random random;
    float fitnessSum;
    int eliteCount;

    public GeneticAlgorithm(int populationSize, int genomeSize, Random _random, IEvolutionInstructions instructions, float _mutationRate = 0.01f, int _eliteCount = 2)
    {
        Generation = 1;
        mutationRate = _mutationRate;
        Population = new List<Genome>(populationSize);
        random = _random;
        fitnessSum = 0;
        BestFitness = 0;
        BestGenes = new object[populationSize];
        eliteCount = _eliteCount;

        for (int i = 0; i < populationSize; i++)
        {
            Population.Add(new Genome(genomeSize, random, instructions));
        }
    }

    public GeneticAlgorithm(List<Genome> genomes, int genomeSize, Random _random, IEvolutionInstructions instructions, float _mutationRate = 0.01f, int _eliteCount = 2)
    {
        Generation = 1;
        mutationRate = _mutationRate;
        Population = genomes;
        random = _random;
        fitnessSum = 0;
        BestFitness = 0;
        BestGenes = new object[genomeSize];
        eliteCount = _eliteCount;
    }

    public void NewGeneration()
    {
        if (Population.Count <= 0)
            return;

        CalculateFitness();
        Population = Population.OrderBy(x => x.Fitness).ToList();
        Population.Reverse();
        List<Genome> newPopulation = new List<Genome>();

        Population[0].IsKing = true;

        for (int i = 0; i < Population.Count; i++)
        {
            if (i < eliteCount)
            {
                Population[i].IsElite = true;
                newPopulation.Add(Population[i]);
                continue;
            }

            Genome parentA = ChooseParent();
            Genome parentB = ChooseParent();

            Genome child = parentA.Crossover(parentB);
            child.IsElite = false;
            child.Mutate(mutationRate);
            newPopulation.Add(child);
        }

        Population = newPopulation;
        Generation++;
    }

    void CalculateFitness()
    {
        fitnessSum = 0;
        Genome bestGenome = Population[0];

        for (int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].CalaculateFitness(i);

            if (Population[i].Fitness > bestGenome.Fitness)
                bestGenome = Population[i];
        }

        BestFitness = bestGenome.Fitness;
        BestGenes = bestGenome.Genes;
    }

    Genome ChooseParent()
    {
        // Roullete wheel

        if(fitnessSum <= 0)
            return Population[0];

        float[] fitnessProbabillity = Population.Select(g => g.Fitness / fitnessSum).ToArray();
        float[] cumaltiveProbabillity = new float[fitnessProbabillity.Length + 1];

        cumaltiveProbabillity[0] = 0.0f;
        for (int i = 1; i < cumaltiveProbabillity.Length; i++)
            cumaltiveProbabillity[i] = fitnessProbabillity[i - 1] + cumaltiveProbabillity[i - 1];

        float randNum = (float)random.NextDouble();

        for (int i = 0; i < cumaltiveProbabillity.Length; i++)
        {
            if (cumaltiveProbabillity[i] > randNum)
                return Population[i - 1];
        }

        return null;
    }
}

/*
 *  START
 *  Generate the initial population
 *  Compute fitness
 *  REPEAT
 *      Selection
 *      Crossover
 *      Mutation
 *      Compute fitness
 *  UNTIL population has converged
 *  STOP
 */
