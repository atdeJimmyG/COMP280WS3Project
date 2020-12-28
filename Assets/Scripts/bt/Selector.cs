using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behaviour tree implementation adapted from: https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/

public class Selector : BtNode
{
    protected List<BtNode> m_nodes;
    protected int runningNode;

    public Selector(List<BtNode> nodes)
    {
        m_nodes = nodes;
        reset();
    }

    public Selector(params BtNode[] nodes)
    {
        m_nodes = new List<BtNode>();
        foreach (BtNode node in nodes)
        {
            m_nodes.Add(node);
        }
        reset();
    }

    public override IEnumerable<BtNode> children()
    {
        return m_nodes;
    }

    public override void reset()
    {
        m_nodeState = NodeState.RUNNING;
        runningNode = 0;

        foreach (BtNode node in m_nodes)
        {
            node.reset();
        }
    }

    public override NodeState evaluate(Blackboard board) {
        return evaluateStep(board);
    }

    // JWR - broken experiment - ignore
    public NodeState evaluateCache(Blackboard board)
    {
        if (m_nodeState != NodeState.RUNNING) {
            return m_nodeState;
        }

        while (runningNode < m_nodes.Count)
        {
            BtNode node = m_nodes[runningNode];
            NodeState nodeStatus = node.evaluate(board);

            // all must be success
            if (nodeStatus == NodeState.FAILURE)
            {
                runningNode++;
                continue;
            }
            else if (nodeStatus == NodeState.SUCCESS)
            {
                m_nodeState = NodeState.SUCCESS;
                return m_nodeState;
            }
            else if (nodeStatus == NodeState.RUNNING)
            {
                m_nodeState = NodeState.RUNNING;
                return m_nodeState;
            }
        }

        // nothing worked :(
        m_nodeState = NodeState.FAILURE;
        return m_nodeState;
    }

    public NodeState evaluateStep(Blackboard blackboard)
    {
        foreach (BtNode node in m_nodes)
        {
            switch (node.evaluate(blackboard))
            {
                case NodeState.SUCCESS:
                    m_nodeState = NodeState.SUCCESS;
                    return m_nodeState;
                case NodeState.FAILURE:
                    continue;
                case NodeState.RUNNING:
                    m_nodeState = NodeState.RUNNING;
                    return m_nodeState;
                default:
                    continue;
            }
        }

        m_nodeState = NodeState.FAILURE;
        return m_nodeState;
    }

    public override string getName()
    {
        return "Selector";
    }

}
