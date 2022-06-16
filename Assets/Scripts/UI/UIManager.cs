using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    #region singleton
    public static UIManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitializeButtonList();
        InitializeMessageBoard();
    }

    #endregion

    [Header("Resources References")]
    [SerializeField] TextMeshProUGUI budget;
    [SerializeField] TextMeshProUGUI income;
    [SerializeField] TextMeshProUGUI citizenNumber;
    [SerializeField] TextMeshProUGUI citizenSatisfaction;
    [SerializeField] TextMeshProUGUI ActionPoints;

    [Header("Buttons References")]
    [SerializeField] GameObject houseButtonDefault;
    [SerializeField] GameObject houseButtonDown;
    [SerializeField] GameObject businessButtonDefault;
    [SerializeField] GameObject businessButtonDown;
    [SerializeField] GameObject GRButtonDefault;
    [SerializeField] GameObject GRButtonDown;
    [SerializeField] GameObject RBButtonDefault;
    [SerializeField] GameObject RBButtonDown;
    [SerializeField] GameObject PPButtonDefault;
    [SerializeField] GameObject PPButtonDown;

    [Header("Info boards references")]
    [SerializeField] GameObject MessageBoard;
    [SerializeField] private float staticTime;
    [SerializeField] private float fadeOutTime;

    //Button variables
    List<GameObject> DefaultButtons;
    bool _houseButtonPressed = false;
    bool _businessButtonPressed = false;
    bool _GRButtonPressed = false;
    bool _PPButtonPressed = false;
    bool _RBButtonPressed = false;

    //Info boards variables
    Color fullAlphaBackground;
    Color fullAlphaTitle;
    Color fullAlphaMessage;

    //InputProvider reference
    InputProvider input;

    private void OnEnable()
    {
        input = new InputProvider();
        input.Enable();
    }

    private void OnDisable()
    {
        input = new InputProvider();
        input.Disable();
    }

    private void InitializeMessageBoard()
    {
        //Get default alpha color from message board
        fullAlphaBackground = MessageBoard.GetComponent<Image>().color;
        fullAlphaTitle = MessageBoard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color;
        fullAlphaMessage = MessageBoard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color;

        Color tmpColor = new Color();
        tmpColor.a = 0;
        //set message board color to transparent (alpha = 0)
        MessageBoard.GetComponent<Image>().color = tmpColor;
        MessageBoard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = tmpColor;
        MessageBoard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = tmpColor;

        MessageBoard.SetActive(false);
    }

    private void InitializeButtonList()
    {
        DefaultButtons = new List<GameObject>();
        DefaultButtons.Add(houseButtonDefault);
        DefaultButtons.Add(businessButtonDefault);
        DefaultButtons.Add(GRButtonDefault);
        DefaultButtons.Add(PPButtonDefault);
        DefaultButtons.Add(RBButtonDefault);
    }

    void ActivateDefaultButtons(GameObject pressedButton)
    {
        foreach (GameObject button in DefaultButtons)
        {
            if (!button.Equals(pressedButton))
            {
                button.GetComponent<Button>().enabled = true;
                button.GetComponent<Button>().interactable = true;
            }
        }
    }

    void DeactivateDefaultButtons(GameObject pressedButton)
    {
        foreach (GameObject button in DefaultButtons)
        {
            if (!button.Equals(pressedButton))
            {
                button.GetComponent<Button>().enabled = false;
                button.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void BusinessButtonPressed()
    {
        _businessButtonPressed = !_businessButtonPressed;
        businessButtonDown.SetActive(_businessButtonPressed);
        businessButtonDefault.SetActive(!_businessButtonPressed);
        if (businessButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(businessButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(businessButtonDefault);
        }
    }

    public void HouseButtonPressed()
    {
        _houseButtonPressed = !_houseButtonPressed;
        houseButtonDown.SetActive(_houseButtonPressed);
        houseButtonDefault.SetActive(!_houseButtonPressed);
        if (houseButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(houseButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(houseButtonDefault);
        }
    }

    public void GRButtonPressed()
    {
        _GRButtonPressed = !_GRButtonPressed;
        GRButtonDown.SetActive(_GRButtonPressed);
        GRButtonDefault.SetActive(!_GRButtonPressed);
        if (GRButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(GRButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(GRButtonDefault);
        }
    }

    public void PPButtonPressed()
    {
        _PPButtonPressed = !_PPButtonPressed;
        PPButtonDown.SetActive(_PPButtonPressed);
        PPButtonDefault.SetActive(!_PPButtonPressed);
        if (PPButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(PPButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(PPButtonDefault);
        }
    }

    public void RBButtonPressed()
    {
        _RBButtonPressed = !_RBButtonPressed;
        RBButtonDown.SetActive(_RBButtonPressed);
        RBButtonDefault.SetActive(!_RBButtonPressed);
        if (RBButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(RBButtonDefault);
        }
        else
        {
            DeactivateDefaultButtons(RBButtonDefault);
        }
    }

    public void MissingActionPointMessage()
    {
        foreach(Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message")) {
                child.GetComponent<TextMeshProUGUI>().text = "You don't have enough Action Points :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }
    
    public void MissingBudgetMessage()
    {
        foreach(Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message")) {
                child.GetComponent<TextMeshProUGUI>().text = "You don't have enough Budget :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    public void SubcatchmentNotBuiltMessage()
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "There are no built subcatchment :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    public void NoSubcatchmentCanHostBGIMessage()
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "The built subcatchments already have this BGI implemented :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    public void SubcatchmentAlreadyHostsBGI(InfrastructureType BGI)
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "You have already build " + BGI + "s on this subcatchment :D";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    private void OffsetMessageBoardPosition()
    {
        Vector2 offset = new Vector2(MessageBoard.GetComponent<RectTransform>().rect.width, -MessageBoard.GetComponent<RectTransform>().rect.height);
        MessageBoard.transform.position = input.MousePosition() + offset;
    }

    public void UpdateBudgetTxt(int newBudget){ budget.text = newBudget.ToString(); }
    public void UpdateIncomeTxt(int newIncome){ income.text = newIncome.ToString(); }
    public void UpdateCitizenNumberTxt(int newCN) { citizenNumber.text = newCN.ToString(); }
    public void UpdateCitizenSatisfactionTxt(int newCS) { citizenSatisfaction.text = newCS.ToString(); }
    public void UpdateActionPointsTxt(int newAP) { ActionPoints.text = newAP.ToString(); }

    IEnumerator FadeInfoBoardOut(GameObject boardToFade, float staticTime, float fadeTime)
    {

        MessageBoard.SetActive(true);

        //get components of objs to fade
        Image background = boardToFade.GetComponent<Image>();
        TextMeshProUGUI title = boardToFade.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI message = boardToFade.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        //increase alphas to full
        background.color = fullAlphaBackground;
        title.color = fullAlphaTitle;
        message.color = fullAlphaMessage;

        //wait for static time
        yield return new WaitForSeconds(staticTime);

        //Calculate amount to subtract from all alphas based on 100 iterations
        float alphaToSubtractFromBackground = background.color.a / 100;
        float alphaToSubtractFromTitle = title.color.a / 100;
        float alphaToSubtractFromMessage = message.color.a / 100;

        //declare color tmp variables to modify alphas
        Color tmpBackgroudColor = background.color;
        Color tmpTitleColor = title.color;
        Color tmpMessageColor = message.color;

        while(title.color.a > 0){

            //subtract alpha amount
            tmpBackgroudColor.a -= alphaToSubtractFromBackground;
            tmpTitleColor.a -= alphaToSubtractFromTitle;
            tmpMessageColor.a -= alphaToSubtractFromMessage;

            //assign new alpha value
            background.color = tmpBackgroudColor;
            title.color = tmpTitleColor;
            message.color = tmpMessageColor;

            yield return new WaitForSeconds(fadeTime/100);
        }

        //deactivate message board to avoid it covering clickable stuff
        MessageBoard.SetActive(false);

    }
}
