using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithm<T>
{
    public List<Genome<T>> Population { get; private set; }
    public int Generation { get; private set; }
    public float BestFitness { get; private set; }
    public T[] BestGenes { get; private set; }

    public float mutationRate;
    Random random;
    float fitnessSum;
    int eliteCount;

    public GeneticAlgorithm(int populationSize, int genomeSize, Random _random, Func<T> _getRandomGene, Func<int, float> _fitnessFunc, float _mutationRate = 0.01f, int _eliteCount = 2)
    {
        Generation = 1;
        mutationRate = _mutationRate;
        Population = new List<Genome<T>>(populationSize);
        random = _random;
        fitnessSum = 0;
        BestFitness = 0;
        BestGenes = new T[populationSize];
        eliteCount = _eliteCount;

        for (int i = 0; i < populationSize; i++)
        {
            Population.Add(new Genome<T>(genomeSize, random, _getRandomGene, _fitnessFunc));
        }
    }

    public void NewGeneration()
    {
        if(Population.Count <= 0)
            return;

        CalculateFitness();
        Population = Population.OrderBy(x => x.Fitness).ToList();
        List<Genome<T>> newPopulation = new List<Genome<T>>();

        for (int i = 0; i < Population.Count; i++)
        {
            if(i< eliteCount)
            {
                newPopulation.Add(Population[i]);
                continue;
            }

            Genome<T> parentA = ChooseParent();
            Genome<T> parentB = ChooseParent();

            Genome<T> child = parentA.Crossover(parentB);

            child.Mutate(mutationRate);
            newPopulation.Add(child);
        }

        Population = newPopulation;
        Generation++;
    }

    void CalculateFitness()
    {
        fitnessSum = 0;
        Genome<T> bestGenome = Population[0];

        for (int i = 0; i < Population.Count; i++)
        {
            fitnessSum += Population[i].CalaculateFitness(i);

            if(Population[i].Fitness > bestGenome.Fitness)
                bestGenome = Population[i];
        }

        BestFitness = bestGenome.Fitness;
        BestGenes = bestGenome.Genes;
    }

    Genome<T> ChooseParent()
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
