using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float voiceSoundFrequency = 2f;
    [SerializeField, Range(0,1)] private float voicepitchVariation = 0.10f;
    [SerializeField] bool randomizeVoice = false;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;
    [SerializeField] private Animator portraitAnimator;
    private Animator layoutAnimator;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    private Story currentStory;

    private Coroutine displayLineCoroutine;

    public bool dialogueIsPlaying { get; private set; }

    private bool canContinueToNextLine = false;
    private bool noMoreDialogue = false;
    private bool introPlaying = false;
    #region getter
    public bool IntroPlaying { get { return introPlaying; } }
    #endregion

    private string currentSpeaker;

    private static DialogueManager instance;

    private InputProvider input;

    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";
    private const string INTRO_TAG = "intro";
    private const string TUTORIAL_ANIMATOR_TAG = "animation";
    private const string TUTORIAL_SUBCATCHMENT_TAG = "subcatchment";
    private const string TUTORIAL_CUTOUT_TAG = "cutout";
    private const string TUTORIAL_ARROW_TAG = "arrow";
    private const string UI_TUTORIAL_FLAG_TAG = "tutorial";
    private const string UI_BUTTON_TAG = "buttons";
    private const string UI_TRAFFIC_BUTTON_TAG = "trafficbtn";
    private const string UI_RAIN_BUTTON_TAG = "rainbtn";

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        input = new InputProvider();

        layoutAnimator = dialoguePanel.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // get the layout animator

        // get all of the choices text 
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        // NOTE: The 'currentStory.currentChoiecs.Count == 0' part was to fix a bug after the Youtube video was made
        if (currentStory.currentChoices.Count == 0 && input.MouseLeftButton())
        {
            //ContinueStory();
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        if (!noMoreDialogue)
        {
            currentStory = new Story(inkJSON.text);
            dialogueIsPlaying = true;
            dialoguePanel.SetActive(true);

            // reset portrait, layout, and speaker
            displayNameText.text = "???";
            //portraitAnimator.Play("default");
            //layoutAnimator.Play("right");

            ContinueStory();
        }
    }

    private IEnumerator ExitDialogueMode()
    {
        //conversation end sound
        AudioManager.Instance.Play("ConversationEnd");
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            // set text for the current dialogue line
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }

            // handle tags
            //HandleTags(currentStory.currentTags);

            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // empty the dialogue text
        dialogueText.text = "";
        // hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        bool isAddingRichTextTag = false;

        // display each letter one at a time
        foreach (char letter in line.ToCharArray())
        {
            // if the submit button is pressed, finish up displaying the line right away
            if (input.MouseLeftButton())
            {
                dialogueText.text = line;
                break;
            }

            // check for rich text tag, if found, add it without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                AudioManager.Instance.PlayVoice(currentSpeaker, dialogueText.text.Length, voiceSoundFrequency, randomizeVoice);
                dialogueText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            // if not rich text, add the next letter and wait a small time
            else
            {
                AudioManager.Instance.PlayVoice(currentSpeaker, dialogueText.text.Length, voiceSoundFrequency, randomizeVoice);
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // actions to take after the entire line has finished displaying
        continueIcon.SetActive(true);
        DisplayChoices();

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    displayNameText.text = tagValue;
                    currentSpeaker = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(tagValue);
                    break;
                case INTRO_TAG:
                    introPlaying = tagValue.Equals("true");
                    break;
                case TUTORIAL_ANIMATOR_TAG:
                    TutorialManager.Instance.PlayAnimation(tagValue);
                    break;
                case TUTORIAL_SUBCATCHMENT_TAG:
                    if (tagValue.Equals("build"))
                    {
                        TutorialManager.Instance.BuildCommercialSubcatchmentTutorial();
                    }
                    break;
                case TUTORIAL_CUTOUT_TAG:
                    TutorialManager.Instance.TutorialCutoutSetActive(tagValue.Equals("active"));
                    break;
                case UI_BUTTON_TAG:
                    if (tagValue.Equals("enable"))
                    {
                        UIManager.Instance.ActivateButtons();
                    }
                    else
                    {
                        UIManager.Instance.DeactivateButtons();
                    }
                    break;
                case UI_TRAFFIC_BUTTON_TAG:
                    if (tagValue.Equals("enable"))
                    {
                        UIManager.Instance.ActivateTrafficButton();
                    }
                    break;
                case UI_RAIN_BUTTON_TAG:
                    if (tagValue.Equals("enable"))
                    {
                        UIManager.Instance.ActivateRainButton();
                    }
                    break;
                case TUTORIAL_ARROW_TAG:
                    if (tagValue.Contains("Point"))
                    {
                        TutorialManager.Instance.AnimatedArrowPlay(tagValue);
                    }
                    else
                    {
                        TutorialManager.Instance.DeactivateArrow();
                    }
                    break;
                case UI_TUTORIAL_FLAG_TAG:
                    if (tagValue.Equals("on"))
                    {
                        UIManager.Instance.ActivateTutorialMode();
                    }
                    else if (tagValue.Equals("end"))
                    {
                        TutorialManager.Instance.EndTutorial();
                    }
                    else
                    {
                        UIManager.Instance.DeactivateTutorialMode();
                    }
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > 0)
        {

            // defensive check to make sure our UI can support the number of choices coming in
            if (currentChoices.Count > choices.Length)
            {
                Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                    + currentChoices.Count);
            }

            int index = 0;
            // enable and initialize the choices up to the amount of choices for this line of dialogue
            foreach (Choice choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                choicesText[index].text = choice.text;
                index++;
            }
            // go through the remaining choices the UI supports and make sure they're hidden
            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }

            StartCoroutine(SelectFirstChoice());
        }
    }

    private IEnumerator SelectFirstChoice()
    {
        // Event System requires we clear it first, then wait
        // for at least one frame before we set the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        // NOTE: The below two lines were added to fix a bug after the Youtube video was made
        //InputManager.GetInstance().RegisterSubmitPressed(); // this is specific to my InputManager script
        ContinueStory();
    }

    public void EndTutorial()
    {
        StartCoroutine(ExitDialogueMode());
        TutorialManager.Instance.EndTutorial();
        noMoreDialogue = true;
    }

}