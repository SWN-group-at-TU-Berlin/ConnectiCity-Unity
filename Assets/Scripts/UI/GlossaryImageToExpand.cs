using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlossaryImageToExpand : MonoBehaviour
{
    [SerializeField] Transform startPoint; //the position from where the image will start the animation
    [SerializeField] float transitionTime = 5f;
    [SerializeField] float speed = 5f;
    [SerializeField] float scaleIncrease = 3.5f;

    bool move = false;
    Vector3 destination;
    Vector3 screenCenter;

    private RectTransform rect;
    private Image panel;
    private float panelAlpha;
    private Image[] imgs;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        panel = GetComponent<Image>();
        panelAlpha = panel.color.a;
        imgs = GetComponentsInChildren<Image>();

        // Get the width and height of the screen
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Set the width and height of the RectTransform component to the screen width and height
        //rect.sizeDelta = new Vector2(screenHeight, screenWidth);
    }

    private void OnEnable()
    {
        //make panel and all its children transparent
        Color panelNoAlpha = panel.color;
        panelNoAlpha.a = 0;
        panel.color = panelNoAlpha;
        foreach (Image img in imgs)
        {
            Color noAlpha = img.color;
            noAlpha.a = 0;
            img.color = noAlpha;
        }

        //scale it up
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float alphaToAddImgs = 0.05f;
        float totalAlpha = 0f;
        while (totalAlpha < 1)
        {
            if (panel.color.a < panelAlpha)
            {
                //increase panel
                Color panelCurrentColor = panel.color;
                panelCurrentColor.a += alphaToAddImgs;
                panel.color = panelCurrentColor;
            }

            foreach (Image img in imgs)
            {
                Color imgCurrentColor = img.color;
                imgCurrentColor.a += alphaToAddImgs;
                img.color = imgCurrentColor;
            }

            totalAlpha += alphaToAddImgs;
            yield return null;
        }
    }

    public void CloseImage()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float alphaToTakeFromImgs = 0.05f;
        float totalAlpha = 1f;
        while (totalAlpha > 0)
        {
            if (panel.color.a > 0)
            {
                //increase panel
                Color panelCurrentColor = panel.color;
                panelCurrentColor.a -= alphaToTakeFromImgs;
                panel.color = panelCurrentColor;
            }

            foreach (Image img in imgs)
            {
                Color imgCurrentColor = img.color;
                imgCurrentColor.a -= alphaToTakeFromImgs;
                img.color = imgCurrentColor;
            }

            totalAlpha -= alphaToTakeFromImgs;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
