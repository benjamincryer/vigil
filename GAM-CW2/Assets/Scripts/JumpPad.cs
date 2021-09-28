using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float ForceAmount = 100f;
    public float CooldownTimer = 0f;

    private AudioSource src;

    void Start()
    {
        src = GetComponent<AudioSource>();
    }


    void OnTriggerEnter(Collider other)
    {
        //If the jumppad has finished its cooldown timer:
        if (Time.time - CooldownTimer > 0.5f)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                Rigidbody r = other.GetComponent<Rigidbody>();
                if (r != null)
                {
                    r.velocity = Vector3.zero;
                    r.AddForce(transform.up * ForceAmount, ForceMode.Impulse);
                    CooldownTimer = Time.time;

                    if (src != null) src.Play();
                }
            }
        }
    }

}
