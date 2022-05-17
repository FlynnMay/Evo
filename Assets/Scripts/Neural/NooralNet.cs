using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Neural
{
    public class NooralNet
    {
        int[] neuronCounts;
        Layer[] layers;
        Random random = new Random();
        public NooralNet(int inputCount, int[] hiddenCounts, int outputCount)
        {
            InitNeuronCount(inputCount, hiddenCounts, outputCount);

            InitLayers();

            // Link Layers
            layers[0].SetNextLayer(layers[1]);
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

            for (int i = 1; i < layers.Length; i++)
                layers[i] = new Layer(LayerType.HIDDEN, neuronCounts[i], random);

            layers[layers.Length - 1] = new Layer(LayerType.INPUT, neuronCounts[neuronCounts.Length - 1], random);
        }

        void InitNeuronCount(int inputCount, int[] hiddenCounts, int outputCount)
        {
            neuronCounts = new int[1 + hiddenCounts.Length + 1];
            neuronCounts[0] = inputCount;

            for (int i = 1; i < hiddenCounts.Length; i++)
                neuronCounts[i] = hiddenCounts[i];

            neuronCounts[hiddenCounts.Length - 1] = outputCount;
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
        public float bias;
        public Connection[] connections;
        public void ForwardConnect(Neuron otherNeuron, int index, float weight)
        {
            connections[index] = new Connection(this, otherNeuron, weight);
        }
    }

    public struct Connection
    {
        Neuron left;
        Neuron right;
        float weight;
        public Connection(Neuron _left, Neuron _right, float _weight)
        {
            left = _left;
            right = _right;
            weight = _weight;
        }
    }
}
//int i = 0;
//while (true)
//{
//    Console.ReadKey();
//    Console.WriteLine(i++);
//}