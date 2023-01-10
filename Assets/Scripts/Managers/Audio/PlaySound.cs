using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This class plays calls the AudioManager to play the sound "soundName".
  The sound is played based on the "playSoundOn" variable that specifies
  the MonoBehaviour functions that triggers the sound.*/

public class PlaySound : MonoBehaviour
{
    [SerializeField] string soundName;
    [SerializeField] PlaySoundAction playSoundOn;

    // Start is called before the first frame update
    void Start()
    {
        if (playSoundOn.Equals(PlaySoundAction.onStart))
        {
            AudioManager.Instance.Play(soundName);
        }
    }

    private void OnEnable()
    {
        if (playSoundOn.Equals(PlaySoundAction.onEnable))
        {
            AudioManager.Instance.Play(soundName);
        }
    }

    private void OnDisable()
    {
        if (playSoundOn.Equals(PlaySoundAction.onDisable))
        {
            AudioManager.Instance.Play(soundName);
        }
    }
}

enum PlaySoundAction
{
    onEnable,
    onStart,
    onDisable
}
