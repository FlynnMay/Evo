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

        layers = new Layer[neuronCountsInEachLayer.Length - 1];

        for (int i = 0; i < layers.Length; i++)
            layers[i] = new Layer(neuronCountsInEachLayer[i], neuronCountsInEachLayer[i + 1], random, (i > 0) ? layers[i - 1] : null);

        //int outputNeurons = neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1];
        //int inputNeurons = neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 2];
        //layers[layers.Length - 1] = new Layer(outputNeurons, outputNeurons, random, layers[layers.Length - 2]);

        Debug.Log("Network Generated");
    }

    public float[] FeedForward(float[] _inputs)
    {
        // feed the input layer
        layers[0].FeedForward(_inputs);

        // feed every other layer with the outputs of the prv layers
        for (int i = 1; i < layers.Length; i++)
        {
            float[] lastOutputs = layers[i - 1].neurons.Select(n => n.outputValue).ToArray();
            layers[i].FeedForward(lastOutputs);
        }
        // return the final output
        return layers[layers.Length - 1].neurons.Select(n => n.outputValue).ToArray();
    }

    public void BackPropagation(float[] expected)
    {
        for (int i = layers.Length - 1; i >= 0; i--)
        {
            if (i == layers.Length - 1)
                layers[i].BackPropOutput(expected);
            else
                layers[i].BackPropHidden(layers[i + 1].GetBiases(), layers[i + 1].GetWeights());
        }

        for (int i = 0; i < layers.Length; i++)
            layers[i].UpdateWeights();

        
    }

    public class Layer
    {
        public Neuron[] neurons;
        //public float[] inputs;
        // used for backproping
        Random random;
        Layer prevLayer;


        private const float WeightDecay = 0.001f;
        private const float LearningRate = 1f;

        public Layer(int inputCount, int outputCount, Random _random, Layer _prevLayer = null)
        {
            random = _random;
            //inputs = new float[inputCount];
            neurons = new Neuron[inputCount];
            prevLayer = _prevLayer;
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new Neuron(prevLayer, random);
            }
        }

        public float[] FeedForward(float[] _inputs)
        {
            //// take in inputs
            //inputs = _inputs;

            //// for each output
            //for (int i = 0; i < neurons.Length; i++)
            //{
            //    Neuron neuron = neurons[i];
            //    // reset outputs
            //    neuron.outputValue = 0;

            //    // CONNECTIONS ARE FUCKING BROKEN FIX THEM YOU MORON
            //    for (int j = 0; j < neuron.connections.Count; j++)
            //    {
            //        Debug.Log($"{i}: {j}");
            //        // apply weight value
            //        neuron.outputValue += _inputs[i] * neuron.connections[j].weight;
            //    }
            //    // set output
            //    neuron.outputValue = Sigmoid(neuron.outputValue - neurons[i].bias);
            //}

            // For all next layer neurons
                // Reset their output
                // For all current neurons
                    // increase it by the weight from all the next layers neurons
                // Sigmoid the next layers neurons 

            return neurons.Select(n => n.outputValue).ToArray();
        }

        public float[,] GetWeights()
        {
            float[,] weights = new float[neurons.Length, neurons[0].connections.Count];

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Count; j++)
                {
                    weights[i, j] = neurons[i].connections.ElementAt(j).weight;
                }
            }

            return weights;
        }

        public float[] GetBiases()
        {
            return neurons.Select(n => n.bias).ToArray();
        }

        public void BackPropOutput(float[] expected)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                neuron.error = neuron.outputValue - expected[i];
            }
            
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                neuron.bias = neuron.error - SigmoidDerivative(neuron.outputValue);
            }

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Count; j++)
                {
                    Connection connection = neurons[i].connections[j];
                    connection.weightNudge = neurons[i].bias * prevLayer.neurons[j].outputValue;
                    neurons[i].connections[j] = connection;
                }
            }
            
        }

        public void BackPropHidden(float[] nextBiases, float[,] nextWeights)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                neuron.bias = 0;
                for (int j = 0; j < nextBiases.Length; j++)
                {
                    neuron.bias += nextBiases[j] * nextWeights[j, i];
                }
                neuron.bias *= SigmoidDerivative(neuron.outputValue);
            }

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Count; j++)
                {
                    Connection connection = neurons[i].connections[j];
                    connection.weightNudge = neurons[i].bias * prevLayer.neurons[j].outputValue;
                    neurons[i].connections[j] = connection;
                }
            }
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Count; j++)
                {
                    Connection connection = neurons[i].connections[j];
                    connection.weight -= connection.weightNudge * LearningRate;
                    neurons[i].connections[j] = connection;
                }
            }
        }
    }

    public class Neuron
    {
        //public float inputValue;
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
            //inputValue = int.MaxValue; // Should help to see when this is reset

            if (prevLayer != null)
            {
                for (int i = 0; i < prevLayer.neurons.Length; i++)
                {
                    EstablishConnection(prevLayer.neurons[i], weight: (float)random.NextDouble() - 0.5f);
                }
            }
        }

        Connection EstablishConnection(Neuron other, float weight)
        {
            Connection connection = new Connection(other, this, weight);
            connections.Add(connection);
            other.connections.Add(connection);
            return connection;
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