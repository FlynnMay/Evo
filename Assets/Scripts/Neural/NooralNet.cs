using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Neural
{
    // with help from https://mattmazur.com/2015/03/17/a-step-by-step-backpropagation-example/
    public class NooralNet
    {
        public int[] neuronCounts;
        public Layer[] layers;
        Random random = new Random();
        public NooralNet(int inputCount, int[] hiddenCounts, int outputCount)
        {
            InitNeuronCount(inputCount, hiddenCounts, outputCount);

            InitLayers();

            // Link Layers
            layers[0].SetNextLayer(layers[1]);
            layers[0].Connect(layers[1]);
            for (int i = 1; i < layers.Length - 1; i++)
            {
                layers[i].SetNextLayer(layers[i + 1]);
                layers[i].SetPreviousLayer(layers[i - 1]);

                // Connect Neurons
                layers[i].Connect(layers[i + 1]);
            }
            layers[layers.Length - 1].SetPreviousLayer(layers[layers.Length - 2]);
        }

        void InitLayers()
        {
            layers = new Layer[neuronCounts.Length];
            layers[0] = new Layer(LayerType.INPUT, neuronCounts[0], random);

            for (int i = 1; i < layers.Length - 1; i++)
                layers[i] = new Layer(LayerType.HIDDEN, neuronCounts[i], random);

            layers[layers.Length - 1] = new Layer(LayerType.OUTPUT, neuronCounts[neuronCounts.Length - 1], random);
        }

        void InitNeuronCount(int inputCount, int[] hiddenCounts, int outputCount)
        {
            neuronCounts = new int[1 + hiddenCounts.Length + 1];
            neuronCounts[0] = inputCount;

            for (int i = 0; i < hiddenCounts.Length; i++)
                neuronCounts[i + 1] = hiddenCounts[i];

            neuronCounts[neuronCounts.Length - 1] = outputCount;
        }

        public float[] FeedForward(float[] inputs)
        {
            return layers[0].RecursiveFeed(inputs);
        }

        public void BackPropagate(float[] expected)
        {
            Layer outputLayer = layers[layers.Length - 1];
            outputLayer.OutputBackProp(expected);
            outputLayer.leftLayer.RecursiveHiddenBackProp();
        }

        public float GetTotalError()
        {
            Layer outputLayer = layers[layers.Length - 1];
            return outputLayer.totalError;
        }

        public override string ToString()
        {
            string output = $"[NeuralNetwork] Layers: {layers.Length}.";
            return output;
        }
    }

    public class Layer
    {
        public LayerType layerType;
        public Neuron[] neurons;
        public Layer leftLayer;
        public Layer rightLayer;
        public float totalError;
        Random random;
        float bias;

        public float LearningRate { get; set; } = 1.0f;
        public float WeightDecay { get; set; } = 0.001f;

        public Layer(LayerType _layerType, int _inputCount, Random _random, Layer _prevLayer = null, Layer _nextLayer = null)
        {
            layerType = _layerType;
            leftLayer = _prevLayer;
            rightLayer = _nextLayer;
            random = _random;

            neurons = new Neuron[_inputCount];
            bias = (float)random.NextDouble();

            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new Neuron((float)random.NextDouble() - 0.5f);
            }
        }

        public void SetPreviousLayer(Layer _prevLayer)
        {
            leftLayer = _prevLayer;
        }

        public void SetNextLayer(Layer _nextLayer)
        {
            rightLayer = _nextLayer;
        }

        public void Connect(Layer layer)
        {
            Neuron[] otherNeurons = layer.neurons;
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                neuron.connections = new Connection[otherNeurons.Length];
                for (int j = 0; j < otherNeurons.Length; j++)
                {
                    Neuron otherNeuron = otherNeurons[j];
                    neuron.ForwardConnect(otherNeuron, j, (float)random.NextDouble());
                }
            }
        }

        public float[] RecursiveFeed(float[] inputs)
        {
            SetInputValues(inputs);

            if (layerType == LayerType.OUTPUT)
                return neurons.Select(n => n.value).ToArray();

            #region backup code
            //Neuron[] nextNeurons = rightLayer.neurons;
            //inputs = new float[nextNeurons.Length];
            //for (int i = 0; i < nextNeurons.Length; i++)
            //{
            //    for (int j = 0; j < neurons.Length; j++)
            //    {
            //        Neuron neuron = neurons[j];
            //        // Use the output index to get the connection between the current neuron and the output values
            //        Connection connection = neuron.connections[i];
            //        // Apply the weighted value to the next output value
            //        inputs[i] += neuron.value * connection.weight;
            //        // Update the connection in the array
            //        neuron.connections[i] = connection;
            //    }
            //    // Use the transfer function applying the bias as an activation threshold
            //    Neuron nextNeuron = nextNeurons[i];
            //    inputs[i] = Sigmoid(inputs[i] + nextNeuron.bias);
            //}
            #endregion

            float[] outputs = new float[rightLayer.neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                for (int j = 0; j < neuron.connections.Length; j++)
                {
                    Connection connection = neuron.connections[j];
                    outputs[j] += neuron.value * connection.weight;
                }
            }

            for (int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = Sigmoid(outputs[i] /*+ bias*/);
            }

            return rightLayer.RecursiveFeed(outputs);
        }

        void SetInputValues(float[] inputs)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].value = inputs[i];
                neurons[i].input = inputs[i];
            }
        }
        public void OutputBackProp(float[] expected)
        {
            totalError = 0;

            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];

                // Calculate how changes in the output value change the error, 1/2(a-y)^2 || 2(a-y)?
                totalError += (float)Math.Pow(neuron.value - expected[i], 2) * 0.5f;

                // Calculate the derivative of the logistics function
                float sigDerivative = SigmoidDerivative(neuron.value);

                // Calculate the partial derivative of error with respect to output value
                neuron.error = -(expected[i] - neuron.value) * sigDerivative;
                //bias += neuron.error * LearningRate;
                //bias *= 1 - WeightDecay;
            }
        }

        public void RecursiveHiddenBackProp()
        {
            // We use the 'right layer' as it is actually the previous layer when going backwards
            Neuron[] prevNeurons = rightLayer.neurons;
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                float errorWRTPrevOutput = 0;
                for (int j = 0; j < prevNeurons.Length; j++)
                {
                    Neuron prevNeuron = prevNeurons[j];
                    Connection connection = neuron.connections[j];
                    errorWRTPrevOutput += prevNeuron.error * connection.weight;
                }
                neuron.error = errorWRTPrevOutput * SigmoidDerivative(neuron.value);
                //bias += neuron.error * LearningRate;
                //bias *= 1 - WeightDecay;
            }

            if (layerType == LayerType.INPUT)
            {
                CalculateWeight();
                return;
            }

            leftLayer.RecursiveHiddenBackProp();
        }

        public void CalculateWeight()
        {
            if (layerType == LayerType.OUTPUT)
                return;

            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];
                for (int j = 0; j < neuron.connections.Length; j++)
                {
                    Connection connection = neuron.connections[j];
                    connection.weightNudge = neuron.error * neuron.input;
                    connection.weight -= LearningRate * connection.weightNudge;
                    connection.weight *= 1 - WeightDecay;
                    connection.weight = Mathf.Round(Mathf.Clamp(connection.weight, 0, 1) * 100) / 100;
                    neuron.connections[j] = connection;
                }
            }
            rightLayer.CalculateWeight();
        }

        static float Sigmoid(float x)
        {
            return 1.0f / (1.0f + (float)Mathf.Exp(-x));
        }

        static float SigmoidDerivative(float x)
        {
            x = Sigmoid(x);
            return x * (1 - x);
        }

    }

    public enum LayerType
    {
        INPUT,
        HIDDEN,
        OUTPUT
    }

    public class Neuron
    {
        public float value;
        public float input;
        public float error;
        public float bias;
        public Connection[] connections;

        public Neuron(float _bias)
        {
            bias = _bias;
        }

        public void ForwardConnect(Neuron otherNeuron, int index, float weight)
        {
            connections[index] = new Connection(this, otherNeuron, weight);
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
            left = _left;
            right = _right;
            weight = _weight;
            weightNudge = 0;
        }
    }
}