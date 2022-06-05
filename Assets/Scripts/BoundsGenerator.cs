using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsGenerator : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    public Vector3 size = Vector3.one;

    //https://answers.unity.com/questions/49860/creating-a-bounding-box-for-multiple-objects-using.html
    void Start()
    {
        Bounds bounds = Encap(transform, new Bounds());
        offset = bounds.center;
        size = bounds.size;
    }

    Bounds Encap(Transform parent, Bounds blocker)
    {
        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            
            if (renderer != null)
                blocker.Encapsulate(renderer.bounds);
            blocker = Encap(child, blocker);
        }
        return blocker;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(offset, size);
    }
}
