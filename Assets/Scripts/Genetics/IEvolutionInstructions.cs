public interface IEvolutionInstructions
{
    // Used to read the output data, idk -- Change later??
    EvolutionValueType EvoType { get; }

    // Use to create a custom fitness calculation
    public float EvolutionFitnessFunction(Genome genome);

    // Use to generate custom random values for the genomes
    public EvolutionValue GetEvolutionRandomValue();
}
