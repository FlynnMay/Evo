using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    EvolutionAgent agent;
    [SerializeField]
    float speed = 1;

    Quaternion startingRotation;
    Vector3 startingPosition;

    void Start()
    {
        agent = GetComponent<EvolutionAgent>();

        startingPosition = transform.position;
        startingRotation = transform.rotation;
        
        agent.onResetEvent.AddListener(() =>
        {
            transform.position = startingPosition;
            transform.rotation = startingRotation;
            agent.IsAlive = true;
        });
    }

    void Update()
    {
        if (!agent.IsAlive || agent.DNA == null)
            return;

        EvolutionValue[] genes = agent.DNA.Genes;
        Vector3 dir = new Vector3(genes[0].GetEvolutionValue<float>(), 0, genes[1].GetEvolutionValue<float>());
        agent.transform.position += dir * speed * Time.deltaTime;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        agent.IsAlive = !other.CompareTag("KillBox");
    }
}
