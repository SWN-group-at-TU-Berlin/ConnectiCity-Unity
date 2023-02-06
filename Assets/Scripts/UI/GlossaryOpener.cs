using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlossaryOpener : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenClose()
    {
        animator.SetBool("Open", !animator.GetBool("Open"));
    }

    public bool IsOpen()
    {
        return animator.GetBool("Open");
    }
}
