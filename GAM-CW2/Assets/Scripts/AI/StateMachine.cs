using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IState
{
    void Enter();
    void Execute();
    void Exit();
}

public class StateMachine
{
    private IState currentState;

    public void ChangeState(IState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null) currentState.Execute();
    }
}

public class State_Idle : IState
{
    private EnemyAI owner;

    public State_Idle(EnemyAI owner) { this.owner = owner; }

    public void Enter()
    {
        owner.animator.SetBool("Moving", false);
    }

    public void Execute()
    {
    }

    public void Exit()
    {
        
    }
}

public class State_Patrol : IState
{
    private EnemyAI owner;
    private Waypoint waypoint;
    private NavMeshAgent agent;

    public State_Patrol(EnemyAI owner) { this.owner = owner; }

    public void Enter()
    {
        waypoint = owner.waypoint;
        agent = owner.GetComponent<NavMeshAgent>();
        agent.destination = waypoint.transform.position;

        agent.isStopped = false;
        owner.animator.SetBool("Moving", true);
    }

    public void Execute()
    {
        //Go to next waypoint if current one reached
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waypoint = waypoint.NextWaypoint;
            agent.destination = waypoint.transform.position;
        }

        if (owner.playerSeen)
        {
            owner.stateMachine.ChangeState(new State_Attack(owner));
        }
    }

    public void Exit()
    {
        agent.isStopped = true;
    }
}

//Approaches the player as straightforwardly as possible and attacks from the preferred range
public class State_Attack : IState
{
    private EnemyAI owner;
    private NavMeshAgent agent;

    private bool approxPathFound = false;

    public State_Attack(EnemyAI owner) { this.owner = owner; }

    public void Enter()
    {
        agent = owner.GetComponent<NavMeshAgent>();

        if (owner.playerSeen) {
            owner.StopMoving = false;
            owner.TargetDestination = owner.lastSeen;
        }
    }

    public void Execute()
    {
        if (agent.enabled)
        {
            //Track the player's last known location on the (accessible) navmesh
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(owner.PlayerLocation(), path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    //Debug.Log("Player location known");

                    //Enemy always knows player location
                    owner.playerSeen = true;
                    owner.lastSeen = owner.PlayerLocation();

                    approxPathFound = false;
                }
                else if (!approxPathFound)
                {
                    /*
                    //Otherwise find a close nearby location to go to, from which it can shoot
                    NavMeshHit hit;
                    for (int i = 0; i < 30; i++)
                    {
                        if (NavMesh.SamplePosition(owner.lastSeen, out hit, 5.0f, agent.areaMask))
                        {
                            //Debug.Log("Player location no longer known, going to last known location: " + hit.position);
                            owner.lastSeen = hit.position;
                            approxPathFound = true;
                            break;
                        }
                    }
                    */
                }
            }
        }

        owner.StopMoving = false;
        owner.TargetDestination = owner.lastSeen;

        float playerDist = Vector3.Distance(owner.transform.position, owner.PlayerLocation());

        //Melee attack if close
        if (owner.e.MeleeDistance != 0 && playerDist <= owner.e.MeleeDistance)
        {
            owner.rotateToPlayer = true;
            owner.StopMoving = true;
            owner.animator.SetBool("Ranged", false);
            owner.animator.SetBool("Melee", true);
        }
        //Ranged attack if further away
        else if (owner.e.ShootDistance != 0 && playerDist <= owner.e.ShootDistance)
        {
            if (agent.enabled && !agent.isOnOffMeshLink)
            {
                //Check for line of sight (if needed)
                RaycastHit hit;
                if (!owner.AggroSightRequired ||
                    Physics.Raycast(owner.transform.position, owner.PlayerLocation() - owner.transform.position, out hit) && hit.transform.tag == "Player")
                {
                    owner.StopMoving = true;
                    owner.rotateToPlayer = true;
                    owner.animator.SetBool("Melee", false);
                    owner.animator.SetBool("Ranged", true);
                }
            }
        }
        //Move closer if in range for neither
        else
        {
            owner.StopMoving = false;
            owner.attacking = false;
            owner.rotateToPlayer = false;
            owner.animator.SetBool("Melee", false);
            owner.animator.SetBool("Ranged", false);
        }

        //Play jumping animation if at a jump point:
        if (agent.enabled && agent.isOnOffMeshLink)
        {
            owner.animator.SetTrigger("Jump");
        }

        //If agent stopped, Moving = false
        owner.animator.SetBool("Moving", !owner.StopMoving);
    }

    public void Exit()
    {
        owner.StopMoving = true;
    }
}


//Circles around the player, either to approach from behind and melee or shoot from varied locations
public class State_Circle : IState
{
    private EnemyAI owner;
    private NavMeshAgent agent;

    public State_Circle(EnemyAI owner) { this.owner = owner; }

    public Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {
        Vector2 o = new Vector2(origin.x, origin.z);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        float randomDistance = Random.Range(minRadius, maxRadius);
        Vector2 point = o + randomDirection * randomDistance;

        Vector3 point3 = new Vector3(point.x, 0f, point.y);

        return point;
    }

    public void Enter()
    {
        agent = owner.GetComponent<NavMeshAgent>();

        owner.animator.SetBool("Moving", true);
        owner.lastSeen = owner.player.transform.position;

        //Pick random point to move to
        if (owner.playerSeen)
        {
            Vector3 randomPoint = RandomPointInAnnulus(owner.lastSeen, owner.e.ShootDistance, owner.e.ShootDistance * 2f);

            //Get a nearby point that is on the navmesh
            /*
            Vector3 validPoint = randomPoint;
            NavMeshHit hit;
            for (int i = 0; i < 30; i++)
            {
                if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, agent.areaMask))
                {
                    Debug.Log("Destination found");
                    validPoint = hit.position;
                    break;
                }
            }*/

            owner.TargetDestination = randomPoint;
            owner.StopMoving = false;
        }
    }

    public void Execute()
    {
        if (agent.enabled && owner.playerSeen)
        {
            //Stop at chosen point, then start attacking
            if (agent.remainingDistance < 0.5f)
            {
                owner.stateMachine.ChangeState(new State_Attack(owner));
            }

            //If close enough to melee, then transition to Attacking
            if (owner.e.MeleeDistance != 0 && agent.remainingDistance < owner.e.MeleeDistance)
            {
                owner.stateMachine.ChangeState(new State_Attack(owner));
            }
        }

        //If agent stopped, Moving = false
        owner.animator.SetBool("Moving", !agent.isStopped);
    }

    public void Exit()
    {
        owner.StopMoving = true;
    }
}


//Dragon Boss! Special States

