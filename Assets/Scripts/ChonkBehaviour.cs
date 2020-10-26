using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChonkBehaviour : MonoBehaviour
{
    public enum State { WanderingInBedroom, Leaving }
    public State state;
    public Room room;
    public float minWait;
    public float maxWait;

    [SerializeField]
    private float waitTimer;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.WanderingInBedroom:
                WanderInBedroom();
                break;
        }


    }

    public void WanderInBedroom()
    {
        if (waitTimer > 0 && Vector3.Distance(transform.position,agent.destination) < agent.stoppingDistance + agent.height / 1.9f)
        {
            waitTimer -= Time.deltaTime;
        }
        else if (waitTimer > 0)
        {
            return;
        }
        else
        {
            agent.SetDestination(room.grid.RandomNode().transform.position);
            waitTimer = Random.Range(minWait, maxWait);
        }
    }
}
