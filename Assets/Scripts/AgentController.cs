using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    EvolutionAgent agent;
    [SerializeField]
    float speed = 1;
    int rotIndex = 1;
    public float timerMax = 0.5f;
    float timer = 0.0f;
    float lifeTime = 0.0f;
    Quaternion startingRotation;
    Vector3 startingPosition;
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        timer = timerMax;
        //StartCoroutine(LerpFunction(Quaternion.Euler(targetRotation), 1));
        agent = GetComponent<EvolutionAgent>();

        startingPosition = transform.position;
        startingRotation = transform.rotation;

        agent.onResetEvent.AddListener(() =>
        {
            rotIndex = 1;
            timer = timerMax;
            lifeTime = 0.0f;
            transform.position = startingPosition;
            transform.rotation = startingRotation;
        });
    }

    void Update()
    {
        if (!agent.IsAlive || agent.DNA == null)
            return;

        meshRenderer.material.color = agent.IsElite ? Color.green : Color.blue;
        meshRenderer.material.color = agent.IsKing ? Color.magenta : meshRenderer.material.color;
        //meshRenderer.enabled = agent.IsElite;

        object[] genes = agent.DNA.Genes;

        lifeTime += GeneticTime.deltaTime;
        timer -= GeneticTime.deltaTime;

        if (timer < 0)
        {
            timer = timerMax;
            rotIndex++;
        }

        if (rotIndex > genes.Length - 1)
        {
            agent.IsAlive = false;
            return;
        }

        transform.rotation = Quaternion.Euler(0, (float)genes[rotIndex] * 360, 0);
        speed = (float)genes[0];

        agent.transform.position += transform.forward * speed * GeneticTime.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillBox"))
        {
            agent.IsAlive = false;
            agent.Penalise();

        }
        else if (other.CompareTag("Destination"))
        {
            agent.IsAlive = false;
        }
        
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            Vector3 dir = (transform.position - other.transform.position).normalized;
            float dot = Vector3.Dot(dir, other.transform.forward);
            if (dot > 0.0f)
                agent.Reward();
            else if (dot < 0.0f)
                agent.Penalise();
        }
    }
}
