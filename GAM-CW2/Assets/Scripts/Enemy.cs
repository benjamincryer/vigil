using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Uuid id;

    public float Health = 50f;
    public Gradient DamageGradient;
    public Animator animator;

    public AudioClip[] sfxDeath;

    public float MeleeDistance = 5.0f;
    public float ShootDistance = 20f;
    public float MeleeDamage = 20f;
    public float MeleeForce = 3f;

    public float DamageTextSize = 40f;
    public float DamageTextBaseDistance = 4f;
    private Object damagePrefab;
    private Transform target;
    private GameObject player;
    public EnemyAI eAI;
    public GameObject bloodPrefab;

    public float ProjectileSpeed = 40f;
    public float ProjectileSpread = 1f;
    public GameObject projectile;
    public Transform projSpawn;

    //Used to accumulate multiple hits in a short time into a single damage-text
    private float damageTaken = 0f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        damagePrefab = Resources.Load("Prefabs/DamageText");
        target = transform.Find("Damage");
        eAI = GetComponent<EnemyAI>();
    }

    void Update()
    {
        /*
        //Re-enable agent when rigidbody velocity is zero
        Rigidbody r = GetComponent<Rigidbody>();
        if (r.velocity == Vector3.zero)
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (!agent.enabled)
            {
                agent.enabled = true;
            }
        }
        */
    }

    public void TakeDamage(float damage)
    {
        //Update accumulated damage counter (will reset in a moment)
        //damageTaken += damage;

        //Receive damage
        Health -= damage;

        //Reset damage counter
        //damageTaken = 0f;

        //Aggro the AI if not already
        if (!eAI.Aggroed)
        {
            eAI.Aggro();
        }

        //If we took enough damage, we can now safely destroy self
        if (Health <= 0f)
        {
            Death();
        }
    }

    public void TakeDamageSingle(float damage)
    {
        TakeDamage(damage);
        //SpawnDamageText();
    }

    public void SpawnBlood(Vector3 pos, float ImpactSize)
    {
        if (bloodPrefab != null)
        {
            //Spawn blood prefab
            GameObject impact = Instantiate(bloodPrefab, pos, Quaternion.identity);
            impact.transform.localScale = new Vector3(ImpactSize, ImpactSize, ImpactSize);
            Destroy(impact, 1f);
        }
    }

    public void SpawnDamageText()
    {
        //Spawn damage text
        GameObject dmg = Instantiate(damagePrefab, target.position, Quaternion.identity) as GameObject;

        TextMesh txt = dmg.GetComponent<TextMesh>();
        txt.text = "-" + Mathf.Round(damageTaken);
        txt.color = DamageGradient.Evaluate(damageTaken / 50f);

        //Scale according to camera distance
        float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
        txt.fontSize = Mathf.RoundToInt(DamageTextSize * Mathf.Sqrt(dist / DamageTextBaseDistance));

        //Spawn a random distance from target
        float max = 1f;
        Vector3 pos = target.position;
        dmg.transform.position = new Vector3(pos.x + Random.Range(0f, max), pos.y + Random.Range(0f, max), pos.z + Random.Range(0f, max));

        /*
        //Reset damage counter
        damageTaken = 0f;

        //If we took enough damage, we can now safely destroy self
        if (hp <= 0f)
        {
            Death();
        }
        */
    }

    /*
    //first-order intercept using absolute target position
    public static Vector3 FirstOrderIntercept
    (
        Vector3 shooterPosition,
        Vector3 shooterVelocity,
        float shotSpeed,
        Vector3 targetPosition,
        Vector3 targetVelocity
    )
    {
        Vector3 targetRelativePosition = targetPosition - shooterPosition;
        Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
        float t = FirstOrderInterceptTime
        (
            shotSpeed,
            targetRelativePosition,
            targetRelativeVelocity
        );
        return targetPosition + t * (targetRelativeVelocity);
    }
    //first-order intercept using relative target position
    public static float FirstOrderInterceptTime
    (
        float shotSpeed,
        Vector3 targetRelativePosition,
        Vector3 targetRelativeVelocity
    )
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - shotSpeed * shotSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
    */

    public static bool CalculateTrajectory(float TargetDistance, float ProjectileVelocity, out float CalculatedAngle)
    {
        CalculatedAngle = 0.5f * (Mathf.Asin((-Physics.gravity.y * TargetDistance) / Mathf.Pow(ProjectileVelocity,2)) * Mathf.Rad2Deg);
        if (float.IsNaN(CalculatedAngle))
        {
            CalculatedAngle = 0;
            return false;
        }
        return true;
    }

    public void FireProjectile()
    {
        GameObject shoot = Instantiate(projectile, projSpawn.position, Quaternion.LookRotation(transform.forward));
        Rigidbody rb = shoot.GetComponent<Rigidbody>();

        Vector3 origin = transform.position;
        Vector3 target = player.transform.position;

        //Player location
        Vector3 dir = (target - origin);

        //Get randomly deviated angle from crosshair
        float deviation = Random.Range(-ProjectileSpread, ProjectileSpread);
        Vector3 dev;
        dev = Quaternion.AngleAxis(deviation, Vector3.up) * dir;
        dev = Quaternion.AngleAxis(deviation, dir) * dev;

        dir = new Vector3(dev.x, dev.y + 0.5f, dev.z); //aim at head
        float dist = Vector3.Distance(origin, target);

        //Predict player movement
        //Vector3 TargetCenter = FirstOrderIntercept(transform.position, Vector3.zero, ProjectileSpeed, target, player.GetComponent<Rigidbody>().velocity);

        //Calculate parabola to hit player
        if (rb.useGravity)
        {
            float angle;

            if (CalculateTrajectory(dist, ProjectileSpeed, out angle))
            {
                float trajectoryHeight = Mathf.Tan(angle * Mathf.Deg2Rad) * dist;
                dir.y += trajectoryHeight;
            }

        }

        rb.AddForce(Vector3.Normalize(dir) * ProjectileSpeed);
    }

    void Death()
    {
        //Play killsound
        //AudioSource src = player.GetComponent<AudioSource>();
        //src.PlayOneShot(killsound);

        //Play deathsound
        //Pick between the sfx given
        int l = sfxDeath.Length;
        if (l > 0)
        {
            int r = Random.Range(0, l);
            GetComponent<AudioSource>().PlayOneShot(sfxDeath[r]);
        }

        Dead();
    }

    public void Dead()
    {
        //Play death animation
        animator.SetBool("Dying", true);

        gameObject.layer = LayerMask.NameToLayer("Corpse");

        //Disable components
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.isStopped = true;
        GetComponent<EnemyAI>().enabled = false;
        enabled = false;
    }

    public void Alive()
    {
        //Play death animation
        animator.SetBool("Dying", false);

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        //Disable components
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        agent.isStopped = false;
        GetComponent<EnemyAI>().enabled = true;
        enabled = true;
    }

    //Destroy when knocked off map
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            Death();
        }
    }

    /*
    public void PopulateSaveData(SaveData a_SaveData)
    {
        SaveData.EnemyData enemyData = new SaveData.EnemyData();
        enemyData.Health = Health;
        enemyData.Pos = transform.position;

        a_SaveData.m_EnemyData.Add(enemyData);
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        foreach (SaveData.EnemyData enemyData in a_SaveData.m_EnemyData)
        {
            if (enemyData.id == id.uuid)
            {
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                agent.Warp(enemyData.Pos);

                transform.position = enemyData.Pos;
                Health = enemyData.Health;

                //Make dead or make alive
                if (Health <= 0f)
                {
                    Dead();
                }
                else
                {
                    Alive();
                }

                break;
            }
        }
    }
    */

}
