using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    void Start()
    {
        //GammaTest();
        NeuralTest();
        //NewralNet net = new NewralNet(3, new int[] { 25, 25 }, 1);
        //net.FeedForward(new float[] { 1, 0, 0 });

    }

    private static void NeuralTest()
    {
        NewralNet neuralNetwork = new NewralNet(3, new int[] { 25, 25 }, 1);

        Debug.Log($"Layers: {neuralNetwork.layers.Length}");

        neuralNetwork.FeedForward(new float[] { 0, 0, 0 });

        for (int i = 0; i < 5000; i++)
        {
            neuralNetwork.FeedForward(new float[] { 0, 0, 0 });
            neuralNetwork.BackPropagate(new float[] { 0 });
            neuralNetwork.FeedForward(new float[] { 0, 0, 1 });
            neuralNetwork.BackPropagate(new float[] { 1 });
            neuralNetwork.FeedForward(new float[] { 0, 1, 0 });
            neuralNetwork.BackPropagate(new float[] { 1 });
            neuralNetwork.FeedForward(new float[] { 0, 1, 1 });
            neuralNetwork.BackPropagate(new float[] { 0 });
            neuralNetwork.FeedForward(new float[] { 1, 0, 0 });
            neuralNetwork.BackPropagate(new float[] { 1 });
            neuralNetwork.FeedForward(new float[] { 1, 0, 1 });
            neuralNetwork.BackPropagate(new float[] { 0 });
            neuralNetwork.FeedForward(new float[] { 1, 1, 0 });
            neuralNetwork.BackPropagate(new float[] { 0 });
            neuralNetwork.FeedForward(new float[] { 1, 1, 1 });
            neuralNetwork.BackPropagate(new float[] { 1 });
        }

        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 0, 1 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 0, 1, 1 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 0, 1 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 0 })[0]);
        Debug.Log(neuralNetwork.FeedForward(new float[] { 1, 1, 1 })[0]);
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

    void Update()
    {

    }
}
