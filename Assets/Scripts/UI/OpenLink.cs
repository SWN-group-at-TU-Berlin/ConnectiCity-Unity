using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OpenLink : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI tmpro;
    InputProvider input;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button.Equals(PointerEventData.InputButton.Left))
        {
            int i = TMP_TextUtilities.FindIntersectingLink(tmpro, eventData.position, null);
            if(i > -1)
            {
                Application.OpenURL(tmpro.textInfo.linkInfo[i].GetLinkID());
            }
        }
    }

    private void Awake()
    {
        tmpro = GetComponent<TextMeshProUGUI>();
        input = new InputProvider();
    }

    
}
