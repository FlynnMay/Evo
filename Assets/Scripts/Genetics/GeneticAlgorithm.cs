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


public class Genome<T>
{
    public T[] Genes { get; set; }
    public float Fitness { get; set; }
    Func<T> getRandomGene;
    Func<int, float> fitnessFunc;
    Random random;

    public Genome(int size, Random _random, Func<T> _getRandomGene, Func<int, float> _fitnessFunc)
    {
        Genes = new T[size];
        Fitness = 0;
        getRandomGene = _getRandomGene;
        fitnessFunc = _fitnessFunc;
        random = _random;

        for (int i = 0; i < Genes.Length; i++)
            Genes[i] = getRandomGene();
    }

    public float CalaculateFitness(int index)
    {
        Fitness =  fitnessFunc(index);
        return Fitness;
    }

    public Genome<T> Crossover(Genome<T> other)
    {
        Genome<T> child = new Genome<T>(Genes.Length, random, getRandomGene, fitnessFunc);

        // Determine how genes should be copied to the child through the parents
        for (int i = 0; i < Genes.Length; i++)
        {
            child.Genes[i] = random.NextDouble() > 0.5 ? Genes[i] : other.Genes[i];
        }

        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < Genes.Length; i++)
        {
            if (random.NextDouble() < mutationRate)
                Genes[i] = getRandomGene();
        }
    }
}