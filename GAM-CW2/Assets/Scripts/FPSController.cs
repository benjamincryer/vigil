using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    private float distToGround;
    private AudioSource audioSrc;
    private Player p;

    public float MoveSpeed = 7.0f;
    public float GroundAccelerate = 50.0f;
    public float AirAccelerate = 15.0f;
    public float MaxVelocityGround = 300.0f;
    public float MaxVelocityAir = 350.0f;
    public float Friction = 4f;
    public float JumpVelocity = 40f;

    public float JumpDelay = 0.1f;
    private float lastJump = 0f;

    float forward, strafe;
    bool jump;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        distToGround = GetComponent<Collider>().bounds.extents.y;
        audioSrc = GetComponent<AudioSource>();
        p = GetComponent<Player>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            p.Restart();
        }
    }

    void FixedUpdate()
    {
        //Character Movement input
        forward = Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime;
        strafe = Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime;
        jump = Input.GetButton("Jump");

        Rigidbody r = GetComponent<Rigidbody>();

        //Get normalized direction to move towards from the axis input
        Vector3 accelDir = new Vector3(strafe, 0, forward);
        accelDir = transform.TransformDirection(accelDir);
        accelDir.Normalize();

        //Air Strafing:
        if (isGrounded())
        {
            r.velocity = MoveGround(accelDir, r.velocity);

            if (jump && (Time.time - lastJump >= JumpDelay))
            {
                if (!audioSrc.isPlaying) audioSrc.Play();
                r.AddForce(new Vector3(0, JumpVelocity, 0), ForceMode.Impulse);
                lastJump = Time.time;
            }
        }
        else
        {
            r.velocity = MoveAir(accelDir, r.velocity);
        }
    }

    //Applies set amount of acceleration
    private Vector3 Accel(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxVelocity)
    {
        float projVel = Vector3.Dot(prevVelocity, accelDir); // Vector projection of current velocity onto accelDir
        float accelVel = accelerate * Time.fixedDeltaTime;

        //Prevent accelerated velocity from going above maxVelocity
        if (projVel + accelVel > maxVelocity) accelVel = maxVelocity - projVel;

        return prevVelocity + accelDir * accelVel;
    }

    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        float speed = prevVelocity.magnitude + MoveSpeed;   //Add a flat amount of speed every update
        if (speed != 0)
        {
            //Reduce the velocity by the amount of friction
            float drop = speed * Friction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accel(accelDir, prevVelocity, GroundAccelerate, MaxVelocityGround);
    }

    //Same as MoveGround but without applying friction (and with the air variables)
    private Vector3 MoveAir(Vector3 accelDir, Vector3 prevVelocity)
    {
        return Accel(accelDir, prevVelocity, AirAccelerate, MaxVelocityAir);
    }


    //Returns true if the player is currently grounded
    private bool isGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.05f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Respawn")
        {
            p.Die();
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Check if it's an enemy melee attack
        if (other.gameObject.tag == "Hitbox")
        {
            EnemyAI e = other.gameObject.transform.parent.GetComponent<EnemyAI>();
            if (e != null && e.attacking) //enemy must be attacking, and this trigger collision must be with the attack hitbox:
            {
                Debug.Log("Attack hit!");
                e.attacking = false;

                p.TakeDamage(e.e.MeleeDamage); //apply damage to player

                //Knock away from hitbox (lift off ground a little too)
                Vector3 angle = gameObject.transform.position;
                angle = new Vector3(angle.x, angle.y + 0.5f, angle.z);
                GetComponent<Rigidbody>().AddForce(Vector3.Normalize(angle - other.gameObject.transform.position) * e.e.MeleeForce, ForceMode.Impulse);
            }
        }
    }

}
