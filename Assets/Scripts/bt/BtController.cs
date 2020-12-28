using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BtController : MonoBehaviour
{
    private BtNode m_root;
    private Blackboard m_blackboard;

    private NavMeshAgent m_agent;
    public GameObject Player;
    public float GhostFlee = 20.0f;

    public BtNode wonderToPill()
    {
        BtNode isTargetSelected = new Sequence(new IsTargeting("pill"), new Inverter(new IsClose(1)));
        BtNode stickyTarget = new Selector(isTargetSelected, new TargetRandom("pill"));
        return new Sequence(stickyTarget, new AwayFromTarget());
    }

    protected BtNode createTree() 
    {
        BtNode isTargetSelected = new Sequence(new IsTargeting("pill"), new Inverter(new IsClose(1)));
        BtNode stickyTarget = new Selector(isTargetSelected, new TargetRandom("pill"));

        BtNode wonderToPill1 = wonderToPill();
        //BtNode chasePlayer = new Sequence(new IsTagClose(10, "Player"), new AwayFromTarget());
        return new Selector(wonderToPill1); // orig; chasePlayer
    }

    // Start is called before the first frame update
    void Start() 
    {
        if ( m_root == null) {
            m_root = createTree();
            m_blackboard = new Blackboard();
            m_blackboard.owner = gameObject;
        }

        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update() 
    {
        NodeState result = m_root.evaluate(m_blackboard);
        if ( result != NodeState.RUNNING ) {
            m_root.reset();
        }

        // Run away from player
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        if (distance < GhostFlee)
        {   
            Vector3 dirToPlayer = transform.position - Player.transform.position;
            Vector3 newPos = transform.position + dirToPlayer;

            m_agent.SetDestination(newPos);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "pill")
        {
            Destroy(col.gameObject);
        }
    }


}
