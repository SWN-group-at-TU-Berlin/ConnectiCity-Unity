using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipUser : MonoBehaviour
{
    [SerializeField] string title;
    [SerializeField] string message;

    InputProvider input;

    private void OnEnable()
    {
        input = new InputProvider();
        input.Enable();
    }

    public void ShowTooltip()
    {
        StartCoroutine(Tooltip.Instance.Activate(title, message, input.MousePosition()));
    }
}
