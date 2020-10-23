using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChonkBehaviour : MonoBehaviour
{
    public enum State { WanderingInBedroom }
    public State state;
    public Room room;
    public float minWait;
    public float maxWait;

    private float waitUntil;

    private Node targetNode;

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
        //if(Time.time < waitUntil && Vector3.Distance(transform.position, ))
        //{
        //    return;
        //}
        //else
        //{

        //}
    }
}
