using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Implementation from: https://www.youtube.com/watch?v=L_PByyJ9g-I&t=2382
 */
public class GammaNeuralNetwork
{
    int[] layer;
    Layer[] layers;

    public GammaNeuralNetwork(int[] _layer)
    {
        layer = new int[_layer.Length];

        for (int i = 0; i < layer.Length; i++)
            layer[i] = _layer[i];

        layers = new Layer[layer.Length - 1];

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(layer[i], layer[i + 1]);
        }
    }

    public float[] FeedForward(float[] _inputs)
    {
        layers[0].FeedForward(_inputs);

        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].FeedForward(layers[i - 1].outputs);
        }

        return layers[layers.Length - 1].outputs;
    }

    public void BackProp(float[] expected)
    {
        // reverse loop through layers
        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (i == layers.Length - 1)
                layers[i].BackPropOutput(expected);
            else
                layers[i].BackPropHidden(layers[i + 1].gamma, layers[i + 1].weights);
        }

        for (int i = 0; i < layers.Length; i++)
            layers[i].UpdateWeights();
    }

    public class Layer
    {
        int numberOfInputs; // number of neurons in the previous layer
        int numberOfOutputs; // number of neurons in the current layer

        public float[] outputs;
        public float[] inputs;
        public float[,] weights;
        public float[,] weightsDelta;
        public float[] gamma;
        public float[] error;

        System.Random random = new System.Random();
        public float LearningRate { get; private set; } = 1.0f;


        public Layer(int _numberOfInputs, int _numberOfOutputs)
        {
            numberOfInputs = _numberOfInputs;
            numberOfOutputs = _numberOfOutputs;

            inputs = new float[numberOfOutputs];
            outputs = new float[numberOfOutputs];
            weights = new float[numberOfOutputs, numberOfInputs];
            weightsDelta = new float[numberOfOutputs, numberOfInputs];
            gamma = new float[numberOfOutputs];
            error = new float[numberOfOutputs];

            InitilizeWeights();
        }

        public void InitilizeWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] = (float)random.NextDouble() - 0.5f;
                }
            }
        }

        public float[] FeedForward(float[] _inputs)
        {
            inputs = _inputs;

            for (int i = 0; i < numberOfOutputs; i++)
            {
                // reset outputs
                outputs[i] = 0;
                for (int j = 0; j < numberOfInputs; j++)
                {
                    // apply weight value
                    outputs[i] += inputs[j] * weights[i, j];
                }

                outputs[i] = Sigmoid(outputs[i]);
            }

            return outputs;
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weights[i, j] -= weightsDelta[i, j] * LearningRate;
                }
            }
        }

        public void BackPropOutput(float[] expected)
        {
            for (int i = 0; i < numberOfOutputs; i++)
                error[i] = outputs[i] - expected[i];

            for (int i = 0; i < numberOfOutputs; i++)
                gamma[i] = error[i] * SigmoidDerivative(outputs[i]);

            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        public void BackPropHidden(float[] gammaForward, float[,] weightsForward)
        {
            for (int i = 0; i < numberOfOutputs; i++)
            {
                gamma[i] = 0;
                for (int j = 0; j < gammaForward.Length; j++)
                {
                    gamma[i] += gammaForward[j] * weightsForward[j, i];
                }
                gamma[i] *= SigmoidDerivative(outputs[i]);
            }

            for (int i = 0; i < numberOfOutputs; i++)
            {
                for (int j = 0; j < numberOfInputs; j++)
                {
                    weightsDelta[i, j] = gamma[i] * inputs[j];
                }
            }
        }

        static float Sigmoid(float x)
        {
            return 1.0f / (1.0f + (float)Mathf.Exp(-x));
        }

        static float SigmoidDerivative(float x)
        {
            return x * (1 - x);
        }
    }
}
