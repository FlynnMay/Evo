using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void BackPropagation(float[] expected)
        {
            layers[layers.Length - 1].OutputBackProp(expected);
        }

    }

    public class Layer
    {
        public LayerType layerType;
        public Neuron[] neurons;
        public Layer prevLayer;
        public Layer nextLayer;
        Random random;

        public Layer(LayerType _layerType, int _inputCount, Random _random, Layer _prevLayer = null, Layer _nextLayer = null)
        {
            layerType = _layerType;
            prevLayer = _prevLayer;
            nextLayer = _nextLayer;
            random = _random;

            neurons = new Neuron[_inputCount];

            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = new Neuron();
            }
        }

        public void SetPreviousLayer(Layer _prevLayer)
        {
            prevLayer = _prevLayer;
        }

        public void SetNextLayer(Layer _nextLayer)
        {
            nextLayer = _nextLayer;
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
                    neuron.ForwardConnect(otherNeuron, j, (float)random.NextDouble() - 0.5f);
                }
            }
        }

        public float[] RecursiveFeed(float[] inputs)
        {
            SetInputValues(inputs);

            if (layerType == LayerType.OUTPUT)
                return neurons.Select(n => n.value).ToArray();

            Neuron[] nextNeurons = nextLayer.neurons;
            inputs = new float[nextNeurons.Length];
            for (int i = 0; i < nextNeurons.Length; i++)
            {
                for (int j = 0; j < neurons.Length; j++)
                {
                    Neuron neuron = neurons[j];
                    // Use the output index to get the connection between the current neuron and the output values
                    Connection connection = neuron.connections[i];
                    // Apply the weighted value to the next output value
                    inputs[i] += neuron.value * connection.weight;
                    // Update the connection in the array
                    neuron.connections[i] = connection;
                }
                // Use the transfer function applying the bias as an activation threshold
                Neuron nextNeuron = nextNeurons[i];
                inputs[i] = Sigmoid(inputs[i] + nextNeuron.bias);
            }

            return nextLayer.RecursiveFeed(inputs);
        }

        void SetInputValues(float[] inputs)
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].value = inputs[i];
            }
        }
        public void OutputBackProp(float[] expected)
        {
            float totalError = 0;
            Neuron[] prevNeurons = prevLayer.neurons;
            for (int i = 0; i < neurons.Length; i++)
            {
                Neuron neuron = neurons[i];

                // Calculate how changes in the output value change the error, 1/2(a-y)^2 || 2(a-y)?
                totalError += (float)Math.Pow(neuron.value - expected[i], 2) * 0.5f;

                // Calculate the partial derivative of error with respect to output value
                neuron.error = neuron.value - expected[i];

                // Calculate the partial derivative of the logistics function
                float sigDerivative = SigmoidDerivative(neuron.value);

                for (int j = 0; j < prevNeurons.Length; j++)
                {
                    Neuron prevNeuron = prevNeurons[j];
                    Connection connection = prevNeuron.connections[i];
                    // Calculate the total net input of value change with respect to the weight
                    connection.weightNudge = -(sigDerivative * neuron.error) * prevNeuron.value;
                }

            }
        }

        static float Sigmoid(float x)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-x));
        }

        static float SigmoidDerivative(float x)
        {
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
        public float error;
        public float bias;
        public Connection[] connections;
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
        }
    }
}