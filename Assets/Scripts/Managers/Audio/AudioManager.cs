using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;


/*this is a singleton class that handles sounds in the game.
 different classes call it in order to play a sound at a specific moment.*/
public class AudioManager : MonoBehaviour
{
    #region singleton
    private static AudioManager instance;
    #region getter
    public static AudioManager Instance { get { return instance; } }
    #endregion
    #endregion

    //collection of all the sounds to initialize
    [SerializeField] Sound[] sounds;

    //TODO: list for music tracks;

    [Header("Random pitch limits:")]
    [SerializeField, Range(-3, 3)] float minPitch = 0f;
    [SerializeField, Range(-3, 3)] float maxPitch = 2f;

    private void Awake()
    {
        #region singleton initialization
        //initialize singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

        //initialize all sound objects AudioSources in sounds
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    //TODO: function to play music
    //TODO: function to fadeIn music
    //TODO: function to fadeOut music

    public void Play(string name)
    {
        //find sound searching by name
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        //check if sound with matching name exists in sounds
        if (s != null)
        {
            s.source.volume = s.volume * AudioOptions.Instance.effectsVolume;
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("No sound with name " + name + " was found");
            return;
        }
    }

    /*this function plays the sound of the voice if the currentCharactersDisplayed is a multiple of the soundFrequency*/
    public void PlayVoice(string name, int currentCharactersDisplayed, float soundFrequency, bool randomizePitch)
    {
        //find sound searching by name
        if (AudioOptions.Instance.voiceEffects)
        {

            Sound s = Array.Find(sounds, sound => sound.name.Equals(name));
            if (s != null)
            {
                if (currentCharactersDisplayed % soundFrequency == 0)
                {
                    s.source.Stop();

                    //if the pitch value to change is not zero
                    if (randomizePitch)
                    {
                        //take the random pitch variation
                        s.source.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
                    }
                    s.source.Play();
                }
            }
            else
            {
                Debug.LogWarning("No sound with name " + name + " was found");
                return;
            }
        }
    }


    public void PlayWithFadeIn(string name, float fadeInTime = 1)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s != null)
        {
            s.source.volume = 0;
            s.source.Play();
            StartCoroutine(FadeInSound(s.source, s.volume*AudioOptions.Instance.effectsVolume, fadeInTime));
        }
        else
        {
            Debug.LogWarning("No sound called " + name + " can be found");
            return;
        }
    }

    public void StopWithFadeOut(string name, float fadeInTime = 1)
    {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));

        if (s != null)
        {
            StartCoroutine(FadeOutSound(s.source, s.volume * AudioOptions.Instance.effectsVolume, fadeInTime));
        }
        else
        {
            Debug.LogWarning("No sound called " + name + " can be found");
            return;
        }
    }

    private IEnumerator FadeInSound(AudioSource audioSource, float maxAudio, float fadeTime = 1)
    {
        float startTime = Time.time;
        float endTime = startTime + fadeTime;
        while (Time.time < endTime && audioSource.volume < maxAudio)
        {
            float progress = ((Time.time - startTime) / fadeTime) * maxAudio; // maxAudio should be always between 0 and 1
            audioSource.volume = progress;
            yield return null;
        }
        audioSource.volume = maxAudio;
    }

    private IEnumerator FadeOutSound(AudioSource audioSource, float maxAudio, float fadeTime = 1)
    {
        float startTime = Time.time;
        float endTime = startTime + fadeTime;
        while (Time.time < endTime)
        {
            float progress = ((Time.time - startTime) / fadeTime) * maxAudio;
            audioSource.volume -= progress;
            yield return null;
        }
        audioSource.Stop();
    }
}
