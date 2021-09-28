using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroller : EnemyAI
{
    public override void Start()
    {
        DefaultBehaviour = new State_Patrol(this);
    }

    private void OnTriggerStay(Collider other)
    {
        //is it the player?
        if (other.gameObject.tag.Equals("Player"))
        {
            // angle between us and the player
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // reset whether we’ve seen the player
            playerSeen = false;
            RaycastHit hit;

            // is it less than our field of view
            if (angle < viewFov * 0.5f)
            {
                // if the raycast hits the player we know
                // there is nothing in the way
                // adding transform.up raises up from the floor by 1 unit
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, GetComponent<BoxCollider>().size.x))
                {
                    if (hit.collider.gameObject == player)
                    {
                        // flag that we've seen the player
                        // remember their position
                        playerSeen = true;
                        lastSeen = player.transform.position;
                    }
                }
            }
        }
    }

}