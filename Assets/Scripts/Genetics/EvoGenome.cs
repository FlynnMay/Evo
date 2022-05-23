using System;
using System.Collections.Generic;

public class Genome
{
    public EvolutionValue[] Genes { get; set; }
    public float Fitness { get; set; }
    public EvolutionValueType EvoType { get; private set; }
    public IEvolutionInstructions Instructions { get; private set; }
    Random random;

    readonly Dictionary<EvolutionValueType, Type> _Types = new Dictionary<EvolutionValueType, Type>
    {
        { EvolutionValueType.EvoInt, typeof(int) },
        { EvolutionValueType.EvoFloat, typeof(float) },
        { EvolutionValueType.EvoChar, typeof(char) }
    };

    public Genome(int size, Random _random, IEvolutionInstructions instructions)
    {
        Instructions = instructions;
        EvoType = Instructions.EvoType;
        Genes = new EvolutionValue[size];
        random = _random;

        for (int i = 0; i < Genes.Length; i++)
        {
            Genes[i] = Instructions.GetEvolutionRandomValue();
        }
    }

    public float CalaculateFitness(int index)
    {
        Fitness = Instructions.EvolutionFitnessFunction(this);
        return Fitness;
    }

    public Genome Crossover(Genome other)
    {
        Genome child = new Genome(Genes.Length, random, Instructions);

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
                Genes[i] = Instructions.GetEvolutionRandomValue();
        }
    }
}