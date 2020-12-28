using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behaviour tree implementation adapted from: https://hub.packtpub.com/building-your-own-basic-behavior-tree-tutorial/

public class Sequence : BtNode {
    private List<BtNode> m_nodes;
    private int runningNode;

    public Sequence(List<BtNode> nodes) {
        m_nodes = nodes;
        reset();
    }

    public Sequence(params BtNode[] nodes) {
        m_nodes = new List<BtNode>();
        foreach (BtNode node in nodes) {
            m_nodes.Add(node);
        }
        reset();
    }

    public override IEnumerable<BtNode> children()
    {
        return m_nodes;
    }

    public override void reset() {
        m_nodeState = NodeState.RUNNING;
        runningNode = 0;

        foreach (BtNode node in m_nodes) {
            node.reset();
        }
    }

    public override NodeState evaluate(Blackboard blackboard) {
        return evaluateStep(blackboard);
    }

    public NodeState evaluateCache(Blackboard blackboard)
    {
        // already fired
        if ( m_nodeState != NodeState.RUNNING ) {
            Debug.Log( m_nodeState );
            return m_nodeState;
        }

        // check remaining nodes
        while (runningNode < m_nodes.Count) {
            BtNode node = m_nodes[runningNode];
            NodeState nodeStatus = node.evaluate( blackboard );
            
            // all must be success
            if ( nodeStatus == NodeState.FAILURE) {
                m_nodeState = NodeState.FAILURE;
                return m_nodeState;
            } else if ( nodeStatus == NodeState.SUCCESS) {
                runningNode++;
            } else if ( nodeStatus == NodeState.RUNNING) {
                m_nodeState = NodeState.RUNNING;
                return m_nodeState;
            }
        }

        m_nodeState = NodeState.SUCCESS;
        return m_nodeState;
    }

    public NodeState evaluateStep(Blackboard blackboard) {
        bool anyChildRunning = false;

        foreach( BtNode node in m_nodes) {
            switch ( node.evaluate(blackboard) ) {
                case NodeState.FAILURE:
                    m_nodeState = NodeState.FAILURE;
                    return m_nodeState;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildRunning = true;
                    continue;
                default:
                    m_nodeState = NodeState.SUCCESS;
                    return m_nodeState;
            }
        }

        m_nodeState = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return m_nodeState;
    }

    public override string getName()
    {
        return "Sequence";
    }
}
