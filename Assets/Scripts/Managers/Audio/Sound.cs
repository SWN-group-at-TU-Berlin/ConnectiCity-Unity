using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

/*This is a basic class that the AudioManager usese in order
 to initialize the audio source in its list.*/

[Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    public float volume;

    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
