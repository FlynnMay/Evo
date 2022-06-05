using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
public class Brain : MonoBehaviour
{
    //public int inputCount;
    //public int outputCount;
    //public int[] hiddenCounts;
    
    //NeuralNet net;
    //Random random;
    
    //void Awake()
    //{
    //    random = new Random();
    //    net = new NeuralNet(inputCount, hiddenCounts, outputCount, random);       
    //}

    //void Update()
    //{
        
    //}

    //public float[] Think(float[] inputs)
    //{
    //    if (inputs.Length != inputCount)
    //    {
    //        Debug.LogError("[Brain] Inputs length needs to equal inputCount");
    //        return null;
    //    }
    //    return net.FeedForward(inputs);
    //}
    
    //public void Learn(float[] inputs, float[] expectedOutputs)
    //{
    //    if (inputs.Length != inputCount)
    //    {
    //        Debug.LogError("[Brain] Inputs length needs to equal inputCount");
    //        return;
    //    }
        
    //    if (expectedOutputs.Length != outputCount)
    //    {
    //        Debug.LogError("[Brain] Expected output length needs to equal outputCount");
    //        return;
    //    }
        
    //    net.FeedForward(inputs);
    //    net.BackPropagate(expectedOutputs);
    //}
}
