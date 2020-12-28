using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTagClose : BtNode
{
    private float m_distanceLimit = 10;
    private string m_tag;

    public IsTagClose(float distanceLimit, string tag)
    {
        m_distanceLimit = distanceLimit;
        m_tag = tag;
    }

    public override NodeState evaluate(Blackboard blackboard)
    {
        double closeDist = double.PositiveInfinity;
        GameObject closest = null;

        GameObject[] objects = GameObject.FindGameObjectsWithTag(m_tag);
        foreach (GameObject obj in objects)
        {
            double distance = Vector3.Distance(obj.transform.position, blackboard.owner.transform.position);
            if ( distance < closeDist)
            {
                closeDist = distance;
                closest = obj;
            }
        }

        if ( closeDist <= m_distanceLimit && closest != null) {
            blackboard.target = closest;
            return NodeState.SUCCESS;
        } else {
            return NodeState.FAILURE;
        }
       
    }

    public override string getName()
    {
        return "isTagClose";
    }

}

