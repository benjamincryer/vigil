using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundCenter : MonoBehaviour
{
    public float AmountPerSecond = 45f;
    private Vector3 center;

    void Start()
    {
        //Get center of collider:
        Collider col = GetComponent<Collider>();
        center = col.bounds.center;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(center, Vector3.up, AmountPerSecond * Time.deltaTime);
    }
}
