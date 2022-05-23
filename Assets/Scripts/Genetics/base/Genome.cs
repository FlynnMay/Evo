using System;

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