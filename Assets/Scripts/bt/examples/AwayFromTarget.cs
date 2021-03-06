﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;


public class AwayFromTarget : BtNode {
    private NavMeshAgent m_agent;
    public GameObject Player;
    public float GhostFlee = 20.0f;

    public override NodeState evaluate(Blackboard blackboard) {
        if (m_agent == null) {
            m_agent = blackboard.owner.GetComponent<NavMeshAgent>();
        }

        // if target is null, we can't move towards it!
        if (blackboard.target == null) {
            return NodeState.FAILURE;
        }

        m_agent.SetDestination(blackboard.target.transform.position);
        Debug.Log("Agent: " + blackboard.owner.name + ", Target: " + blackboard.target.name);
        if ( Vector3.Distance(blackboard.owner.transform.position, blackboard.target.transform.position) < 20)
        {
            return NodeState.RUNNING;
        }

        return NodeState.SUCCESS;
    }

    

    public override string getName()
    {
        return "AwayFromTarget";
    }

}
