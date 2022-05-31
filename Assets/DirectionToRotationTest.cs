using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionToRotationTest : MonoBehaviour
{
    List<Vector3> directions = new List<Vector3>() { Vector3.back, Vector3.forward, Vector3.left, Vector3.right};
    void Start()
    {
        Vector3 direction = directions[Random.Range(0, directions.Count)];
        transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(transform.forward, direction, Vector3.up), 0);
        Debug.Log(transform.rotation.eulerAngles);
    }

    void Update()
    {
        
    }
}
