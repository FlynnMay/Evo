using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour
{
    EvolutionGroup group;
    [SerializeField] Transform target;

    public float timerMax = 3.0f;
    [ReadOnly][SerializeField] float timer = 0.0f;
    void Start()
    {
        timer = timerMax;
        group = GetComponent<EvolutionGroup>();
        group.CustomFitnessFunction = (g) =>
        {
            EvolutionAgent agent = group.GetAgentFromDNA(g);
            Vector3 pos = agent.transform.position;
            float dist = Vector3.Distance(pos, target.position);
            return 1 - dist;
        };

        group.CustomRandomFunction = () =>
        {
            return UnityEngine.Random.Range(-1.0f, 1.0f);
        };
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (group.agents.All(a => !a.IsAlive) || timer <= 0.0f)
        {
            timer = timerMax;
            group.EvolveGeneration();

            foreach (var agent in group.agents)
                agent.Reset();
        }
    }
}
