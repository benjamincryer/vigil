using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourPickup : Trigger
{
    public float Amount = 100f;

    public override void TriggerAction()
    {
        AudioSource src = GetComponent<AudioSource>();
        if (src != null) src.Play();

        //Apply healing
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddArmour(Amount);
        Destroy(gameObject, 1f);

        //Disable this script and all renderers until collection sound played
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
        {
            r.enabled = false;
        }
    }

    public override void OnLoad()
    {
        //Disable this script and all renderers until collection sound played
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rs)
        {
            r.enabled = false;
        }
    }
}
