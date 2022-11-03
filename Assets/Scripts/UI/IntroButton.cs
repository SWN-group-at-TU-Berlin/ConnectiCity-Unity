using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroButton : MonoBehaviour
{
    void Update()
    {
        transform.GetChild(0).gameObject.SetActive(!DialogueManager.GetInstance().IntroPlaying && !DialogueManager.GetInstance().dialogueIsPlaying);
    }
}
