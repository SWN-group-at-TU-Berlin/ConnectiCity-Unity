using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GlossaryTitle : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] GameObject Paragraph;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.Play("BTNHover");
    }

    public void OpenParagraph()
    {
        if (!Paragraph.activeInHierarchy)
        {
            Paragraph.SetActive(true);
        }
    }
}
