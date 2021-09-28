using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    EnemyAI eAI;
    Enemy e;

    //animation events can only do 1 parameter,
    //so I'm working around it by passing 1 parameter per event, but storing them in the script, so they can later by used
    private float chance = 1f;
    private float delay = 0f;
    private List<AudioClip> sfxPool = new List<AudioClip>();

    void Start()
    {
        eAI = transform.parent.GetComponent<EnemyAI>();
        e = transform.parent.GetComponent<Enemy>();
    }

    //Plays the sound
    public void PlaySound(AudioClip sfx)
    {
        AudioSource a = e.GetComponent<AudioSource>();
        a.clip = sfx;
        a.PlayDelayed(delay); //add any delay set

        Reset();
    }

    //Picks from the sfxPool
    public void PickSound()
    {
        int l = sfxPool.Count;
        int sound = Random.Range(0, l);

        AudioSource a = e.GetComponent<AudioSource>();
        a.clip = sfxPool[sound];
        a.PlayDelayed(delay); //add any delay set

        Reset();
    }

    //IF % chance is hit: Plays the sound
    public void PlaySoundWithChance(AudioClip sfx)
    {
        float r = Random.value;
        if (r < chance)
        {
            PlaySound(sfx);
        }
    }

    //IF % chance is hit: Picks from the sfxPool
    public void PickSoundWithChance()
    {
        float r = Random.value;
        if (r < chance)
        {
            PickSound();
        }
    }

    private void Reset()
    {
        //reset queue
        sfxPool.Clear();

        //reset delay and chance
        delay = 0f;
        chance = 1f;
    }

    public void AddSoundToPool(AudioClip sfx)
    {
        sfxPool.Add(sfx);
    }

    public void SetChance(float chance)
    {
        this.chance = chance;
    }

    public void AddRandomDelay(float max)
    {
        delay = Random.Range(0f, max);
    }

    //ATTACKS

    public void OnActiveHitboxFrame()
    {
        eAI.attacking = true;
    }

    public void OnDeactivateHitbox()
    {
        eAI.attacking = false;
    }

    public void OnAttackEnd()
    {
        eAI.rotateToPlayer = true;
    }

    public void OnProjectileShoot()
    {
        e.FireProjectile();
    }

    public void OnExplode(GameObject proj)
    {
        Instantiate(proj, transform.position, Quaternion.identity);
    }

    IEnumerator MoveToPoint(Vector3 start, Vector3 end, float d)
    {
        float t = 0f;

        while (t < d) {
            transform.localPosition = Vector3.Lerp(start, end, t / d);
            t += Time.deltaTime;

            yield return null;
        }
        
    }

    public void LerpToPoint()
    {
        StartCoroutine(MoveToPoint(transform.localPosition, Vector3.zero, 1f));
    }
}
