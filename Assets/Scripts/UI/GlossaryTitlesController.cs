using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlossaryTitlesController : MonoBehaviour
{
    [SerializeField] List<GameObject> titles;

    private void Start()
    {
        foreach(Transform child in transform)
        {
            titles.Add(child.gameObject);
        }
    }

    public void SearchTitle(string titleName)
    {
        if (titleName.Length > 0)
        {
            foreach (GameObject title in titles)
            {
                if (title.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Contains(titleName))
                {
                    title.SetActive(true);
                }
                else
                {
                    title.SetActive(false);
                }
            }
        }
        else
        {
            foreach (GameObject title in titles)
            {
                title.SetActive(true);
            }
        }
    }
}
