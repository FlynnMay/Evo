using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

// Im running out of name ideas
public class NewralNet : MonoBehaviour
{
    public int[] neuronCountsInEachLayer;
    public Layer[] layers;
    Random random;

    public int WeightDecay { get; private set; }
    public float LearningRate { get; private set; }

    public NewralNet(int inputNeuronsCount, int[] hiddenLayersNeuronCounts, int outputNeuronsCount, Random _random = null)
    {
        random = (_random != null) ? _random : new Random();

        SetNeuronCounts(inputNeuronsCount, hiddenLayersNeuronCounts, outputNeuronsCount);

        // Initialise the layers array
        layers = new Layer[neuronCountsInEachLayer.Length];
        layers[0] = new Layer(neuronCountsInEachLayer[0], neuronCountsInEachLayer[1], 0, random);
        for (int i = 1; i < layers.Length - 1; i++)
        {
            layers[i] = new Layer(neuronCountsInEachLayer[i], neuronCountsInEachLayer[i + 1], layers[i - 1], i, random);
        }
        layers[layers.Length - 1] = new Layer(neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1], 0, layers[layers.Length - 2], layers.Length - 1, random);

        for (int i = layers.Length - 1; i >= 1; i--)
        {
            layers[i - 1].Connect(layers[i]);
        }

        Debug.Log("Network Generated");
    }

    void SetNeuronCounts(int inputNeuronsCount, int[] hiddenLayersNeuronCounts, int outputNeuronsCount)
    {
        //-------------------------(input layer) + (hidden layers) + (output layer)
        neuronCountsInEachLayer = new int[1 + hiddenLayersNeuronCounts.Length + 1];

        // Set input layer neuron count
        neuronCountsInEachLayer[0] = inputNeuronsCount;

        // Set each hidden layer neuron count
        for (int i = 0; i < hiddenLayersNeuronCounts.Length; i++)
            neuronCountsInEachLayer[i + 1] = hiddenLayersNeuronCounts[i];

        // Set output layer neuron count
        neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1] = outputNeuronsCount;
    }

    public float[] FeedForward(float[] inputs)
    {
        float[] outputs = layers[0].FeedForward(inputs);

        for (int i = 1; i < layers.Length; i++)
        {
            outputs = layers[i].FeedForward(outputs);
        }

        outputs = layers[layers.Length - 1].neurons.Select(n => n.value).ToArray();
        return outputs;
    }

    public void BackPropagate(float[] expected)
    {
        expected = layers[layers.Length - 1].BackPropagate(expected);
        for (int i = layers.Length - 2; i >= 0; i--)
        {
            layers[i].BackPropagate(expected);
        }

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].UpdateWeights();
        }
    }
}

public class Layer
{
    public Layer prevLayer;
    public Neuron[] neurons;
    public OutputData[] outputs;
    Random random;
    int index = -1;

    public Layer(int inputCount, int outputCount, int _index, Random _random)
    {
        index = _index;
        neurons = new Neuron[inputCount];
        outputs = new OutputData[outputCount];
        random = _random;

        for (int i = 0; i < neurons.Length; i++)
        {
            neurons[i] = new Neuron();
        }
    }

    public Layer(int inputCount, int outputCount, Layer _prevLayer, int _index, Random _random) : this(inputCount, outputCount, _index, _random)
    {
        prevLayer = _prevLayer;
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < neurons.Length; i++)
        {
            Neuron neuron = neurons[i];
            neuron.value = inputs[i];
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            OutputData output = outputs[i];
            for (int j = 0; j < neurons.Length; j++)
            {
                Neuron neuron = neurons[j];
                Connection connection = neuron.connections[i];
                output.value += neuron.value * connection.weight;
                neuron.connections[i] = connection;
            }
            output.value = Sigmoid(output.value + output.bias);
            outputs[i] = output;
        }

        return outputs.Select(o => o.value).ToArray();
    }

    public float[] BackPropagate(float[] expected)
    {
        for (int i = 0; i < outputs.Length; i++)
        {
            OutputData output = outputs[i];
            output.desiredValue = expected[i];
            output.error = 2 * (output.value - output.desiredValue);
            output.biasNudge += output.error;

            for (int j = 0; j < neurons.Length; j++)
            {
                Neuron neuron = neurons[i];
                Connection connection = neuron.connections[i];

                connection.weightNudge = neuron.value * output.error;
                
                neuron.connections[i] = connection;
            }
            outputs[i] = output;
        }

        return outputs.Select(o => o.value).ToArray();
    }

    public void Connect(Layer otherLayer)
    {
        Neuron[] otherNeurons = otherLayer.neurons;
        for (int i = 0; i < neurons.Length; i++)
        {
            Neuron currentNeuron = neurons[i];
            currentNeuron.connections = new Connection[otherNeurons.Length];
            for (int j = 0; j < otherNeurons.Length; j++)
            {
                Neuron otherNeuron = otherNeurons[j];
                currentNeuron.Connect(otherNeuron, j, random);
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

    public void UpdateWeights()
    {
        for (int i = 0; i < outputs.Length; i++)
        {
            OutputData output = outputs[i];
            output.desiredValue = 0;
            output.error = 0;
            output.bias += output.biasNudge;
            output.biasNudge = 0;
            for (int j = 0; j < neurons.Length; j++)
            {
                Neuron neuron = neurons[j];
                Connection connection = neuron.connections[i];
                connection.weight += connection.weightNudge;
                connection.weightNudge = 0;
                neuron.connections[i] = connection;
            }
            outputs[i] = output;
        }
    }
}

public class Neuron
{
    public float value;
    public Connection[] connections;

    // Creates a connection to the other neuron, if random is null the weight will be 0
    public void Connect(Neuron otherNeuron, int index, Random random)
    {
        connections[index] = new Connection(this, otherNeuron, random == null ? 0.0f : (float)random.NextDouble() - 0.5f);
    }
}

public struct Connection
{
    public Neuron left;
    public Neuron right;
    public float weight;
    public float weightNudge;

    public Connection(Neuron _left, Neuron _right, float _weight)
    {
        weightNudge = 0;
        left = _left;
        right = _right;
        weight = _weight;
    }
}

public struct OutputData
{
    public float value;
    public float desiredValue;
    public float bias;
    public float biasNudge;
    public float error;
}
