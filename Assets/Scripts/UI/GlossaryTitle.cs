using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlossaryTitle : MonoBehaviour
{
    [SerializeField] GameObject Paragraph;


    public void OpenParagraph()
    {
        if (!Paragraph.activeInHierarchy)
        {
            Paragraph.SetActive(true);
        }
    }
}
