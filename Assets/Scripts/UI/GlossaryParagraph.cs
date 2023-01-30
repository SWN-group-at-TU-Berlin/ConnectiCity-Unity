using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryParagraph : MonoBehaviour
{
    private Scrollbar scrollBar;

    private void Start()
    {
        scrollBar = GetComponentInChildren<Scrollbar>();

        if (scrollBar != null)
        {
            scrollBar.value = 1;
        }
        else
        {
            Debug.Log(name + " didn't found scrollbar");
        }
    }

    private void OnEnable()
    {
        if(scrollBar != null && scrollBar.value != 1)
        {
            scrollBar.value = 1;
        }
    }

    public void Close()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
    }
}
