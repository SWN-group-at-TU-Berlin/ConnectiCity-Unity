using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    #region signleton
    private static TutorialManager instance;
    #region getter
    public static TutorialManager Instance { get { return instance; } }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] List<TutorialOjectsSetup> tutroialObjectsSetups;
    [SerializeField] RectTransform tutorialOpaquePanel;
    [SerializeField] RectTransform tutorialOpaquePanelCutout;
    [SerializeField] RectTransform dialogueBoxRect;
    [SerializeField] TextMeshProUGUI dialogueBoxText;
    [SerializeField] RectTransform arrow;
    [SerializeField] GameObject animatedArrow;


    int i = 0;

    private void Start()
    {
        UIManager.Instance.DeactivateButtons();
        SwithcTutorialPanels();
    }

    public void SwithcTutorialPanels()
    {

        animatedArrow.SetActive(false);
        tutorialOpaquePanel.gameObject.SetActive(true);
        arrow.gameObject.SetActive(true);
        dialogueBoxRect.gameObject.SetActive(true);

        TutorialOjectsSetup panelSetup = tutroialObjectsSetups[i];
        CopyRectTransform(tutorialOpaquePanel, panelSetup.tutorialOpaquePanel);
        CopyRectTransform(tutorialOpaquePanelCutout, panelSetup.tutorialOpaquePanelCutoff);
        CopyRectTransform(dialogueBoxRect, panelSetup.dialogueBox);
        dialogueBoxText.text = panelSetup.text;
        CopyRectTransform(arrow, panelSetup.arrow);

        if (tutroialObjectsSetups[i].tutorialPanelNumber == 1)
        {
            UIManager.Instance.ActivateTutorialInfrastructureButton();
            animatedArrow.SetActive(true);
            animatedArrow.GetComponent<Animator>().Play("PointInfrastructureButton", 0, 0f);
            tutorialOpaquePanel.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
            dialogueBoxRect.gameObject.SetActive(false);
            i++;
        }
        else if (tutroialObjectsSetups[i].tutorialPanelNumber == 2)
        {
            //deactivate buttons
            UIManager.Instance.DeactivateButtons();

            //activate rain button
            UIManager.Instance.ActivateRainButton();

            //deactivate tutorials panel
            tutorialOpaquePanel.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
            dialogueBoxRect.gameObject.SetActive(false);

            //activate animated arro
            animatedArrow.SetActive(true);

            //animated arrow play point rain button
            animatedArrow.GetComponent<Animator>().Play("PointRainButton", 0, 0f);

            i++;

            //In UIManager clicking rain button with tutorialOn should reactivate tutorial panels
        }
        else if (tutroialObjectsSetups[i].tutorialPanelNumber == 3)
        {
            UIManager.Instance.ActivateButtons();
            UIManager.Instance.DeactivateTutorialMode();
            tutorialOpaquePanel.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
            dialogueBoxRect.gameObject.SetActive(false);
        }
        else if (i < tutroialObjectsSetups.Count - 1)
        {
            i++;
        }
    }

    public void ArrowPointInfrastructureButton()
    {
        if (i < 18)
        {
            animatedArrow.GetComponent<Animator>().Play("PointInfrastructureButton", 0, 0f);
        }
    }

    public void ArrowPointSubcat7()
    {
        animatedArrow.GetComponent<Animator>().Play("Subcat7", 0, 0f);
    }

    public void ArrowPointBuildButton()
    {
        animatedArrow.GetComponent<Animator>().Play("PointBuildButton", 0, 0f);
    }

    public void DeactivateArrow()
    {
        animatedArrow.SetActive(false);
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
        ;
    }
}

[System.Serializable]
public class TutorialOjectsSetup
{
    public int tutorialPanelNumber;
    public string text;
    public RectTransform tutorialOpaquePanel;
    public RectTransform tutorialOpaquePanelCutoff;
    public RectTransform dialogueBox;
    public RectTransform arrow;
}

