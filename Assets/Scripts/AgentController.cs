using System.Linq;
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
    List<Checkpoint> checkpoints;
    void Start()
    {
        checkpoints = FindObjectsOfType<Checkpoint>().ToList();
        meshRenderer = GetComponent<MeshRenderer>();
        timer = timerMax;
        //StartCoroutine(LerpFunction(Quaternion.Euler(targetRotation), 1));
        agent = GetComponent<EvolutionAgent>();

        startingPosition = transform.position;
        startingRotation = transform.rotation;

        agent.onResetEvent.AddListener(() =>
        {
            foreach (Checkpoint checkpoint in checkpoints.Where(c => c.Found(agent)))
                checkpoint.Remove(agent);

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
        transform.localScale = Vector3.one / 2 + (agent.IsElite ? Vector3.up * 0.25f : Vector3.zero);

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            Checkpoint check = other.GetComponent<Checkpoint>();
            Vector3 dir = (transform.position - other.transform.position).normalized;
            float dot = Vector3.Dot(dir, other.transform.forward);

            if (dot > 0.0f)
            {
                if (!check.Found(agent))
                {
                    agent.Reward(2);
                    check.Add(agent);
                }
            }
            else if (dot < 0.0f)
            {
                if (check.Found(agent))
                {
                    agent.Penalise(2);
                    check.Remove(agent);
                }
            }
        }
    }
}
