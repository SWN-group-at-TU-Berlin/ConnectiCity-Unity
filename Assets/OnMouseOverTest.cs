using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMouseOverTest : MonoBehaviour
{
    private InputProvider input;

    private void OnEnable()
    {
        input = new InputProvider();
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(input.MousePosition());
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Raycast hit");
        }
    }
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }
}
