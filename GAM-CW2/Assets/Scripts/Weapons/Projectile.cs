using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public bool PlayerOwned = true;

    public float Damage = 5f;
    public float Radius = 16f;
    public float ImpactForce = 20f;
    public float DamageFalloff = 0f;
    public int MaxBounces = 0;
    public float TimeToExplode = 0f;
    public bool ExplodeOnEnemyContact = true;
    public bool ExplodeOnGroundContact = true;
    public bool SelfDamaging = false;

    public AudioClip hitsound;
    public GameObject impactEffect;

    private int bounces = 0;
    private float spawnTime;

    void Start()
    {
        //Start explosion timer if applicable
        spawnTime = Time.time;

        //If it's not hit anything in 10 seconds, abort mission
        //Destroy(gameObject, 10f);
    }


    void Update()
    {
        //Check whether to explode
        if (TimeToExplode != 0f && Time.time - spawnTime > TimeToExplode)
        {
            Explode();
        }
    }

    private void HitSound()
    {
        if (hitsound != null)
        {
            AudioSource.PlayClipAtPoint(hitsound, transform.position, 1f);
        }
    }

    private void SpawnImpactEffect()
    {
        //Spawn any impact effect we might want
        if (impactEffect != null)
        {
            GameObject obj = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(obj, 2f);
        }
    }

    //EXPLOSION
    private void Explode()
    {
        HitSound();

        //Projectile has a radius set, so model an explosion sphere
        Vector3 centre = transform.position;
        Collider[] colliders = Physics.OverlapSphere(centre, Radius);
        foreach (Collider hit in colliders)
        {
            /*
            //Also need to check if the explosion is blocked by any large, solid objects
            //This should only include level geometry like walls, as props shouldn't block explosions
            RaycastHit block;
            int onlyInclude = 1 << LayerMask.NameToLayer("BlockDamage");
            Vector3 dir = Vector3.Normalize(hit.transform.position - centre);   //get direction for raycast
            float dist = Vector3.Distance(centre, hit.transform.position);  //don't go further than this, only want to find objects BETWEEN these points

            if (Physics.Raycast(centre, transform.forward, out block, dist, onlyInclude))
            {
                continue;   //if geometry blocks, explosion doesn't affect this hit collider
            }
            */

            //^^ this has a bug where shooting a ceiling or wall will cause it to block the collisions on THIS side as well as the other side, pretty annoying
            //presumably the explosion centre decides to go inside the object so the raycast doesn't get anywhere

            //If target is destructible, delete it:
            if (hit.tag.Equals("Destructible"))
            {
                Destroy(hit.gameObject);
            }


            Rigidbody rb = hit.GetComponent<Rigidbody>();
            Enemy e = hit.transform.GetComponent<Enemy>();
            Player p = hit.transform.GetComponent<Player>();

            //Get distance from explosion to hit target
            float dist = Vector3.Distance(centre, hit.transform.position);
            float falloff;

            //If falloff is set, apply it as a scaling factor to the distance from target
            if (DamageFalloff != 0f)
                falloff = 1 / Mathf.Clamp(dist * DamageFalloff, 1f, 50f); //Don't go above max damage
            else
                falloff = 1;


            //Damage enemies and/or players depending who owns the projectile and whether it's self-damaging
            if (e != null && e.enabled && (PlayerOwned || SelfDamaging))
            {
                if (ImpactForce > 0f)
                {
                    //Disable pathfinding while they are being knocked back:
                    //e.GetComponent<NavMeshAgent>().enabled = false;
                    //e.eAI.StopMoving = true;
                }

                e.TakeDamage(Damage * falloff);
            }

            if (p != null && (!PlayerOwned || SelfDamaging))
            {
                p.TakeDamage(Damage * falloff);
            }

            if (rb != null)
            {
                rb.AddExplosionForce(ImpactForce, centre, Radius, 1.0f, ForceMode.Impulse);
            }

        }

        SpawnImpactEffect();
        Destroy(gameObject);
    }

    //SINGLE-TARGET IMPACT
    private void Impact(Collision other)
    {
        HitSound();

        Enemy e = other.transform.GetComponent<Enemy>();
        Player p = other.transform.GetComponent<Player>();

        //No damage radius, just damage whatever it hit if applicable
        Rigidbody rb = other.rigidbody;

        if (e != null)
        {
            e.TakeDamageSingle(Damage);

            if (ImpactForce > 0f)
            {
                //Disable pathfinding while they are being knocked back:
                e.GetComponent<NavMeshAgent>().enabled = false;
                e.eAI.StopMoving = true;
            }
        }

        if (p != null)
        {
            p.TakeDamage(Damage);
        }

        if (rb != null)
        {
            rb.AddForce(-other.GetContact(0).normal * ImpactForce);
        }

        SpawnImpactEffect();
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        string tag = other.transform.tag;

        //Only trigger impact if one of the following conditions is met:
        bool impact = false;

        //ON PLAYER HIT by enemy-owned projectile
        if (!PlayerOwned)
        {
            //Explode on contact with player (if applicable)
            if (ExplodeOnEnemyContact)
            {
                if (other.gameObject.tag.Equals("Player"))
                {
                    impact = true;
                }
                //If set not to explode on Enemy contact, then don't and just bounce again (even if bounce limit is exceeded)
            }
        }
        //ON ENEMY HIT by player-owned projectile
        else if (PlayerOwned)
        {
            //Explode on contact with enemy (if applicable)
            if (ExplodeOnEnemyContact)
            {
                if (other.gameObject.tag.Equals("Enemy"))
                {
                    impact = true;
                }
            }
        }
        
        if (!impact)   //ELSE ON GROUND HIT:
        {

            //Explode on contact with ground (if applicable)
            if (ExplodeOnGroundContact)
            {
                impact = true;
            }

            //If there is a bounce minimum:
            if (MaxBounces != 0)
            {
                //If bounce minimum is met:
                if (bounces > MaxBounces)
                {
                    impact = true;
                }
            }
            else
            {
                //Else there is no bounce minimum
                //Therefore, if there is no timer, just explode now
                if (TimeToExplode == 0f) impact = true;
            }
        }

        //Increment bounces, since we hit a surface
        bounces++;

        //ON IMPACT: Stop bouncing and either explode or apply impact at point
        if (impact)
        {
            if (Radius != 0)
            {
                Explode();
            }
            else
            {
                Impact(other);
            }
                
        }

    }

}
