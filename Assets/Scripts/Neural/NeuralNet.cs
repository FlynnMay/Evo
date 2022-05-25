//using System;
//using System.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Random = System.Random;

//public class NeuralNet : MonoBehaviour
//{
//    public int[] neuronCountsInEachLayer;
//    public Layer[] layers;
//    Random random;

//    public int WeightDecay { get; private set; }
//    public float LearningRate { get; private set; }

//    public NeuralNet(int inputNeuronsCount, int[] hiddenLayersNeuronCounts, int outputNeuronsCount, Random _random = null)
//    {
//        // make a new random if random is null
//        random = _random ?? new Random();

//        SetNeuronCounts(inputNeuronsCount, hiddenLayersNeuronCounts, outputNeuronsCount);

//        InitLayers();

//        ConnectLayerNeurons();

//        Debug.Log("Network Generated");
//    }

//    private void ConnectLayerNeurons()
//    {
//        for (int i = layers.Length - 1; i >= 1; i--)
//        {
//            layers[i - 1].ConnectLayer(layers[i]);
//        }
//    }

//    private void InitLayers()
//    {
//        // Initialise the layers array
//        layers = new Layer[neuronCountsInEachLayer.Length];

//        // Set the input layer 
//        layers[0] = new Layer(neuronCountsInEachLayer[0], neuronCountsInEachLayer[1], Layer.LayerType.INPUT, random);

//        // Set the hidden layers ('i' starts at 1 to skip input and 'layers.length - 1' ignores output)
//        for (int i = 1; i < layers.Length - 1; i++)
//            layers[i] = new Layer(neuronCountsInEachLayer[i], neuronCountsInEachLayer[i + 1], layers[i - 1], Layer.LayerType.HIDDEN, random);

//        // Set the output layer (input neurons is set to the count of at the end of the count array, output is set to the same)
//        layers[layers.Length - 1] = new Layer(neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1], neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1], layers[layers.Length - 2], Layer.LayerType.OUTPUT, random);
//    }

//    void SetNeuronCounts(int inputNeuronsCount, int[] hiddenLayersNeuronCounts, int outputNeuronsCount)
//    {
//        //-------------------------(input layer) + (hidden layers) + (output layer)
//        neuronCountsInEachLayer = new int[1 + hiddenLayersNeuronCounts.Length + 1];

//        // Set input layer neuron count
//        neuronCountsInEachLayer[0] = inputNeuronsCount;

//        // Set each hidden layer neuron count
//        for (int i = 0; i < hiddenLayersNeuronCounts.Length; i++)
//            neuronCountsInEachLayer[i + 1] = hiddenLayersNeuronCounts[i];

//        // Set output layer neuron count
//        neuronCountsInEachLayer[neuronCountsInEachLayer.Length - 1] = outputNeuronsCount;
//    }

//    public float[] FeedForward(float[] inputs)
//    {
//        float[] outputs = layers[0].FeedForward(inputs);

//        for (int i = 1; i < layers.Length; i++)
//        {
//            outputs = layers[i].FeedForward(outputs);
//        }

//        outputs = layers[layers.Length - 1].neurons.Select(n => n.value).ToArray();
//        return outputs;
//    }

//    public void BackPropagate(float[] expected)
//    {
//        // Backpropagate the output with the expected values
//        // FIX: Currently im trying to take the outputs of this layer to be used as the previous layers expected,
//        // but i assume that is wrong
//        expected = layers[layers.Length - 1].BackPropagateLayer(expected);

//        for (int i = layers.Length - 2; i >= 0; i--)
//        {
//            layers[i].BackPropagateLayer(expected);
//        }

//        for (int i = 0; i < layers.Length; i++)
//        {
//            layers[i].UpdateWeights();
//        }

//        // Calculate the total error (c) on the output layer
        
//    }
//}

//public class Layer
//{
//    public enum LayerType { 
//        INPUT,
//        HIDDEN,
//        OUTPUT
//    }

//    public Layer prevLayer;
//    public Neuron[] neurons;
//    public OutputData[] outputs;
//    public float totalError =0;
//    LayerType layerType;
//    Random random;

//    public Layer(int inputCount, int outputCount, LayerType type, Random _random)
//    {
//        neurons = new Neuron[inputCount];
//        outputs = new OutputData[outputCount];
//        random = _random;
//        layerType = type;

//        for (int i = 0; i < neurons.Length; i++)
//        {
//            neurons[i] = new Neuron();
//        }
//    }

//    public Layer(int inputCount, int outputCount, Layer _prevLayer, LayerType type, Random _random) : this(inputCount, outputCount, type, _random)
//    {
//        prevLayer = _prevLayer;
//    }

//    public float[] FeedForward(float[] inputs)
//    {
//        SetInputValues(inputs);

//        if (layerType == LayerType.OUTPUT)
//            return neurons.Select(n => n.value).ToArray();

//        for (int i = 0; i < outputs.Length; i++)
//        {
//            OutputData output = outputs[i];
//            for (int j = 0; j < neurons.Length; j++)
//            {
//                Neuron neuron = neurons[j];
//                // Use the output index to get the connection between the current neuron and the output values
//                Connection connection = neuron.forwardConnections[i];
//                // Apply the weighted value to the next output value
//                output.value += neuron.value * connection.weight;
//                // Update the connection in the array
//                neuron.forwardConnections[i] = connection;
//            }
//            // Use the transfer function applying the bias as an activation threshold
//            output.value = Sigmoid(output.value + output.bias);
            
//            // Update the output in the array
//            outputs[i] = output;
//        }

//        // Use the output data in as the next feeds inputs
//        return outputs.Select(o => o.value).ToArray();
//    }

//    void SetInputValues(float[] inputs)
//    {
//        for (int i = 0; i < neurons.Length; i++)
//        {
//            Neuron neuron = neurons[i];
//            neuron.value = inputs[i];
//        }
//    }

//    public float[] BackPropagateLayer(float[] expected)
//    {
//        //https://mattmazur.com/2015/03/17/a-step-by-step-backpropagation-example/

//        // Calculate the error
//        totalError = 0;
//        for (int i = 0; i < outputs.Length; i++)
//        {
//            OutputData output = outputs[i];

//            // Set the desired value to its expected value
//            output.desiredValue = expected[i];

//            // Calculate how changes in the output value change the error, 1/2(a-y)^2 || 2(a-y)?
//            totalError += (float)Math.Pow(output.value - output.desiredValue, 2) * 0.5f;

//            // Calculate the partial derivative of error with respect to output value
//            output.error = output.value - output.desiredValue;

//            float val = SigmoidDerivative(output.value);

//            //for (int j = 0; j < ne; j++)
//            //{

//            //}

//            outputs[i] = output;
//        }

//        for (int i = 0; i < outputs.Length; i++)
//        {
//            OutputData output = outputs[i];

//            outputs[i] = output;
//        }

//        for (int i = 0; i < outputs.Length; i++)
//        {
//            OutputData output = outputs[i];

//            // Set the desired value to its expected value
//            output.desiredValue = expected[i];

//            // honestly cant remember where i got this from
//            output.biasNudge += SigmoidDerivative(output.value) * output.error;

//            for (int j = 0; j < neurons.Length; j++)
//            {
//                Neuron neuron = neurons[i];
//                Connection connection = neuron.forwardConnections[i];

//                // How should 'w' change for 'a' to change in such a way that 'error' decreases
//                // 'a' might actually be the output value in this case, i dont rememner
//                connection.weightNudge = neuron.value * output.error;
                
//                // Update the connections
//                neuron.forwardConnections[i] = connection;
//            }
//            outputs[i] = output;
//        }
//        // Use the output values for the next expected
//        return outputs.Select(o => o.value).ToArray();
//    }

//    public void ConnectLayer(Layer otherLayer)
//    {
//        Neuron[] otherNeurons = otherLayer.neurons;
//        for (int i = 0; i < neurons.Length; i++)
//        {
//            Neuron currentNeuron = neurons[i];
//            currentNeuron.forwardConnections = new Connection[otherNeurons.Length];
//            for (int j = 0; j < otherNeurons.Length; j++)
//            {
//                Neuron otherNeuron = otherNeurons[j];
//                float weight = random == null ? 0.0f : (float)random.NextDouble() - 0.5f;
//                currentNeuron.ForwardsConnectNeuron(otherNeuron, j, weight);
//                otherNeuron.BackwardsConnectNeuron(currentNeuron, i, weight);
//            }
//        }
//    }

//    static float Sigmoid(float x)
//    {
//        return 1.0f / (1.0f + (float)Mathf.Exp(-x));
//    }

//    static float SigmoidDerivative(float x)
//    {
//        return x * (1 - x);
//    }

//    public void UpdateWeights()
//    {
//        for (int i = 0; i < outputs.Length; i++)
//        {
//            OutputData output = outputs[i];
//            output.desiredValue = 0;
//            output.error = 0;
//            output.bias += output.biasNudge;
//            output.biasNudge = 0;
//            for (int j = 0; j < neurons.Length; j++)
//            {
//                Neuron neuron = neurons[j];
//                Connection connection = neuron.forwardConnections[i];
//                connection.weight += connection.weightNudge;
//                connection.weightNudge = 0;
//                neuron.forwardConnections[i] = connection;
//            }
//            outputs[i] = output;
//        }
//    }
//}

//public class Neuron
//{
//    public float value;
//    public Connection[] forwardConnections;
//    public Connection[] backConnections;

//    // Creates a connection to the other neuron, if random is null the weight will be 0, returns the wait used in the connection
//    public void ForwardsConnectNeuron(Neuron otherNeuron, int index, float weight)
//    {
//        forwardConnections[index] = new Connection(this, otherNeuron, weight);
//    }
    
//    public void BackwardsConnectNeuron(Neuron otherNeuron, int index, float weight)
//    {
//        backConnections[index] = new Connection(this, otherNeuron, weight);
//    }
//}

//public struct Connection
//{
//    public Neuron left;
//    public Neuron right;
//    public float weight;
//    public float weightNudge;

//    public Connection(Neuron _left, Neuron _right, float _weight)
//    {
//        weightNudge = 0;
//        left = _left;
//        right = _right;
//        weight = _weight;
//    }
//}

//public struct OutputData
//{
//    public float value;
//    public float desiredValue;
//    public float bias;
//    public float biasNudge;
//    public float error;
//}
