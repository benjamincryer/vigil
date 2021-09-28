using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePortal : Trigger
{
    public GameObject Particles;
    public Collider TriggerCollider;

    public bool SetBoolean = true;

    public override void TriggerAction()
    {
        //Play sound
        //Portal p = TriggerCollider.gameObject.GetComponent<Portal>();
        //p.SoundActivate();
        TriggerCollider.GetComponent<AudioSource>().Play();

        TriggerCollider.enabled = SetBoolean;
        Particles.gameObject.SetActive(SetBoolean);
    }

    public override void OnLoad()
    {
        TriggerCollider.enabled = SetBoolean;
        Particles.gameObject.SetActive(SetBoolean);
    }

}
