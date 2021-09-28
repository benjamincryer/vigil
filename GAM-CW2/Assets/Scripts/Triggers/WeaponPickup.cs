using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Trigger
{
    public Gun Weapon;

    public override void TriggerAction()
    {
        float destroyTimer = 0f;

        //Play sound if attached
        AudioSource sound = GetComponent<AudioSource>();
        if (sound != null)
        {
            sound.Play();
            destroyTimer = sound.clip.length;
        }

        WeaponManager wm = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponManager>();
        wm.UnlockWeapon(Weapon);

        //Disable visuals
        foreach (Renderer visual in transform.GetComponentsInChildren<Renderer>())
        {
            visual.enabled = false;
        }
    }

    public override void OnLoad()
    {
        WeaponManager wm = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponManager>();
        wm.UnlockWeapon(Weapon);

        //Disable visuals
        foreach (Renderer visual in transform.GetComponentsInChildren<Renderer>())
        {
            visual.enabled = false;
        }
    }

}
