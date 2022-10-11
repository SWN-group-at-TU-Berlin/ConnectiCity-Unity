using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<TutorialOjectsSetup> tutroialObjectsSetups;
    [SerializeField] RectTransform tutorialOpaquePanel;
    [SerializeField] RectTransform tutorialOpaquePanelCutout;
    [SerializeField] RectTransform dialogueBoxRect;
    [SerializeField] TextMeshProUGUI dialogueBoxText;
    [SerializeField] RectTransform arrow;

    int i = 0;

    private void Start()
    {
        SwithcTutorialPanels();
    }

    public void SwithcTutorialPanels()
    {
        TutorialOjectsSetup panelSetup = tutroialObjectsSetups[i];
        CopyRectTransform(tutorialOpaquePanel, panelSetup.tutorialOpaquePanel);
        CopyRectTransform(tutorialOpaquePanelCutout, panelSetup.tutorialOpaquePanelCutout);
        CopyRectTransform(dialogueBoxRect, panelSetup.dialogueBox);
        dialogueBoxText.text = panelSetup.text;
        CopyRectTransform(arrow, panelSetup.arrow);

        if(i == tutroialObjectsSetups.Count - 1)
        {
            i = 0;
        } else
        {
            i++;
        }
    }

    void CopyRectTransform(RectTransform destination, RectTransform toCopy)
    {
        destination.anchorMax = toCopy.anchorMax;
        destination.anchorMin = toCopy.anchorMin;
        destination.pivot = toCopy.pivot;
        destination.anchoredPosition = toCopy.anchoredPosition;
        destination.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, toCopy.rect.height);
        destination.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, toCopy.rect.width);
        destination.localScale = toCopy.localScale;
        destination.rotation = toCopy.rotation;
    }
}

[System.Serializable]
public class TutorialOjectsSetup
{
    public int tutorialPanelNumber;
    public string text;
    public RectTransform tutorialOpaquePanel;
    public RectTransform tutorialOpaquePanelCutout;
    public RectTransform dialogueBox;
    public RectTransform arrow;
}

