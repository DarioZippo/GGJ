using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class SoundEffectEvent : MonoBehaviour
{
    public AudioClip[] audioClips;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public AudioClip GetRandomClip()
    {
        int randomIndex = Random.Range(0, audioClips.Length);
        return audioClips[randomIndex];
    }
}
