using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all Tabs
public class Tab : MonoBehaviour
{

    private void OnEnable()
    {
        PlayOpenTab(true);
    }

    private void OnDisable()
    {
        PlayOpenTab(false);
    }

    protected void PlayOpenTab(bool open)
    {
        if (open)
        {
            AudioManager.Instance.Play("OpenInfoTab");
        }
        else
        {
            AudioManager.Instance.Play("CloseInfoTab");
        }
    }
}
