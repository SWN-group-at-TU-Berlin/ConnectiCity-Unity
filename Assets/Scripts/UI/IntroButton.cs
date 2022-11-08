using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroButton : MonoBehaviour
{
    private float timeFromStart;

    private void Start()
    {
        timeFromStart = Time.time;
    }
    void Update()
    {
        if (Time.time - timeFromStart > 2)
        {
            transform.GetChild(0).gameObject.SetActive(!DialogueManager.GetInstance().IntroPlaying && !DialogueManager.GetInstance().dialogueIsPlaying);
        }
    }
}
