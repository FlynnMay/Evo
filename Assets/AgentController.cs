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
    float timer = 0;
    Quaternion startingRotation;
    Vector3 startingPosition;
    Vector3 targetRotation;

    void Start()
    {
        timer = timerMax;
        //StartCoroutine(LerpFunction(Quaternion.Euler(targetRotation), 1));
        agent = GetComponent<EvolutionAgent>();
        
        startingPosition = transform.position;
        startingRotation = transform.rotation;

        agent.onResetEvent.AddListener(() =>
        {
            rotIndex = 1;
            timer = timerMax;
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

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = timerMax;
            rotIndex++;
        }

        if(rotIndex > genes.Length - 1)
        {
            agent.IsAlive = false;
            return;
        }

        transform.rotation = Quaternion.Euler(0, genes[rotIndex].GetEvolutionValue<float>() * 360, 0);
        speed = genes[0].GetEvolutionValue<float>();

        agent.transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        agent.IsAlive = !other.CompareTag("KillBox");
    }
}
