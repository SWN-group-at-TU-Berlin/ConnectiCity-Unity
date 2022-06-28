using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] Sprite moneyIcon;
    [SerializeField] Sprite incomeIcon;
    [SerializeField] Sprite citizenSatisfactionIcon;

    TextMeshProUGUI floatingText;
    float floatingSpeed;
    float autodestructionTime;
    float fadeOutValue;
    bool startFloating = false;

    private void Awake()
    {
        floatingText = GetComponent<TextMeshProUGUI>();
    }

    public void SetupFloatingText(string txt, float speed, float fadeOutTime, float fadeOutVal, string iconType)
    {
        fadeOutValue = fadeOutVal;
        floatingSpeed = speed;
        floatingText.text = txt;
        autodestructionTime = fadeOutTime + Time.time;
        startFloating = true;

        switch (iconType)
        {
            case "b": //budget
                {
                    icon.sprite = moneyIcon;
                    break;
                }
            case "c": //citizen satisfaction
                {
                    icon.sprite = citizenSatisfactionIcon;
                    break;
                }
            case "i": //income
                {
                    icon.sprite = incomeIcon;
                    break;
                }
            default:
                {
                    Debug.LogWarning("The type " + iconType + " is not handled by 'SetupFloatingText()' function");
                    break;
                }

                icon.preserveAspect = true;
        }
        StartCoroutine(FloatingTxtFadeOut());
    }

    // Update is called once per frame
    IEnumerator FloatingTxtFadeOut()
    {
        Color tmpColor = floatingText.color;
        tmpColor.a = 1;
        float startTime = 0;
        while (tmpColor.a > 0)
        {
            transform.Translate(Vector3.up * Time.deltaTime * floatingSpeed);
            if (startTime < 2)
            {
                startTime += Time.deltaTime;
            }
            else
            {
                tmpColor.a -= fadeOutValue;
                floatingText.color = tmpColor;
            }
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}

