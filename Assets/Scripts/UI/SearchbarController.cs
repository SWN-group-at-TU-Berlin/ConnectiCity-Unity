using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchbarController : MonoBehaviour
{
    private TMP_InputField searchBar;

    private void Awake()
    {
        searchBar = GetComponent<TMP_InputField>();
    }

    public void Clear()
    {
        searchBar.text = "";
    }
}
