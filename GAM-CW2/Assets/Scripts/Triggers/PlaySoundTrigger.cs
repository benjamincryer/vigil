using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundTrigger : Trigger
{
    public AudioClip sfx;

    public override void TriggerAction()
    {
        AudioSource src = GetComponent<AudioSource>();
        if (src != null) src.PlayOneShot(sfx, 1f);

        Destroy(gameObject, sfx.length);
    }

}