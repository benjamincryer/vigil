using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour, ISaveable
{
    public Uuid id;
    public bool Unlocked = true;

    public Camera cam;
    public AudioSource audioSrc;

    public AudioClip sfxShoot;
    public bool sfxLoop = false;

    public GameObject projectile;
    public float ProjectileSpeed = 50f;

    public float ZoomFactor = 0.0f;
    private bool zoomed;

    public Ammo AmmoUsed;

    public float Damage = 10f;
    public float DamageFalloff = 0f;
    public float Range = 100f;
    public int NumBullets = 1;
    public int AmmoConsumed = 1;
    public float Spread = 0f;
    public float Kickback = 1f;
    public float FireRate = 6f; //Number of shots per second
    public float ImpactForce = 10f;
    public float ImpactSize = 1f;

    private PlayerLook look;
    private Rigidbody player;
    private ParticleSystem flash;
    private Object impactPrefab;

    private WeaponPos wepPos;
    private float shotDelay = 0f;
    private bool fired = false;
    private HashSet<Enemy> enemiesHit;

    void Start()
    {
        impactPrefab = Resources.Load("Prefabs/Impact");
    }

    void Awake()
    {
        cam = Camera.main;
        wepPos = transform.parent.gameObject.GetComponent<WeaponPos>();
        audioSrc = GameObject.FindGameObjectWithTag("GunSounds").GetComponent<AudioSource>();
        flash = GameObject.FindGameObjectWithTag("GunParticles").GetComponent<ParticleSystem>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        player = p.GetComponent<Rigidbody>();
        look = p.GetComponent<PlayerLook>();
    }

    void Update()
    {
        //SHOOT
        if (Input.GetButton("Fire1") && Time.time > (1 / FireRate) + shotDelay)
        {
            if (audioSrc.loop && fired == false) audioSrc.Stop();
            Shoot();
        }

        //STOP SHOOTING
        if (Input.GetButtonUp("Fire1") && audioSrc.loop)
        {
            ResetAnim();
            StopShooting();
        }

        //TOGGLE ZOOM
        if (ZoomFactor != 0.0f)
        {
            if (Input.GetButton("Fire2") && zoomed == false)
                Zoom(true);

            if (Input.GetButtonUp("Fire2"))
                Zoom(false);
        }
    }

    public void OnSwitched()
    {
        fired = false;

        //Move muzzle flash to this gun's location
        Transform flashPos = transform.Find("FlashPos");
        if (flashPos != null)
        {
            flash.transform.position = flashPos.position;
        }

        //Set up new gun sound
        audioSrc.loop = sfxLoop;
        if (sfxLoop) audioSrc.clip = sfxShoot;

        //Reset to default zoom
        Zoom(false);

        wepPos.kickback = 0f;
    }

    public void StopShooting()
    {
        audioSrc.Stop();
    }

    public void ResetAnim()
    {
        Animation anim = GetComponentInChildren<Animation>();
        AnimationClip clip = anim.GetClip("Shoot");
        if (clip != null) clip.SampleAnimation(gameObject, 0);
        anim.Stop();
    }

    private void Zoom(bool b)
    {
        zoomed = b;

        float zoom = 1;
        if (zoomed) zoom = ZoomFactor;

        cam.fieldOfView = 80 / zoom;
        look.effectiveSens = look.Sensitivity / zoom;
    }

    private void Shoot()
    {
        //If we don't have enough ammo, don't fire
        if (AmmoUsed.MaxCapacity != 0 && AmmoConsumed > AmmoUsed.Amount)
        {
            if (audioSrc.loop) StopShooting();
            return;
        }

        fired = true;

        //Play muzzle flash particles
        flash.Play();

        //Apply kickback/recoil to gun position
        wepPos.kickback = Kickback;

        //Play shoot sound
        if (!audioSrc.loop)
        {
            audioSrc.PlayOneShot(sfxShoot, 1f);       //Using OneShot to allow multiple gunshot sounds to overlap on one AudioSource if necessary
        }
        else
        {
            if (!audioSrc.isPlaying) audioSrc.Play();   //We're looping, so just start playing once if it isn't already playing
        }

        //Play shoot animation
        GetComponentInChildren<Animation>().Play();

        //Apply force to player in opposite direction of shot (half of impact force)
        player.AddForce(-Camera.main.transform.forward * NumBullets * ImpactForce * 0.5f, ForceMode.Impulse);


        //Reset list of enemies hit by bullets before we populate it:
        enemiesHit = new HashSet<Enemy>();

        //Repeat x times for the number of bullets
        for (int b = 0; b < NumBullets; b++)
        {
            if (projectile == null)
            {
                Hitscan();
            }
            else
            {
                Projectile();
            }
        }

        //Then apply damage at once to all enemies hit
        foreach(Enemy hit in enemiesHit)
        {
            //hit.SpawnDamageText();
        }

        //Remove ammo:
        AmmoUsed.UseAmmo(AmmoConsumed);

        shotDelay = Time.time;
    }

    private void Hitscan()
    {
        //For hitscan weapon: Use Raycast
        RaycastHit hit;
        int ignoreLayer = LayerMask.GetMask("Player", "Invisible", "Hitbox", "Corpse");

        //Get randomly deviated angle from crosshair
        float deviation = Random.Range(-Spread, Spread);
        Vector3 angle;
        angle = Quaternion.AngleAxis(deviation, Vector3.up) * cam.transform.forward;
        angle = Quaternion.AngleAxis(deviation, cam.transform.forward) * angle;

        if (Physics.Raycast(cam.transform.position, angle, out hit, Range, ~ignoreLayer))
        {
            Enemy e = hit.transform.GetComponent<Enemy>();

            //check if we hit a trigger
            Trigger[] trigs = hit.transform.GetComponents<Trigger>();
            if (trigs != null)
            {
                foreach(Trigger t in trigs)
                {
                    if (t.TriggerOnShoot)
                    {
                        t.TriggerAction();
                        t.isTriggered = true;
                    }
                }
            }

            //Calculate damage falloff
            float dist = Vector3.Distance(cam.transform.position, hit.transform.position);  //Get distance travelled by raycast
            float falloff;

            //If falloff is set, apply it as a scaling factor to the distance from target
            if (DamageFalloff != 0f)
                falloff = 1 / Mathf.Clamp(dist * DamageFalloff, 1f, 50f);
            else
                falloff = 1;

            if (e != null)
            {
                if (ImpactForce > 0f)
                {
                    //Disable pathfinding while they are being knocked back:
                    //e.GetComponent<NavMeshAgent>().enabled = false;
                    //e.eAI.StopMoving = true;
                }

                e.TakeDamage(Damage * falloff);
                e.SpawnBlood(hit.point, ImpactSize);
                //enemiesHit.Add(e);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * ImpactForce, ForceMode.Impulse);
            }

            GameObject impact = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
            impact.transform.localScale = new Vector3(ImpactSize, ImpactSize, ImpactSize);
            Destroy(impact, 1f);
        }
    }

    private void Projectile()
    {
        //For projectile weapon: Spawn projectile object at end of barrel

        //Get randomly deviated angle from crosshair
        float deviation = Random.Range(-Spread, Spread);
        Vector3 angle;
        angle = Quaternion.AngleAxis(deviation, Vector3.up) * cam.transform.forward;
        angle = Quaternion.AngleAxis(deviation, cam.transform.forward) * angle;

        GameObject shoot = Instantiate(projectile, flash.transform.position, Quaternion.LookRotation(angle));

        Projectile proj = shoot.GetComponent<Projectile>();
        proj.Damage = Damage;
        proj.DamageFalloff = DamageFalloff;
        proj.ImpactForce = ImpactForce;

        Rigidbody rb = shoot.GetComponent<Rigidbody>();
        rb.AddForce(angle * ProjectileSpeed);
    }

    public void PopulateSaveData(SaveData a_SaveData)
    {
        SaveData.WeaponData wepData = new SaveData.WeaponData();
        wepData.id = id.uuid;
        wepData.Unlocked = Unlocked;
        wepData.Ammo = AmmoUsed.Amount;

        a_SaveData.m_WeaponData.Add(wepData);
    }

    public void LoadFromSaveData(SaveData a_SaveData)
    {
        foreach (SaveData.WeaponData wepData in a_SaveData.m_WeaponData)
        {
            if (wepData.id == id.uuid)
            {
                Unlocked = wepData.Unlocked;
                AmmoUsed.Amount = wepData.Ammo;
                break;
            }
        }
    }
}
