using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public Gradient FadeGradient;
    public float secondsToLive = 1f;

    private float spawnTime;
    private MeshRenderer r;

    void Start()
    {
        spawnTime = Time.time;
        r = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        float timeElapsed = Time.time - spawnTime;

        //Set opacity
        r.material.color = FadeGradient.Evaluate(timeElapsed / secondsToLive);

        if (timeElapsed >= secondsToLive)
        {
            Destroy(gameObject);
        }
    }
}
