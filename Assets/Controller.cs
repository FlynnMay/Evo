using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Brain brain;

    void Awake()
    {
        brain = GetComponent<Brain>();
    }

    void Start()
    {

    }
}
