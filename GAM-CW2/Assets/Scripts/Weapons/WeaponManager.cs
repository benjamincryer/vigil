using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weps;
    public GameObject grenade;
    public int wepSelected = 0;

    public float WeaponSwitchDelay = 0.2f;
    public float GrenadeSpeed = 75f;
    private bool grenadeHeld = false;
    private float grenadeTimer = 0f;

    public Ammo Grenades;

    public TextMeshProUGUI txtAmmo, txtGrenades;

    void Start()
    {
        ActivateWep();
    }

    void Update()
    {
        //Check if grenade button pressed
        //Disable weapons and tick down the grenade timer until released
        if (Input.GetKeyDown(KeyCode.F) && Grenades.Amount > 0)
        {
            HolsterWep();
            grenadeHeld = true;
            grenadeTimer = Time.time;
        }

        //If grenade held, check for release or timer expiring
        if (grenadeHeld)
        {
            Projectile proj = grenade.GetComponent<Projectile>();
            float timeElapsed = Time.time - grenadeTimer;

            //If grenade button released OR the fuse expires, throw it
            if (Input.GetKeyUp(KeyCode.F)
                || (timeElapsed >= proj.TimeToExplode))
            {
                ThrowGrenade(timeElapsed);
                ActivateWep();
                grenadeHeld = false;
            }
        }
        else    //Only allow weapon switching if grenade not held:
        {
            //Check if a number was pressed
            for (int i = 0; i < weps.Length; i++)
            {
                if (Input.GetKeyDown("" + (i + 1)) && wepSelected != i)
                {
                    SwitchWep(i);
                }
            }
        }

        UpdateText();

    }

    public void UnlockWeapon(Gun gunToUnlock)
    {
        //If this is the first time seeing the weapon, switch to it automatically
        if (gunToUnlock.Unlocked == false)
        {
            //find index of the gun in our list so we can switch
            for (int i = 0; i < weps.Length; i++)
            {
                Gun gun = weps[i].GetComponent<Gun>();
                if (gun == gunToUnlock)
                {
                    SwitchWepWithoutDelay(i);
                }
            }
        }

        //Otherwise just unlock it and do nothing
        gunToUnlock.Unlocked = true;
    }

    private void UpdateText()
    {
        GameObject wep = weps[wepSelected];

        if (wep.activeSelf)
        {
            Ammo ammo = weps[wepSelected].GetComponent<Gun>().AmmoUsed;

            //Update text
            if (ammo.MaxCapacity != 0)
                txtAmmo.text = ammo.Name + " " + ammo.Amount;
            else txtAmmo.text = ammo.Name + " ∞";

            //txtGrenades.text = "GRENADES  " + Grenades.Amount;
        }
        else
        {
            txtAmmo.text = "";
            txtGrenades.text = "";
        }
    }

    private void ThrowGrenade(float timeExpired)
    {
        Camera cam = Camera.main;

        //Shoot slightly above where the player is looking
        Vector3 f = cam.transform.forward;
        Vector3 angle = new Vector3(f.x, f.y, f.z);
        GameObject shoot = Instantiate(grenade, cam.transform.position, Quaternion.LookRotation(angle));

        Projectile proj = shoot.GetComponent<Projectile>();
        proj.TimeToExplode = proj.TimeToExplode - timeExpired;                  //remove amount of time already held

        Rigidbody rb = shoot.GetComponent<Rigidbody>();
        rb.AddForce(cam.transform.forward * GrenadeSpeed);

        Grenades.UseAmmo(1);
    }

    //Switches weapons without adding the delay
    private void SwitchWepWithoutDelay(int wepIndex)
    {
        //Disable current weapon, enable new one
        HolsterWep();
        wepSelected = wepIndex;
        ActivateWep();
    }

    //Switches weapons but waits WeaponSwitchDelay seconds first, and disables shooting until switch is complete
    //Prevents player from spamming single-shot, high-damage weapons by rapidly switching
    private void SwitchWep(int wepIndex)
    {
        //Only switch if the weapon is unlocked:
        if (weps[wepIndex].GetComponent<Gun>().Unlocked)
        {
            //First disable this weapon script
            SetWeaponScript(false);

            StartCoroutine(SwitchAfterDelay(wepIndex));
        }
    }

    //Wait t seconds then activate weapon
    IEnumerator SwitchAfterDelay(int wepIndex)
    {
        yield return new WaitForSeconds(WeaponSwitchDelay);

        HolsterWep();
        wepSelected = wepIndex;
        ActivateWep();
        SetWeaponScript(true);
    }

    //Enables or disables the Gun script on a weapon, used to prevent shooting while keeping gun model visible
    private void SetWeaponScript(bool b)
    {
        GameObject wep = weps[wepSelected];
        wep.GetComponent<Gun>().enabled = b;
    }

    //Activates the weapon's GameObject
    private void ActivateWep()
    {
        GameObject wep = weps[wepSelected];
        wep.SetActive(true);

        Gun gun = wep.GetComponent<Gun>();
        gun.OnSwitched();
    }

    //Disables the weapon's GameObject and resets all animations/sound effects
    private void HolsterWep()
    {
        GameObject wep = weps[wepSelected];

        Gun gun = wep.GetComponent<Gun>();
        gun.ResetAnim();

        if (gun.sfxLoop) gun.audioSrc.Stop();

        wep.SetActive(false);
    }
}
