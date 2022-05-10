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

        // Set input layer neuron count
        neuronCountsInEachLayer[0] = inputNeuronsCount;

        // Set each hidden layer neuron count
        for (int i = 0; i < hiddenLayersNeuronCounts.Length; i++)
            neuronCountsInEachLayer[i + 1] = hiddenLayersNeuronCounts[i];

        // Set output layer neuron count
        neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1] = outputNeuronsCount;

        // Initialise the layers array
        layers = new Layer[neuronCountsInEachLayer.Length];

        // Create each layer
        for (int i = 0; i < layers.Length - 1; i++)
        {
            // input: current layers neuron count, output: next layers neuron count
            layers[i] = new Layer(neuronCountsInEachLayer[i], neuronCountsInEachLayer[i + 1], random);
        }

        layers[layers.Length - 1] = new Layer(neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1], 0, random);

        // Loop through each layer again and connect them
        for (int i = 1; i < layers.Length; i++)
        {
            layers[i - 1].Connect(layers[i]);
        }

        Debug.Log("Network Generated");
    }

    public float[] FeedForward(float[] _inputs)
    {
        // feed the input layer
        layers[0].FeedForward(_inputs);

        // feed every other layer with the outputs of the prv layers
        for (int i = 1; i < layers.Length; i++)
        {
            float[] lastOutputs = layers[i - 1].outputs;
            layers[i].FeedForward(lastOutputs);
        }
        // return the final output
        return layers[layers.Length - 1].GetInputs();
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

        Random random;
        //Layer nextLayer;

        public float[] outputs;

        private const float WeightDecay = 0.001f;
        private const float LearningRate = 1f;

        public Layer(int inputCount, int outputCount, Random _random)
        {
            random = _random;
            neurons = new Neuron[inputCount];
            outputs = new float[outputCount];
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new Neuron(random);
            }
        }

        public float[] FeedForward(float[] _inputs)
        {
            SetNeuronInputValues(_inputs);

            // For all next layer neurons/ current outputs
            for (int i = 0; i < outputs.Length; i++)
            {
                // Reset their output
                outputs[i] = 0;
                // For all current neurons
                for (int j = 0; j < neurons.Length; j++)
                {
                    // increase outputs[i] by the weight from all the shared connections
                    Neuron currentNeuron = neurons[j];
                    if (currentNeuron.connections == null)
                        break;

                    outputs[i] += currentNeuron.inputValue * currentNeuron.connections[i].weight;
                }
                // Sigmoid the next layers neurons 
                outputs[i] = Sigmoid(outputs[i]);
            }

            return outputs;
        }

        void SetNeuronInputValues(float[] inputs)
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                neurons[i].inputValue = inputs[i];
            }
        }

        public float[,] GetWeights()
        {
            float[,] weights = new float[neurons.Length, neurons[0].connections.Length];

            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Length; j++)
                {
                    weights[i, j] = neurons[i].connections[j].weight;
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
            // For outputs
                // set neurons error to output - expected

            // For outputs
                // set bias to error * sigDeriv(output)
            
            // For outputs
                // For Inputs
                    // Set weightNudge to bias * input
        }

        public void BackPropHidden(float[] nextBiases, float[,] nextWeights)
        {
            // for outputs
                // reset bias
                // for next biases
                    // bias += nextbias * nextweight
                // bias *= SidDeriv(output)

            // for outputs
                // for inputs
                    // weightNudge = bias * input
        }

        public void UpdateWeights()
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                for (int j = 0; j < neurons[i].connections.Length; j++)
                {
                    Connection connection = neurons[i].connections[j];
                    connection.weight -= connection.weightNudge * LearningRate;
                    neurons[i].connections[j] = connection;
                }
            }
        }

        public void Connect(Layer otherLayer)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron currentNeuron = neurons[i];
                currentNeuron.connections = new Connection[otherLayer.neurons.Length];

                for (int j = 0; j < otherLayer.neurons.Length; j++)
                {
                    Neuron otherNeuron = otherLayer.neurons[j];
                    currentNeuron.EstablishOneWayConnection(otherNeuron, j, (float)random.NextDouble() - 0.5f);
                }
            }
        }

        public float[] GetInputs()
        {
            return neurons.Select(n => n.inputValue).ToArray();
        }
    }

    public class Neuron
    {
        public float inputValue;
        public float bias;
        public float biasNudge;
        public float desiredValue;
        public float error;
        public Connection[] connections;
        public Random random;

        public Neuron(Random _random)
        {
            inputValue = 0;
            bias = 0;
            biasNudge = 0;
            desiredValue = 0;
            error = 0;
            random = _random;
        }

        public Connection EstablishOneWayConnection(Neuron other, int index, float weight)
        {
            Connection connection = new Connection(this, other, weight);
            connections[index] = connection;
            return connection;
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