using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    public List<Genome> Population { get; set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public EvolutionValue[] BestGenes { get; private set; }

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
        BestGenes = new EvolutionValue[populationSize];
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
        BestGenes = new EvolutionValue[genomeSize];
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

        for (int i = 0; i < Population.Count; i++)
        {
            if (i < eliteCount)
            {
                newPopulation.Add(Population[i]);
                continue;
            }
            
            Genome parentA = ChooseParent();
            Genome parentB = ChooseParent();

            Genome child = parentA.Crossover(parentB);

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
        double randNum = random.NextDouble() * fitnessSum;

        for (int i = 0; i < Population.Count; i++)
        {
            if (randNum < Population[i].Fitness)
                return Population[i];

            randNum -= Population[i].Fitness;
        }

        return Population.Last();
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
