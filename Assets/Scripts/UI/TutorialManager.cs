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

        tutorialDialoguesQ = new Queue<TextAsset>();
        foreach (TextAsset dialogue in tutorialDialoguesL)
        {
            tutorialDialoguesQ.Enqueue(dialogue);
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
    [SerializeField] GameObject skipTutorialButton;
    [SerializeField] Animator tutorialAnimator;

    [SerializeField] List<TextAsset> tutorialDialoguesL;
    [SerializeField] TextAsset TPt1HPNBC;
    [SerializeField] TextAsset TPt1HPBR;
    [SerializeField] TextAsset TPt1HPED;

    [SerializeField] TextAsset TPt2HPRBP;

    Queue<TextAsset> tutorialDialoguesQ;

    int i = 0;
    int tutorialDialogue = 0;
    #region getter
    public int TutorialDialogue { get { return tutorialDialogue; } }
    #endregion


    private void Start()
    {
        UIManager.Instance.DeactivateButtons();
        UIManager.Instance.ActivateTutorialMode();
        //SwithcTutorialPanels();
        NextTutorialDialogue();
    }

    public void NextTutorialDialogue()
    {
        DialogueManager.GetInstance().EnterDialogueMode(tutorialDialoguesQ.Dequeue());
        tutorialDialogue++;
    }

    public void HanldePlayer()
    {
        if (tutorialDialogue <= 2)
        {
            if (InfrastructureBuilder.Instance.SelectedSubcatchment != null)
            {
                if (InfrastructureBuilder.Instance.SelectedSubcatchment.Usage.Equals(AreaUsage.Residential))
                {
                    DialogueManager.GetInstance().EnterDialogueMode(TPt1HPBR);
                }
            }
            else
            {
                DialogueManager.GetInstance().EnterDialogueMode(TPt1HPNBC);
            }
        }
        else if (tutorialDialogue <= 3)
        {
            DialogueManager.GetInstance().EnterDialogueMode(TPt1HPED);
        }
        else if(TutorialDialogue == 5)
        {
            DialogueManager.GetInstance().EnterDialogueMode(TPt2HPRBP);
        }
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
            BuildCommercialSubcatchmentTutorial();
            tutorialOpaquePanel.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
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
            //Swap all of this with SkipTutorial()
            UIManager.Instance.ActivateButtons();
            UIManager.Instance.DeactivateTutorialMode();
            tutorialOpaquePanel.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
            dialogueBoxRect.gameObject.SetActive(false);
            skipTutorialButton.SetActive(false);
        }
        else if (i < tutroialObjectsSetups.Count - 1)
        {
            i++;
        }
    }

    public void BuildCommercialSubcatchmentTutorial()
    {
        UIManager.Instance.ActivateTutorialInfrastructureButton();
        animatedArrow.SetActive(true);
        animatedArrow.GetComponent<Animator>().Play("PointInfrastructureButton", 0, 0f);
    }

    /*This function should substitute all functions that control the animated arrow animator*/
    public void AnimatedArrowPlay(string anim)
    {
        if (!animatedArrow.activeInHierarchy)
        {
            animatedArrow.SetActive(true);
            animatedArrow.GetComponent<Animator>().enabled = true;
        }
        animatedArrow.GetComponent<Animator>().Play(anim, 0, 0f);
    }


    //DEPRECATED
    public void ArrowPointInfrastructureButton()
    {
        if (i < 18)
        {
            animatedArrow.GetComponent<Animator>().Play("PointInfrastructureButton", 0, 0f);
        }
    }

    //DEPRECATED
    public void ArrowPointSubcat7()
    {
        animatedArrow.GetComponent<Animator>().Play("PointSubcat7", 0, 0f);
    }

    //DEPRECATED
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

    public void SkipTutorial()
    {
        UIManager.Instance.ActivateButtons();
        UIManager.Instance.DeactivateTutorialMode();
        tutorialOpaquePanel.gameObject.SetActive(false);
        arrow.gameObject.SetActive(false);
        animatedArrow.gameObject.SetActive(false);
        dialogueBoxRect.gameObject.SetActive(false);
        skipTutorialButton.SetActive(false);
    }


    public void TutorialCutoutSetActive(bool status)
    {
        tutorialOpaquePanel.gameObject.SetActive(status);
    }


    public void PlayAnimation(string anim)
    {
        tutorialAnimator.Play(anim, 0, 0f);
    }

    public void EndTutorial()
    {
        SkipTutorial();
        tutorialDialogue = 10;
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

