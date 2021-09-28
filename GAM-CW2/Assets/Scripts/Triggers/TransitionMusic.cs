using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMusic : MonoBehaviour
{
    public AudioClip song;
    public float volume = 0.2f;
    public bool DestroyThis = false;

    private AudioSource src;

    void Start()
    {
        src = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
    }

    private IEnumerator FadeIn(float delay)
    {
        //After fade-out is done, replace audio clip and fade in
        yield return new WaitForSeconds(delay);

        //(if replacement song given)
        if (song != null)
        {
            src.clip = song;
            src.Play();
            StartCoroutine(FadeAudioSource.StartFade(src, delay, volume));
        }

        if (DestroyThis) enabled = false;

        yield break;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;

        if (other.tag.Equals("Player"))
        {
            float delay = 0f;

            //Fade out (if song playing)
            if (src.volume != 0f)
            {
                delay = 0.5f;
                StartCoroutine(FadeAudioSource.StartFade(src, delay, 0f));
            }

            //Queue a fade-in
            StartCoroutine(FadeIn(delay));
        }
    }
}
