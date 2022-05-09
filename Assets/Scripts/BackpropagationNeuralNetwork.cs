using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/*
    Implementation from: https://www.youtube.com/watch?v=-WjKICvAOsY
    I cant figure out how to use this network
 */
public class BackpropagationNeuralNetwork
{
    System.Random random;
    public float[][] values;
    public float[][] biases;
    public float[][][] weights;

    public float[][] desiredValues;
    public float[][] biasNudges;
    public float[][][] weightNudges;

    private const float WeightDecay = 0.001f;
    private const float LearningRate = 1f;

    public BackpropagationNeuralNetwork(IReadOnlyList<int> structure)
    {
        random = new System.Random();
        values = new float[structure.Count][];
        desiredValues = new float[structure.Count][];
        biases = new float[structure.Count][];
        biasNudges = new float[structure.Count][];
        weights = new float[structure.Count - 1][][];
        weightNudges = new float[structure.Count - 1][][];

        for (int i = 0; i < structure.Count; i++)
        {
            values[i] = new float[structure[i]];
            desiredValues[i] = new float[structure[i]];
            biases[i] = new float[structure[i]];
            biasNudges[i] = new float[structure[i]];
        }

        for (int i = 0; i < structure.Count - 1; i++)
        {
            weights[i] = new float[values[i + 1].Length][];
            weightNudges[i] = new float[values[i + 1].Length][];

            for (int j = 0; j < weights[i].Length; j++)
            {
                weights[i][j] = new float[values[i].Length];
                weightNudges[i][j] = new float[values[i].Length];
                for (int k = 0; k < weights[i][j].Length; k++)
                    weights[i][j][k] = (float)random.NextDouble() * Mathf.Sqrt(2.0f / weights[i][j].Length);
            }
        }
    }

    public float[] Test(float[] inputs)
    {
        // Set the inputs
        for (int i = 0; i < values[0].Length; i++)
            values[0][i] = inputs[i];

        // iterate over nodes
        for (int i = 1; i < values.Length; i++)
        {
            for (int j = 0; j < values[i].Length; j++)
            {
                // calculate their values
                values[i][j] = Sigmoid(Sum(values[i - 1], weights[i - 1][j]) + biases[i][j]);
                desiredValues[i][j] = values[i][j];
            }
        }

        // return outputs
        return values[values.Length - 1];
    }

    static float Sigmoid(float x)
    {
        return 1.0f / (1.0f + (float)Mathf.Exp(-x));
    }

    static float SigmoidDerivative(float x)
    {
        return x * (1 - x);
    }

    // Returns similar results as Sigmoid(float x) (less precise), but is less intensive
    static float HardSigmoid(float x)
    {
        if (x < -2.5f)
            return 0.0f;
        if (x > 2.5f)
            return 1.0f;

        return 0.2f * x + 0.5f;
    }

    public void Train(float[][] trainingInputs, float[][] trainingOutputs)
    {
        // Iterate over every training input
        for (int i = 0; i < trainingInputs.Length; i++)
        {
            // Get outputs from network
            Test(trainingInputs[i]);
            for (int j = 0; j < desiredValues[desiredValues.Length - 1].Length; j++)
            {
                // Set correct/desired outputs of network
                desiredValues[desiredValues.Length - 1][j] = trainingOutputs[i][j];
            }

            /* =========================
             *  Backwards Propagation
             *  
             *     end   Start
             * ()--()--()--()
             *     / 
             * ()-/
             * 
             * |<-------------|
             * 
             * =========================
             */

            // Iterate over every layer backwards (backpropagation)
            for (int j = values.Length - 1; j >= 1; j--)
            {
                // Iterate over every neuron
                for (int k = 0; k < values[j].Length; k++)
                {
                    // Store by how much we need to nudge the weights and biases
                    float biasNudge = SigmoidDerivative(values[j][k]) * (desiredValues[j][k] - values[j][k]);
                                                                        // error
                    biasNudges[j][k] += biasNudge;

                    for (int l = 0; l < values[j - 1].Length; l++)
                    {
                        // Store the desired values of the previous layer
                        float weightNudge = values[j - 1][l] * biasNudge;
                        weightNudges[j - 1][k][l] += weightNudge;

                        float valueNudge = weights[j - 1][k][l] * biasNudge;
                        desiredValues[j - 1][l] += valueNudge;
                    }
                }
            }
        }

        // Iterate over every layer backwards again
        for (int i = values.Length - 1; i >= 1; i--)
        {
            for (int j = 0; j < values[i].Length; j++)
            {
                // Apply weights and biased changes and reset
                biases[i][j] += biasNudges[i][j] * LearningRate;
                biases[i][j] *= 1 - WeightDecay;
                biasNudges[i][j] = 0;

                for (int k = 0; k < values[i - 1].Length; k++)
                {
                    weights[i - 1][j][k] += weightNudges[i - 1][j][k] * LearningRate;
                    weights[i - 1][j][k] *= 1 - WeightDecay;
                    weightNudges[i - 1][j][k] = 0;
                }
                desiredValues[i][j] = 0;
            }
        }

    }

    static float Sum(IEnumerable<float> values, IReadOnlyList<float> weights)
    {
        // iterate over every neuron in a given layer
        // then multiply the value by the coresponding weight
        // finally add it all together

        return values.Select((v, i) => v * weights[i]).Sum();
        // this is equivalent to our "w1 a1 + w2 a2 + w3 a3..."
    }
}
