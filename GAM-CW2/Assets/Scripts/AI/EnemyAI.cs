using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public StateMachine stateMachine = new StateMachine();
    public Enemy e;
    public IState DefaultBehaviour;

    public bool Aggroed = false;
    public bool StopMoving = false;
    public Vector3 TargetDestination;

    public float Speed = 5f;
    public bool AggroSightRequired = true;
    public float AggroRange = 30f;
    public float DeaggroRange = 0f;

    public float viewFov = 100f;
    public bool playerSeen = false;
    public bool attacking = false;
    public bool rotateToPlayer = false;
    public Vector3 lastSeen;
    public Animator animator;
    public Waypoint waypoint;

    public GameObject player;
    private bool linking = false;
    private NavMeshAgent agent;

    public virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        e = GetComponent<Enemy>();

        stateMachine.ChangeState(new State_Idle(this));
    }

    protected void Update()
    {
        stateMachine.Update();
        if (agent.enabled && Aggroed)
        {
            agent.isStopped = StopMoving;
            if (TargetDestination != null) agent.destination = TargetDestination;
        }

        //Rotate towards player if necessary (for aiming attacks)
        if (rotateToPlayer)
        {
            Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - gameObject.transform.position);
            float str = Mathf.Min(5f * Time.deltaTime, 5f);
            gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, targetRotation, str);
        }

        /*
        //Don't snap to navmesh if we're being knocked back
        if (!UseNavmesh)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
        }
        */
    }

    public Vector3 PlayerLocation()
    {
        return player.transform.position;
    }

    protected void FixedUpdate()
    {
        //Jumping
        if (agent.isOnOffMeshLink && linking == false)
        {
            linking = true;
            agent.speed = 0.1f;
        }
        else if (agent.isOnNavMesh && linking == true)
        {
            linking = false;
            agent.velocity = Vector3.zero;
            agent.speed = Speed;
        }
        else
        {
            agent.speed = Speed;
        }

        float d = Vector3.Distance(transform.position, player.transform.position);

        if (!Aggroed)
        {
            //Aggro if player close
            if (AggroRange == 0f || d <= AggroRange)
            {
                Aggro();
            }
        }
        else
        {
            //De-aggro if player is far away
            if (DeaggroRange > 0f && d >= DeaggroRange)
            {
                Deaggro();
            }
        }


        
        
    }

    public void Aggro()
    {
        Aggroed = true;
        stateMachine.ChangeState(DefaultBehaviour);
    }

    public void Deaggro()
    {
        Aggroed = false;
    }
}