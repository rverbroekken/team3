using UnityEngine;
using System;

//[Serializable]
[CreateAssetMenu(fileName = "Sound", menuName = "Sounds/Sound", order = 1)]
public class Sound : ScriptableObject
{
//    public string soundName;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume;
    [Range(.1f, 3f)] public float pitch;

    [HideInInspector] public AudioSource audioSource;
}


