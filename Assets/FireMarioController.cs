﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class FireMarioController : MonoBehaviour
{

    public enum AIState
    {
        Patrol,
        Chase
    };

    public AIState aiState;

    public GameObject[] waypoints;
    public int currWaypoint;
    public NavMeshAgent agent;
    public Animator anim;

    public GameObject movingTarget;
    public VelocityReporter movingTargetScript;

    // Use this for initialization
    void Start()
    {
        aiState = AIState.Patrol;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currWaypoint = -1;
        setNextWaypoint();

        movingTargetScript = movingTarget.GetComponent<VelocityReporter>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (aiState)
        {
            case AIState.Patrol:
                if (!agent.pathPending && agent.remainingDistance == 0)
                {
                    float dist1 = (movingTarget.transform.position - agent.transform.position).magnitude;
                    if (dist1 <= 50) // agent close enough to Koopa, chase Koopa
                    {
                        aiState = AIState.Chase;
                        break;
                    }

                    setNextWaypoint();
                }
                break;

            case AIState.Chase:
                float dist2 = (movingTarget.transform.position - agent.transform.position).magnitude;
                float lookAheadT = dist2 / agent.speed;
                Vector3 futureTarget = movingTarget.transform.position + lookAheadT * movingTargetScript.velocity;
                agent.SetDestination(futureTarget);

                if (dist2 > 50) // agent far enough from Koopa, go back to patrolling
                {
                    aiState = AIState.Patrol;
                    setNextWaypoint();
                }

                break;
        }
    }

    private void setNextWaypoint()
    {
        if (waypoints.Length == 0)
        {
            return;
        }
        currWaypoint = (currWaypoint + 1) % waypoints.Length;
        agent.SetDestination(waypoints[currWaypoint].transform.position);
    }
}
