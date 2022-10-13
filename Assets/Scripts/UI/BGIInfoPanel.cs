using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BGIInfoPanel : MonoBehaviour, IPointerExitHandler
{
    [SerializeField] GameObject BGIinfoPanel;

    // Update is called once per frame


    public void ActivateBGIInfoPanel()
    {
        if (!UIManager.Instance.TutorialOn)
        {
            BGIinfoPanel.SetActive(true);
        }
    }

    public void DeactivateBGIInfoPanel()
    {
        BGIinfoPanel.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DeactivateBGIInfoPanel();
    }
}
