using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Neural;

public class Tester : MonoBehaviour, IEvolutionInstructions
{
    System.Random random;
    GeneticAlgorithm geneticAlgorithm;
    int target = 100;
    public EvolutionValueType EvoType { get; set; } = EvolutionValueType.EvoInt;

    void Start()
    {
        //NeuralTest();
        random = new System.Random();
        geneticAlgorithm = new GeneticAlgorithm(1000, 3, random, this);

        while (geneticAlgorithm.BestFitness < 1)
        {
            geneticAlgorithm.NewGeneration();
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
    {
        int[] genes = genome.Genes.Select(g => g.GetEvolutionValue<int>()).ToArray();
        float sum = genes.Sum();
        float error = Mathf.Abs(sum - target);
        
        return 1 - error / target;
    }

    public EvolutionValue GetEvolutionRandomValue()
    {
        return new EvolutionValue(random.Next(0, 100), EvolutionValueType.EvoInt);
    }

    private static void NeuralTest()
    {
        NooralNet neuralNetwork = new NooralNet(1, new int[] { 10 }, 1);

        Debug.Log($"Layers: {neuralNetwork.layers.Length}");
        //neuralNetwork.FeedForward(new float[] { 0, 0, 0 });

        for (int i = 0; i < 1000; i++)
        {
            neuralNetwork.FeedForward(new float[] { 0 });
            neuralNetwork.BackPropagate(new float[] { 1 });
            neuralNetwork.FeedForward(new float[] { 1 });
            neuralNetwork.BackPropagate(new float[] { 0 });
        }

        Debug.Log(neuralNetwork.FeedForward(new float[] { 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1 })[0]);

        //for (int i = 0; i < 5000; i++)
        //{
        //    neuralNetwork.FeedForward(new float[] { 0, 0, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 0, 0, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 0, 1, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 0, 1, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 0, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 1, 0, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 1, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 1, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //}

        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 1 })[0]);
    }

    private static void GammaTest()
    {
        // 0 0 0 => 0
        // 0 0 1 => 1
        // 0 1 0 => 1
        // 0 1 1 => 0
        // 1 0 0 => 1
        // 1 0 1 => 0
        // 1 1 0 => 0
        // 1 1 1 => 1

        // number of inputs = 3
        // hidden layers 1 = 25 
        // hidden layers 2 = 25 
        // number of outputs = 1 

        GammaNeuralNetwork net = new GammaNeuralNetwork(new int[] { 3, 25, 25, 1 });
        net.FeedForward(new float[] { 0, 0, 0 });

        //for (int i = 0; i < 5000; i++)
        //{
        //    net.FeedForward(new float[] { 0, 0, 0 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 0, 0, 1 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 0, 1, 0 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 0, 1, 1 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 0, 0 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 1, 0, 1 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 1, 0 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 1, 1 });
        //    net.BackProp(new float[] { 1 });
        //}

        //Debug.Log(net.FeedForward(new float[] { 0, 0, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 0, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 1, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 1, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 0, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 0, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 1, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 1, 1 })[0]);
    }

    void Update()
    {

    }

}
