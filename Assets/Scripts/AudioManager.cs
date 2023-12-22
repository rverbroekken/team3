using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager Instance;
    //    private Item item;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        foreach (var sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.clip;
            sound.audioSource.volume = sound.volume;
            //sound.audioSource.pitch= sound.pitch;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.clip?.name== name);
        s?.audioSource.Play();
    }
}


