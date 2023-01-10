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

    private void Awake()
    {
        #region singleton initialization
        //initialize singleton
        if (instance == null)
        {
            instance = this;
        } else
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

    public void Play(string name)
    {
        //find sound searching by name
        Sound s = Array.Find(sounds, sound => sound.name.Equals(name));
        
        //check if sound with matching name exists in sounds
        if(s != null)
        {
            s.source.Play();
        }
        else
        {
            Debug.LogWarning("No sound with name " + name + " was found");
            return;
        }
    }
}
