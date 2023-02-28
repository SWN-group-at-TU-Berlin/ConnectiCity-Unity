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


    //list for music tracks;
    [SerializeField] Sound[] tracks;

    int i;
    bool canSwitchTrack = false;

    [Header("Random pitch limits:")]
    [SerializeField, Range(-3, 3)] float minPitch = 0f;
    [SerializeField, Range(-3, 3)] float maxPitch = 2f;

    Sound currentTrackPlaying;

    //TODO: function to start music

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

        foreach (Sound s in tracks)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }

        i = 0;
    }

    private void Update()
    {
        if (currentTrackPlaying != null)
        {
            if (currentTrackPlaying.source.isPlaying)
            {
                Debug.Log("Currently playing: " + currentTrackPlaying.name + " at volume: " + currentTrackPlaying.source.volume);
            }
            if (currentTrackPlaying.source.time > currentTrackPlaying.source.clip.length - 4 && canSwitchTrack)
            {
                canSwitchTrack = false;
                PlayNextTrack();
            }
        }
    }

    void PlayNextTrack()
    {
        if (i > tracks.Length - 1)
        {
            i = 0;
        }
        StartCoroutine(PlayTrack(tracks[i].name));
        i++;
    }

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
            StartCoroutine(FadeInSound(s.source, s.volume * AudioOptions.Instance.effectsVolume, fadeInTime));
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
        Debug.Log("Fading in: " + audioSource.name);
        audioSource.volume = 0;
        audioSource.Play();
        float startTime = Time.time;
        float endTime = startTime + fadeTime;
        while (Time.time < endTime && audioSource.volume < maxAudio)
        {
            float progress = ((Time.time - startTime) / fadeTime) * maxAudio; // maxAudio should be always between 0 and 1
            audioSource.volume = progress;
            yield return null;
        }
        audioSource.volume = maxAudio;
        canSwitchTrack = true;
    }

    private IEnumerator FadeOutSound(AudioSource audioSource, float maxAudio, float fadeTime = 1)
    {
        /*float startTime = Time.time;
        float endTime = startTime + fadeTime;
        while (Time.time < endTime)
        {
            float progress = ((Time.time - startTime) / fadeTime) * maxAudio;
            audioSource.volume -= progress;
            yield return null;
        }
        audioSource.Stop();*/
        Debug.Log("Fading out: " + audioSource.name);
        float startTime = Time.time;
        float endTime = startTime + fadeTime;
        float startVolume = audioSource.volume;
        while (startTime < endTime)
        {
            float progress = Time.deltaTime * startVolume;
            audioSource.volume -= progress;
            startTime += Time.deltaTime;
            yield return null;
        }
        audioSource.Stop();
    }

    public IEnumerator PlayTrack(string track)
    {
        Sound t = Array.Find(tracks, tr => tr.name.Equals(track));
        if (t != null)
        {
            t.source.volume = t.volume * AudioOptions.Instance.musicVolume;
            if (currentTrackPlaying != null)
            {
                if (currentTrackPlaying.source.isPlaying)
                {
                    StartCoroutine(FadeOutSound(currentTrackPlaying.source, currentTrackPlaying.volume, 3));
                    StartCoroutine(FadeInSound(t.source, t.source.volume, 3));
                }
                else
                {
                    canSwitchTrack = true;
                    StartCoroutine(FadeInSound(t.source, t.volume, 3));
                }
                yield return new WaitUntil(() => canSwitchTrack);
                currentTrackPlaying = t;
            }
            else
            {
                canSwitchTrack = true;
                StartCoroutine(FadeInSound(t.source, t.volume, 3));
                currentTrackPlaying = t;
            }
        }
        else
        {
            Debug.LogWarning("No track named " + track + " can be found");
        }
    }

    public void StopTrack()
    {
        if (currentTrackPlaying.source.isPlaying)
        {
            StartCoroutine(FadeOutSound(currentTrackPlaying.source, currentTrackPlaying.volume, 3f));
        }
    }

    public void ChangeCurrentMusicVolume()
    {
        if (currentTrackPlaying.source.isActiveAndEnabled)
        {
            currentTrackPlaying.source.volume = currentTrackPlaying.volume * AudioOptions.Instance.musicVolume;
        }
    }
}
