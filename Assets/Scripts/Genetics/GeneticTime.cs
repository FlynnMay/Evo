using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticTime : MonoBehaviour
{
    public static GeneticTime instance;
    public static float timeScale = 1.0f;
    public static float deltaTime;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        deltaTime = Time.deltaTime * timeScale;        
    }
}
