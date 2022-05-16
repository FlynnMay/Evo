using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : MonoBehaviour
{
    BackpropagationNeuralNetwork brain;
    void Start()
    {
        brain = new BackpropagationNeuralNetwork(new int[] {2, 2, 2});
    }

    // Update is called once per frame
    void Update()
    {
        //brain.Train()
    }
}
