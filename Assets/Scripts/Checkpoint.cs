using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

public class Checkpoint : MonoBehaviour
{
    List<EvolutionAgent> agents = new List<EvolutionAgent>();

    public void Add(EvolutionAgent agent)
    {
        agents.Add(agent);
    }

    public void Remove(EvolutionAgent agent)
    {
        agents.Remove(agent);
    }

    public bool Found(EvolutionAgent agent)
    {
        return agents.Contains(agent);
    }
}
