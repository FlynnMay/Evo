using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Neural;

public class Tester : MonoBehaviour
{
    System.Random random;
    public GameObject neuralUI;
    public GameObject layerPrefab;
    public LineRenderer lineRendererPrefab;
    public TextBubble neuronPrefab;
    public TextBubble weightPrefab;
    public TextBubble epoch;
    public TextBubble error;
    [Range(0, 2)]
    public float timeBetweenFeeding = 0.1f;
    NooralNet neuralNetwork;
    public EvolutionValueType EvoType { get; set; } = EvolutionValueType.EvoInt;

    Dictionary<Layer, GameObject> layerObjectPairs = new Dictionary<Layer, GameObject>();
    Dictionary<Neuron, TextBubble> neuronObjectPairs = new Dictionary<Neuron, TextBubble>();
    Dictionary<Neuron, List<TextBubble>> weightObjectPairs = new Dictionary<Neuron, List<TextBubble>>();

    void Start()
    {
        StartCoroutine(NeuralTest());
    }

    void Update()
    {
        error.textMesh.text = neuralNetwork.GetTotalError().ToString();
        foreach (var neuronObj in neuronObjectPairs)
        {
            Neuron neuron = neuronObj.Key;
            TextBubble obj = neuronObj.Value;

            obj.textMesh.text = neuron.input.ToString();

            if (neuron.connections == null)
                continue;
            int i = 0;
            foreach (var connection in neuron.connections)
            {
                weightObjectPairs[neuron][i++].textMesh.text = connection.weight.ToString();
            }
        }
    }

    private IEnumerator NeuralTest()
    {
        neuralNetwork = new NooralNet(2, new int[] { }, 1);

        Layer[] layers = neuralNetwork.layers;

        for (int i = 0; i < layers.Length; i++)
        {
            Layer layer = layers[i];
            GameObject obj = Instantiate(layerPrefab, neuralUI.transform);
            layerObjectPairs.Add(layer, obj);
            Neuron[] neurons = layer.neurons;
            for (int j = 0; j < neurons.Length; j++)
            {
                Neuron neuron = neurons[j];
                TextBubble neuronObj = Instantiate(neuronPrefab, obj.transform);
                neuronObjectPairs.Add(neuron, neuronObj);
            }
        }

        yield return new WaitForEndOfFrame();

        foreach (var neuronObj in neuronObjectPairs)
        {
            Neuron neuron = neuronObj.Key;
            TextBubble obj = neuronObj.Value;

            obj.textMesh.text = neuron.value.ToString();

            if (neuron.connections == null)
                continue;

            weightObjectPairs.Add(neuron, new List<TextBubble>());
            foreach (var connection in neuron.connections)
            {
                LineRenderer lineRenderer = Instantiate(lineRendererPrefab, obj.transform);

                TextBubble otherObj = neuronObjectPairs[connection.right];

                Vector3 objPos = obj.transform.position;
                Vector3 otherPos = otherObj.transform.position;

                TextBubble weight = Instantiate(weightPrefab);
                weight.transform.position = (objPos + otherPos) / 2;
                weight.transform.SetParent(obj.transform);

                weightObjectPairs[neuron].Add(weight);

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(lineRenderer.positionCount - 2, objPos);
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, otherPos);
            }
        }

        Debug.LogWarning($"New Network");

        for (int i = 0; i < 10000; i++)
        {
            epoch.textMesh.text = (i + 1).ToString();
            neuralNetwork.FeedForward(new float[] { 0, 0 });
            neuralNetwork.BackPropagate(new float[] { 0 });

            if (timeBetweenFeeding != 0)
                yield return new WaitForSeconds(timeBetweenFeeding);

            neuralNetwork.FeedForward(new float[] { 1, 1 });
            neuralNetwork.BackPropagate(new float[] { 1 });

            if (timeBetweenFeeding != 0)
                yield return new WaitForSeconds(timeBetweenFeeding);

            neuralNetwork.FeedForward(new float[] { 0, 1 });
            neuralNetwork.BackPropagate(new float[] { 1 });

            if (timeBetweenFeeding != 0)
                yield return new WaitForSeconds(timeBetweenFeeding);

            neuralNetwork.FeedForward(new float[] { 1, 0 });
            neuralNetwork.BackPropagate(new float[] { 1 });

            if (timeBetweenFeeding != 0)
                yield return new WaitForSeconds(timeBetweenFeeding);
        }

        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1 })[0]);

        //for (int i = 0; i < 5000; i++)
        //{
        //    neuralNetwork.FeedForward(new float[] { 0, 0, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 0, 0, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 0, 1, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 0, 1, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 0, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //    neuralNetwork.FeedForward(new float[] { 1, 0, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 1, 0 });
        //    neuralNetwork.BackPropagate(new float[] { 0 });
        //    neuralNetwork.FeedForward(new float[] { 1, 1, 1 });
        //    neuralNetwork.BackPropagate(new float[] { 1 });
        //}

        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 1 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 0 })[0]);
        //Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 1 })[0]);
    }

    private static void GammaTest()
    {
        // 0 0 0 => 0
        // 0 0 1 => 1
        // 0 1 0 => 1
        // 0 1 1 => 0
        // 1 0 0 => 1
        // 1 0 1 => 0
        // 1 1 0 => 0
        // 1 1 1 => 1

        // number of inputs = 3
        // hidden layers 1 = 25 
        // hidden layers 2 = 25 
        // number of outputs = 1 

        GammaNeuralNetwork net = new GammaNeuralNetwork(new int[] { 3, 25, 25, 1 });
        net.FeedForward(new float[] { 0, 0, 0 });

        //for (int i = 0; i < 5000; i++)
        //{
        //    net.FeedForward(new float[] { 0, 0, 0 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 0, 0, 1 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 0, 1, 0 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 0, 1, 1 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 0, 0 });
        //    net.BackProp(new float[] { 1 });

        //    net.FeedForward(new float[] { 1, 0, 1 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 1, 0 });
        //    net.BackProp(new float[] { 0 });

        //    net.FeedForward(new float[] { 1, 1, 1 });
        //    net.BackProp(new float[] { 1 });
        //}

        //Debug.Log(net.FeedForward(new float[] { 0, 0, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 0, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 1, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 0, 1, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 0, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 0, 1 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 1, 0 })[0]);
        //Debug.Log(net.FeedForward(new float[] { 1, 1, 1 })[0]);
    }
}
