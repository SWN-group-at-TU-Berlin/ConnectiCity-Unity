using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioOptions : MonoBehaviour
{
    #region singleton instance
    private static AudioOptions instance;
    public static AudioOptions Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }
    #endregion

    public float effectsVolume = 1f;
    public float musicVolume = 1f;
    public bool voiceEffects = true;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
