using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    #region sinlgleton
    public static Tooltip Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        animator = GetComponent<Animator>();
    }
    #endregion

    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _message;
    [SerializeField] float staticTime;
    [SerializeField] float fadeTime;
    [Tooltip("Higer the value slower the fade effect")]
    [SerializeField] float fadeSpeed = 50;

    bool activated = false;
    Animator animator;
    InputProvider input;

    private void OnEnable()
    {
        input = new InputProvider();
        input.Enable();
    }

    public void Activated(bool b)
    {
        activated = b;
    }

    public IEnumerator Activate(string title, string message, Vector3 position)
    {
        yield return new WaitForSeconds(staticTime);
        if (activated)
        {

            _title.text = title;
            _message.text = message;
            RectTransform offset = GetComponent<RectTransform>();
            Vector3 mousePos = input.MousePosition();
            Vector3 newPosition = mousePos + new Vector3(offset.rect.width, offset.rect.height, transform.position.z);
            transform.position = newPosition;
            animator.Play("TooltipAppear");
        }
    }

    public void Deactivate()
    {
        if (!activated)
        {
            //animator.Play("TooltipDisappear");
        }
    }
}
