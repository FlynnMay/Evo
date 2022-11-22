using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedStartTest : MonoBehaviour
{
    [SerializeField] Evo.EvolutionGroup group;
    [SerializeField] float delay = 1.0f;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(delay);
        group.StartEvolving();
    }
}
