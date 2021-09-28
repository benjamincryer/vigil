using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal Destination;
    public float CooldownTimer = 0f;
    public AudioClip sfxActivate, sfxTeleport;

    private AudioSource src;

    void Start()
    {
        src = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    public void SoundActivate()
    {
        src.clip = sfxActivate;
        src.Play();
    }

    public void SoundTeleport()
    {
        src.clip = sfxTeleport;
        src.Play();
    }

    //Trigger teleport
    void OnTriggerEnter(Collider other)
    {
        //If the portal has finished its cooldown timer:
        if (Time.time - CooldownTimer > 1f)
        {
            //Teleport any rigidbody
            Rigidbody r = other.GetComponent<Rigidbody>();
            if (r != null)
            {
                Destination.GetComponent<Portal>().SoundTeleport();

                //Set this and the other portal's cooldown timer
                CooldownTimer = Time.time;
                Destination.CooldownTimer = Time.time;

                //Teleport and keep velocity, but change direction of rigidbody and camera
                other.transform.position = Destination.transform.position;
                r.velocity = Destination.transform.forward * r.velocity.magnitude;

                if (other.tag.Equals("Player"))
                {
                    //Get difference between rotations of each camera, shift the player rotation by that amount
                    //Quaternion diff = Quaternion.Inverse(Destination.transform.rotation) * transform.rotation;
                    Vector3 eu1 = transform.rotation.eulerAngles;
                    Vector3 eu2 = Destination.transform.rotation.eulerAngles;

                    PlayerLook look = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLook>();
                    look.rotation.y += 180f + (eu2.y - eu1.y);
                }
            }

        }
    }

}
