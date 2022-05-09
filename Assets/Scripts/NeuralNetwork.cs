using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public class NeuralNetwork
{
    // Please for the love of god find a better name for this
    int[] neuronCountsInEachLayer;
    Layer[] layers;
    Random random;

    public int WeightDecay { get; private set; }
    public float LearningRate { get; private set; }

    public NeuralNetwork(int inputNeuronsCount, int[] hiddenLayersNeuronCounts, int outputNeuronsCount, Random _random = null)
    {
        random = (_random != null) ? _random : new Random();

        //-------------------------(input layer) + (hidden layers) + (output layer)
        neuronCountsInEachLayer = new int[1 + hiddenLayersNeuronCounts.Length + 1];

        neuronCountsInEachLayer[0] = inputNeuronsCount;

        for (int i = 0; i < hiddenLayersNeuronCounts.Length; i++)
            neuronCountsInEachLayer[i + 1] = hiddenLayersNeuronCounts[i];

        neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1] = outputNeuronsCount;

        layers = new Layer[neuronCountsInEachLayer.Length];
        for (int i = 0; i < layers.Length - 1; i++)
            layers[i] = new Layer(neuronCountsInEachLayer[i], neuronCountsInEachLayer[i + 1], random, (i > 0) ? layers[i - 1] : null);

        int InNeurons = neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1];
        layers[layers.Length - 1] = new Layer(InNeurons, InNeurons, random);

        Debug.Log("Network Generated");
    }

    public float[] FeedForward(float[] _inputs)
    {
        // feed the input layer
        layers[0].FeedForward(_inputs);

        // feed every other layer with the outputs of the prv layers
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i].FeedForward(layers[i - 1].outputs);
        }
        // return the final output
        return layers[layers.Length - 1].outputs;
    }

    public void BackPropagation(float[] expected)
    {
        
    }

    public class Layer
    {
        int numberOfInputs; // number of neurons in the previous layer
        int numberOfOutputs; // number of neurons in the current layer

        public Neuron[] neurons;
        // used for backproping
        public float[] outputs;
        public float[] inputs;
        Random random;

        private const float WeightDecay = 0.001f;
        private const float LearningRate = 1f;

        public Layer(int inputCount, int outputCount, Random _random, Layer prevLayer = null)
        {
            numberOfInputs = inputCount;
            numberOfOutputs = outputCount;
            random = _random;
            outputs = new float[numberOfOutputs];
            neurons = new Neuron[numberOfOutputs];
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new Neuron(prevLayer, random);
            }
        }

        public float[] FeedForward(float[] _inputs)
        {
            // take in inputs
            inputs = _inputs;

            // for each output
            for (int i = 0; i < numberOfOutputs; i++)
            {
                // reset outputs
                outputs[i] = 0;
                int j = 0;
                foreach (Connection connection in neurons[i].connections)
                {
                    // apply weight value
                    outputs[i] += inputs[j++] * connection.weight;
                }
                // set output
                outputs[i] = Sigmoid(outputs[i] - neurons[i].bias);
            }
            return outputs;
        }
    }

    public class Neuron
    {
        public float inputValue;
        public float outputValue;
        public float bias;
        public float biasNudge;
        public float desiredValue;
        public float error;
        public int layer = 0;
        public List<Connection> connections;
        public Random random;
        public Layer prevLayer;

        public Neuron(Layer _prevLayer, Random _random)
        {
            prevLayer = _prevLayer;
            random = _random;
            bias = 0;
            connections = new List<Connection>();

            if (prevLayer != null)
            {
                for (int i = 0; i < prevLayer.neurons.Length; i++)
                {
                    connections.Add(new Connection(this, prevLayer.neurons[i], _weight: (float)random.NextDouble() - 0.5f));
                }
            }
        }

        public float FeedForward()
        {
            //   sigmoid the output
            return Sigmoid(SumOfPreviousConnectedLayer() - bias);
        }

        float SumOfPreviousConnectedLayer()
        {
            float sum = 0;
            if (prevLayer != null)
            {
                for (int i = 0; i < prevLayer.neurons.Length; i++)
                {
                    Neuron neuron = prevLayer.neurons[i];
                    //List<Connection> list = neuron.connections.Where(connection => connection.toNeuron == this).ToList();

                    foreach (Connection connection in connections)
                    {
                        // increase the sum by the current input * the weight
                        sum += neuron.outputValue * connection.weight;
                    }
                }
            }
            return sum;
        }

        float[] GetWeights()
        {
            float[] weights = new float[connections.Count];
            for (int i = 0; i < connections.Count; i++)
            {
                Connection connection = connections.ElementAt(i);
                weights[i] = connection.weight;
            }
            return weights;
        }
    }

    public struct Connection
    {
        public Neuron fromNeuron;
        public Neuron toNeuron;
        public float weight;
        public float weightNudge;

        public Connection(Neuron from, Neuron to, float _weight)
        {
            weightNudge = 0;
            fromNeuron = from;
            toNeuron = to;
            weight = _weight;
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

/*
 *  Neural Network Psuedocode
 *  
 *  Assign all network inputs and output
 *  Initialize all weights with small random numbers, typically between -1 and 1 
 *  
 *  repeat (for every pattern in the training set)
 *  
 *      Present the pattern to the network
 *      
 //     Propagated the input forward through the network
 *        for each layer in the network 
 *          for every node in the layer
 *          
 *              1. Calculate the weight sum of the inputs to the node 
 *              2. Add the threshold to the sum 
 *              3. Calculate the activation for the node 
 *          
 *          end 
 *        end 
 *        
//      Propagate the errors backward through the network
 *        for every node in the output layer 
 *            calculate the error signal 
 *        end 
 *        
 *        for all hidden layers 
 *            for every node in the layer 
 *                1. Calculate the node's signal error 
 *                2. Update each node's weight in the network 
 *            end 
 *        end 
 *          
//      Calculate Global Error
 *        Calculate the Error Function
 *
 *      end
 * 
 * while ((maximum  number of iterations < than specified) AND 
 *        (Error Function is > than specified))
 */