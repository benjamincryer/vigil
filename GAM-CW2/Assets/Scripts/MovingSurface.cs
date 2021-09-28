using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSurface : MonoBehaviour
{
    public float ForceSpeed = 25f;
    public float hScrollSpeed = 0.0f;
    public float vScrollSpeed = 0.25f;

    private Renderer ren;
    private Rigidbody rig;
    private bool scroll = true;

    void Start()
    {
        ren = GetComponent<Renderer>();
        rig = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (scroll)
        {
            //Scroll the conveyer belt texture
            float verticalOffset = Time.time * vScrollSpeed;
            float horizontalOffset = Time.time * hScrollSpeed;
            ren.material.mainTextureOffset = new Vector2(horizontalOffset, verticalOffset);

            //Simulate moving object
            /*
            Vector3 movement = transform.forward * MoveSpeed * Time.deltaTime;
            rig.position -= movement;
            rig.MovePosition(rig.position + movement);
            */
        }
    }

    void OnCollisionStay(Collision other)
    {
        Vector3 direction = transform.forward * ForceSpeed;

        Rigidbody r = other.rigidbody;
        if (r != null)
        {
            r.AddForce(direction, ForceMode.Acceleration);
        }
    }

    public void ToggleScroll()
    {
        scroll = !scroll;
    }
}
