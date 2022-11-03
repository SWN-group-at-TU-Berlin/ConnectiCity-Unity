using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogueTrigger : MonoBehaviour
{
    [SerializeField] TextAsset inkDialogue;

    private void Start()
    {
        //try invoke to delay start call
        DialogueManager.GetInstance().EnterDialogueMode(inkDialogue);
    }
}
