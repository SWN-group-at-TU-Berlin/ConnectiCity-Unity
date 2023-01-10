using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

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
        InitializeInfoPanels();
    }

    private void InitializeInfoPanels()
    {
        infoPanelsNotInUse = new Queue<Transform>();
        infoPanelsInUse = new Queue<Transform>();
        foreach (Transform infoPanel in InfoPanels)
        {
            infoPanel.gameObject.SetActive(false);
            infoPanelsNotInUse.Enqueue(infoPanel);
        }
    }

    #endregion

    [Header("Resources References")]
    [SerializeField] TextMeshProUGUI GeneralBudget;
    [SerializeField] TextMeshProUGUI BGIsBudget;
    [SerializeField] TextMeshProUGUI GeneralIncrease;
    [SerializeField] TextMeshProUGUI BGIsIncrease;
    [SerializeField] TextMeshProUGUI Citizens;
    [SerializeField] TextMeshProUGUI Workers;
    [SerializeField] TextMeshProUGUI CitizensGrowth;
    [SerializeField] TextMeshProUGUI WorkersGrowth;
    [SerializeField] TextMeshProUGUI HostableCitizens;
    [SerializeField] TextMeshProUGUI JobsAvailable;
    [SerializeField] TextMeshProUGUI PopulationDensityText;
    [SerializeField] TextMeshProUGUI UnemploymentPercentageText;
    [SerializeField] TextMeshProUGUI FlashFloodRiskText;
    [SerializeField] TextMeshProUGUI EmissionsText;
    [SerializeField] TextMeshProUGUI TrafficText;
    [SerializeField] TextMeshProUGUI RoundText;

    [Header("Buttons References")]
    [SerializeField] GameObject InfrasructureButton;
    [SerializeField] GameObject RoundButton;
    [SerializeField] GameObject SocialButton;
    [SerializeField] GameObject GRButtonDefault;
    [SerializeField] GameObject RBButtonDefault;
    [SerializeField] GameObject PPButtonDefault;
    [SerializeField] GameObject RainButton;
    [SerializeField] GameObject TrafficButton;

    //DEPRECATED
    [SerializeField] GameObject houseButtonDown;
    [SerializeField] GameObject businessButtonDefault;
    [SerializeField] GameObject businessButtonDown;
    [SerializeField] GameObject GRButtonDown;
    [SerializeField] GameObject RBButtonDown;

    [Header("Info boards references and properties")]
    [SerializeField] GameObject MessageBoard;
    [SerializeField] GameObject InfoTab;
    [SerializeField] GameObject RainInfoTab;
    [SerializeField] GameObject TrafficInfoTab;
    [SerializeField] GameObject RainDistributionPanel;
    [SerializeField] GameObject RainDistributionGraph;
    [SerializeField] LayerMask UILayer;
    [SerializeField] private float staticTime;
    [SerializeField] private float fadeOutTime;
    [Tooltip("parent obj of all info panels")]
    [SerializeField] Transform InfoPanels;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] Color CanonGreen;
    [SerializeField] Color CanonRed;
    [SerializeField] float tooltipCooldown = 2;
    [SerializeField] Sprite jobs;
    [SerializeField] Sprite house;
    [SerializeField] Sprite runoff;
    [SerializeField] Sprite money;


    [Header("Floating text references")]
    [SerializeField] GameObject floatingTextParent;
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] float floatingSpeed;
    [SerializeField] float txtFadeOutTime;

    [Header("Score references and parameters")]
    [SerializeField] GameObject scoreGraph;
    [SerializeField] GameObject scoreBoard;
    [SerializeField] ScoreSummary scoreSummary;
    [SerializeField] Animator RainEventInfoPanelAnimator;
    [SerializeField] float timeToWaitBeforeNextAnimation = 5f;
    [SerializeField] Slider populationDensity;
    [SerializeField] Slider unemploymentRate;
    [SerializeField] Slider flashFloodRisk;
    [SerializeField] Slider predictedRunoff;
    [SerializeField] Slider actualRunoff;
    [SerializeField] Slider trafficIntensity;
    [SerializeField] Slider emissions;
    [SerializeField] TextMeshProUGUI actualPrecipitation;
    [SerializeField] TextMeshProUGUI predictedPrecipitation;
    [SerializeField] float sliderFillingTime = 3f;
    [SerializeField] float flashFloodRiskSliderCap = 744f;

    [Header("VFXs")]
    [SerializeField] Transform flashFloodVfx;


    UIState uiState = UIState.Social;
    #region getter
    public UIState UIState { get { return uiState; } }
    #endregion

    //Button variables
    List<GameObject> DefaultButtons;
    InfrastructureType infrastructureTypeButtonPressed;
    #region getter
    public InfrastructureType InfrastructureTypeButtonPressed { get { return infrastructureTypeButtonPressed; } }
    #endregion
    bool _houseButtonPressed = false;
    bool _businessButtonPressed = false;
    bool _buildMode = false;
    bool _canPlayNextAnimation = false;
    bool _showingRainEventInfos = false;
    bool _showingRainDistributionGraph = false;
    bool _showingRunoffReduction = false;
    #region getter
    public bool ShowingRunoffReduction { get { return _showingRunoffReduction; } }
    #endregion
    bool _tutorialOn = false;
    #region getter
    public bool TutorialOn { get { return _tutorialOn; } }
    #endregion


    //Info boards variables
    Color fullAlphaBackground;
    Color fullAlphaTitle;
    Color fullAlphaMessage;
    Queue<Transform> infoPanelsNotInUse; // deactivated
    Queue<Transform> infoPanelsInUse; // active
    float tooltipCooldownTimer;
    int lastSubcatchmentCallingInfoTab = -1;

    //InputProvider reference
    InputProvider input;

    //DEPREATED
    bool _GRButtonPressed = false;
    bool _PPButtonPressed = false;
    bool _RBButtonPressed = false;

    private void Start()
    {
        ChangeButtonColorToPressed(SocialButton.GetComponent<Button>());

        flashFloodRisk.minValue = 0;
        flashFloodRisk.maxValue = RainEventsManager.Instance.FlashFloodThreshold;
        flashFloodRisk.value = Mathf.Clamp(RainEventsManager.Instance.CalculateFlashFloorRisk(), 0f, flashFloodRiskSliderCap);
        UpdateFlashFloodRiskTxt(flashFloodRisk.value.ToString("F2"));
    }

    private void Update()
    {
        //TooltipShow();
        if (input.PauseButton())
        {
            TogglePauseMenu();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(input.MousePosition());

        //if mouse not hitting subcatchments or UI
        if (!Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
        {
            //If mosue button pressed
            if (input.MouseLeftButton())
            {
                //if you are not in runoff reduction showing mode
                if (uiState.Equals(UIState.Social))
                {
                    //then exit the build mode
                    ExitBuildMode();
                }
                else //if you are in runoff reduction mode
                {
                    //hide the info tab
                    if (!_tutorialOn)
                    {
                        RainInfoTab.SetActive(false);
                    }

                    TrafficInfoTab.SetActive(false);

                    // if you are showing the graph
                    if (_showingRainDistributionGraph)
                    {
                        //hide it [name of the function is counterintuitive]
                        ShowRainDistributionGraph();
                    }
                }

            }
        } //If you are just not on the UI
        else if (!EventSystem.current.IsPointerOverGameObject())
        {
            //and pressig the mouse button
            if (input.MouseLeftButton())
            {
                // if you are showing the graph
                if (_showingRainDistributionGraph)
                {
                    //hide it [name of the function is counterintuitive]
                    ShowRainDistributionGraph();
                }
            }
        }
    }

    public void TogglePauseMenu()
    {
        PauseMenu.SetActive(!PauseMenu.activeInHierarchy);
    }

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
        DefaultButtons.Add(InfrasructureButton);
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

    public void ShowTrafficInfoTab(int subcatNum)
    {
        //recover current budget
        float currentBgt = ResourceManager.Instance.BGIbudget;

        //recover pt cost
        float costExample = CostsManager.Instance.TransportCosts[subcatNum];

        //calculate new budget
        float newBudget = currentBgt - costExample;

        //call traffic info tab update text
        TrafficInfoTab.SetActive(true);
        TrafficInfoTab.GetComponent<TrafficInfoTab>().UpdateTexts(
            MapManager.Instance.GetSubcatchment(subcatNum).CanHostPublicTransport(),
            subcatNum.ToString(),
            currentBgt.ToString("F0"),
            costExample.ToString("F0"),
            newBudget.ToString("F0"));
    }

    public void HideTrafficInfoTab()
    {
        TrafficInfoTab.SetActive(false);
    }

    private void EnteringBuildMode(InfrastructureType infrastructureTypeToHandle)
    {
        if ((TutorialManager.Instance.TutorialDialogue <= 2 && !infrastructureTypeToHandle.Equals(InfrastructureType.Building))
            || TutorialManager.Instance.TutorialDialogue == 3)
        {
            TutorialManager.Instance.HanldePlayer();
            return;
        }
        //infrastructure type button pressed is null if you exited build mode without switching between infrastructure an bgis
        if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Null) || infrastructureTypeButtonPressed.Equals(infrastructureTypeToHandle))
        {
            _buildMode = !_buildMode;
        }


        if (_buildMode)
        {
            AudioManager.Instance.Play(InfrastructureType.Building.ToString() + "ButtonPressed");
            SocialButtonPressed();
            infoPanelsNotInUse.Clear();
            foreach (Transform infoPanel in InfoPanels)
            {
                infoPanelsNotInUse.Enqueue(infoPanel);
            }
            infrastructureTypeButtonPressed = infrastructureTypeToHandle;
            if (_tutorialOn)
            {
                InfrastructureBuilder.Instance.EnterTutorialBuildStatus();
            }
            else if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Building))
            {
                InfrastructureBuilder.Instance.EnterInfrastructureBuildStatus();
            }
            else
            {
                InfrastructureBuilder.Instance.EnterBGIBuildStatus(infrastructureTypeButtonPressed);
            }
            //switch to new info tab info
            if (InfoTab.activeInHierarchy)
            {
                if (MapManager.Instance.GetSubcatchment(lastSubcatchmentCallingInfoTab).IsHighlighted)
                {
                    MapManager.Instance.GetSubcatchment(lastSubcatchmentCallingInfoTab).UpdateInfoTabInfrastructure();
                }
                else
                {
                    HideInfoTab();
                }
            }

        }
        else
        {
            ExitBuildMode();
        }
    }

    public void InfrastructureButtoPressed()
    {
        EnteringBuildMode(InfrastructureType.Building);
    }

    public void ExitBuildMode()
    {
        if (_tutorialOn)
        {
            TutorialManager.Instance.ArrowPointInfrastructureButton();
        }
        _buildMode = false;
        MapManager.Instance.DehighlightBuildableSubcatchments();
        HideInfoPanels();
        HideInfoTab();
        infrastructureTypeButtonPressed = InfrastructureType.Null;
        InfrastructureBuilder.Instance.ResetSelectedSubcatchment();
    }

    //DRPRECATED
    public void BusinessButtonPressed()
    {
        _businessButtonPressed = !_businessButtonPressed;
        businessButtonDown.SetActive(_businessButtonPressed);
        businessButtonDefault.SetActive(!_businessButtonPressed);
        if (businessButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(businessButtonDefault);
            HideInfoPanels();
        }
        else
        {
            DeactivateDefaultButtons(businessButtonDefault);
        }
    }

    //DRPRECATED
    public void HouseButtonPressed()
    {
        _houseButtonPressed = !_houseButtonPressed;
        houseButtonDown.SetActive(_houseButtonPressed);
        InfrasructureButton.SetActive(!_houseButtonPressed);
        if (InfrasructureButton.activeInHierarchy)
        {
            ActivateDefaultButtons(InfrasructureButton);
            HideInfoPanels();
        }
        else
        {
            DeactivateDefaultButtons(InfrasructureButton);
        }
    }

    public void GRButtonPressed()
    {
        EnteringBuildMode(InfrastructureType.GR);
    }

    public void PPButtonPressed()
    {
        EnteringBuildMode(InfrastructureType.PP);
    }

    public void RBButtonPressed()
    {
        EnteringBuildMode(InfrastructureType.RB);

    }

    public void MissingActionPointMessage()
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "You don't have enough Action Points :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    public void MissingBudgetMessage()
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
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

    public void BuildMoreResidentialSubcatchmentsMessage()
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "The number of Residential subcatchment must be higher than the number of Commercial subcatchment in order to build a new one :(";
            }
        }
        OffsetMessageBoardPosition();
        StartCoroutine(FadeInfoBoardOut(MessageBoard, staticTime, fadeOutTime));
    }

    public void NotEnoughResourceToBuildInfrastructureMessage(AreaUsage subcatUsage)
    {
        foreach (Transform child in MessageBoard.transform)
        {
            if (child.name.Equals("Message"))
            {
                child.GetComponent<TextMeshProUGUI>().text = "You don't have enough resources to build a new " + subcatUsage.ToString() + " subcatchment :(";
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

    #region Text update functions
    public void UpdateGeneralBudgetTxt(string val) { GeneralBudget.text = val; }
    public void UpdateBGIsBudgetTxt(string val) { BGIsBudget.text = val; }
    public void UpdateGeneralIncreaseTxt(string val) { GeneralIncrease.text = val; }
    public void UpdateBGIsIncreaseTxt(string val) { BGIsIncrease.text = val; }
    public void UpdateCitizensTxt(string val) { Citizens.text = val; }
    public void UpdateWorkersTxt(string val) { Workers.text = val; }
    public void UpdateCitizensGrowthTxt(string val) { CitizensGrowth.text = val; }
    public void UpdateWorkersGrowthTxt(string val) { WorkersGrowth.text = val; }
    public void UpdateHostablePeopleTxt(string val) { HostableCitizens.text = val; }
    public void UpdateJobsAvailableTxt(string val) { JobsAvailable.text = val; }
    public void UpdatePopulationDensityTxt(string val) { PopulationDensityText.text = val; }
    public void UpdateUnemploymentPercentageTxt(string val) { UnemploymentPercentageText.text = val + "%"; }
    public void UpdateFlashFloodRiskTxt(string val) { FlashFloodRiskText.text = val + "%"; }
    public void UpdateEmissionsTxt(string val) { EmissionsText.text = val + "kg"; }
    public void UpdateTrafficTxt(string val) { TrafficText.text = val + "%"; }
    public void UpdateRoundTxt(string val) { RoundText.text = "Round " + val; }
    #endregion

    public void ShowSocialInfoPanel(AreaUsage subcatType, InfrastructureType infrastructureType, Vector3 position, int actionPoints, int budget, int income, int citizenSatisfaction, int citizenNumber)
    {
        if (infoPanelsNotInUse.Count > 0)
        {

            Transform infoPanel = infoPanelsNotInUse.Dequeue();
            infoPanel.gameObject.SetActive(true);
            foreach (Transform textField in infoPanel)
            {
                if (textField.name.Equals("AP"))
                {
                    textField.GetComponent<TextMeshProUGUI>().text = actionPoints.ToString();
                    textField.GetComponent<TextMeshProUGUI>().color = CanonRed;
                }
                if (textField.name.Equals("CS"))
                {
                    textField.GetComponent<TextMeshProUGUI>().text = "+" + citizenSatisfaction.ToString();
                    textField.GetComponent<TextMeshProUGUI>().color = CanonGreen;
                }
                if (textField.name.Equals("CN"))
                {
                    textField.GetComponent<TextMeshProUGUI>().text = "+" + citizenNumber.ToString();
                    textField.GetComponent<TextMeshProUGUI>().color = CanonGreen;
                }
                if (textField.name.Equals("Budget"))
                {
                    textField.GetChild(0).gameObject.SetActive(true);
                    textField.GetChild(0).GetComponent<Image>().sprite = money;
                    textField.GetComponent<TextMeshProUGUI>().text = "-" + budget.ToString();
                    textField.GetComponent<TextMeshProUGUI>().color = CanonRed;
                }
                if (textField.name.Equals("Benefit"))
                {
                    textField.GetComponent<TextMeshProUGUI>().text = "+" + income;
                    textField.GetComponent<TextMeshProUGUI>().color = CanonGreen;
                    textField.GetChild(0).gameObject.SetActive(true);
                    if (infrastructureType.Equals(InfrastructureType.Building))
                    {

                        if (subcatType.Equals(AreaUsage.Residential))
                        {
                            textField.GetChild(0).GetComponent<Image>().sprite = house;
                        }
                        else if (subcatType.Equals(AreaUsage.Commercial))
                        {
                            textField.GetChild(0).GetComponent<Image>().sprite = jobs;
                        }
                    }
                    else
                    {
                        textField.GetChild(0).GetComponent<Image>().sprite = runoff;
                    }
                }
            }

            infoPanel.transform.position = Camera.main.WorldToScreenPoint(position);
            infoPanel.transform.position = new Vector3(infoPanel.position.x, infoPanel.position.y, 0f);
            infoPanelsInUse.Enqueue(infoPanel);
        }
        else
        {
            Debug.LogWarning("InfoPanelNotInUse empty");
        }

    }

    public void ShowRainInfoPanel(float runoffReduction, Vector3 position)
    {
        if (infoPanelsNotInUse.Count > 0)
        {
            Transform infoPanel = infoPanelsNotInUse.Dequeue();
            infoPanel.gameObject.SetActive(true);
            foreach (Transform textField in infoPanel)
            {
                if (textField.name.Equals("Budget"))
                {
                    textField.GetChild(0).GetComponent<Image>().sprite = runoff;
                    textField.GetComponent<TextMeshProUGUI>().text = "Reduction";
                }
                if (textField.name.Equals("Benefit"))
                {
                    textField.GetComponent<TextMeshProUGUI>().color = Color.blue;
                    textField.GetComponent<TextMeshProUGUI>().text = runoffReduction + "%";
                    textField.GetChild(0).gameObject.SetActive(false);
                }
            }
            infoPanel.transform.position = Camera.main.WorldToScreenPoint(position);
            infoPanel.transform.position = new Vector3(infoPanel.position.x, infoPanel.position.y, 0f);
            infoPanelsInUse.Enqueue(infoPanel);
        }
        else
        {
            Debug.LogWarning("InfoPanelNotInUse empty");
        }
    }

    public void ShowRunoffReductions()
    {
        _showingRunoffReduction = true;
        if (_showingRunoffReduction)
        {
            foreach (Subcatchment subcat in MapManager.Instance.GetBuiltSubcatchments())
            {
                subcat.ShowRunoffReductionReductionInfo();
            }
        }
    }

    public void HideRunoffReductions()
    {
        RainInfoTab.SetActive(false);
        _showingRunoffReduction = false;
        infoPanelsNotInUse.Clear();
        infoPanelsInUse.Clear();
        foreach (Transform infoPanel in InfoPanels)
        {
            infoPanel.gameObject.SetActive(false);
            infoPanelsNotInUse.Enqueue(infoPanel.transform);
        }
    }

    public void HideInfoPanels()
    {
        infoPanelsInUse.Clear();
        foreach (Transform infoPanel in InfoPanels)
        {
            if (infoPanel.gameObject.activeInHierarchy)
            {
                infoPanel.gameObject.SetActive(false);
                infoPanelsNotInUse.Enqueue(infoPanel);
            }
        }
    }

    IEnumerator FadeInfoBoardOut(GameObject boardToFade, float staticTime, float fadeTime)
    {

        boardToFade.SetActive(true);

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

        while (title.color.a > 0)
        {

            //subtract alpha amount
            tmpBackgroudColor.a -= alphaToSubtractFromBackground;
            tmpTitleColor.a -= alphaToSubtractFromTitle;
            tmpMessageColor.a -= alphaToSubtractFromMessage;

            //assign new alpha value
            background.color = tmpBackgroudColor;
            title.color = tmpTitleColor;
            message.color = tmpMessageColor;

            yield return new WaitForSeconds(fadeTime / 100);
        }

        //deactivate message board to avoid it covering clickable stuff
        boardToFade.SetActive(false);

    }


    public void ShowFloatingTxt(float valueToShow, string resourceAffected, Subcatchment subcatAffected)
    {
        Vector3 tmpTxtPos = Camera.main.WorldToScreenPoint(subcatAffected.GetInfoPanelPosition());
        Vector3 txtPos = new Vector3(tmpTxtPos.x, tmpTxtPos.y, 0f);
        /*foreach(Transform tmp in floatingTextParent.transform)
        {
            if (tmp.name.Contains(subcatAffected.SubcatchmentNumber.ToString()))
            {
                txtPos = tmp.position;
            }
        }*/
        GameObject floatingTxtInstance = Instantiate(floatingTextPrefab, txtPos, Quaternion.identity, InfoPanels.transform.parent);
        //GameObject floatingTxtInstance = Instantiate(floatingTextPrefab, Vector3.zero, Quaternion.identity, InfoPanels.transform.parent);
        //floatingTxtInstance.transform.position = txtPos;
        floatingTxtInstance.SetActive(true);
        TextMeshProUGUI textMesh = floatingTxtInstance.GetComponent<TextMeshProUGUI>();
        if (valueToShow > 0)
        {
            textMesh.color = CanonGreen;
        }
        else
        {
            textMesh.color = CanonRed;
        }

        floatingTxtInstance.GetComponent<FloatingText>().SetupFloatingText(valueToShow.ToString(), floatingSpeed, fadeOutTime, 0.01f, resourceAffected);

    }

    void TooltipShow()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = input.MousePosition();

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResultList);
        if (raycastResultList.Count > 0)
        {
            if (tooltipCooldownTimer > tooltipCooldown)
            {
                for (int i = 0; i < raycastResultList.Count; i++)
                {
                    if (raycastResultList[i].gameObject.GetComponent<ToolTipUser>() != null)
                    {
                        raycastResultList[i].gameObject.GetComponent<ToolTipUser>().ShowTooltip();
                    }
                }
            }
            else
            {
                tooltipCooldownTimer += Time.deltaTime;
            }
        }
        else
        {
            tooltipCooldown = 0;
            //StartCoroutine(Tooltip.Instance.Deactivate());
        }
    }

    public void ShowInfoTabInfrastructure(float subcatchmentNumber, InfrastructureType infrastructureType, BuildStatus infrastructureToBuild, float buildCost, float benefit)
    {
        if (!InfoTab.activeInHierarchy)
        {
            InfoTab.SetActive(true);
            lastSubcatchmentCallingInfoTab = (int)subcatchmentNumber;

            //show animated arrow if tutorial on
            TutorialManager.Instance.ArrowPointBuildButton();
        }

        /*THIS IS A QUICK N DIRTY FIX TO CONVERT THE ENUM INTO A STRING
         IT SHOULD BE ADDRESSED IN A MORE GENERAL WAY PLZ*/
        string infrastructureTypeStr = "";
        string benefitLable = "";
        float currentBenefit = ResourceManager.Instance.Jobs;

        switch (infrastructureType)
        {
            case InfrastructureType.Business:
                {
                    infrastructureTypeStr = "Commercial Building";
                    benefitLable = "Jobs increase:";
                    break;
                }
            case InfrastructureType.House:
                {
                    infrastructureTypeStr = "Residential Building";
                    benefitLable = "Hostable people increase:";
                    currentBenefit = ResourceManager.Instance.HostablePeople;
                    break;
                }
        }

        float currentBudget = ResourceManager.Instance.Budget;
        InfoTab.GetComponent<InfoTab>().UpdateTextFieldsInfrastructure(subcatchmentNumber, infrastructureTypeStr, currentBudget, buildCost, currentBenefit, benefit, benefitLable);

    }

    public void ShowInfoTabBGI(float subcatchmentNumber, InfrastructureType infrastructureType, BuildStatus infrastructureToBuild, float buildCost, Dictionary<int, float> newRunoffReductions, Dictionary<int, float> currentRunoffReductions)
    {
        if (!InfoTab.activeInHierarchy)
        {
            InfoTab.SetActive(true);
        }

        lastSubcatchmentCallingInfoTab = (int)subcatchmentNumber;

        /*THIS IS A QUICK N DIRTY FIX TO CONVERT THE ENUM INTO A STRING
         IT SHOULD BE ADDRESSED IN A MORE GENERAL WAY PLZ*/
        string infrastructureTypeStr = ConvertBGIToString(infrastructureType, infrastructureToBuild);

        float currentBudget = ResourceManager.Instance.BGIbudget;
        float currentap = ResourceManager.Instance.ActionPoints;
        InfoTab.GetComponent<InfoTab>().UpdateTextFieldsBGI(subcatchmentNumber, infrastructureTypeStr, currentBudget, buildCost, currentRunoffReductions, newRunoffReductions);

    }

    private string ConvertBGIToString(InfrastructureType infrastructureType, BuildStatus infrastructureToBuild)
    {
        string infrastructureTypeStr = "";

        switch (infrastructureType)
        {
            case InfrastructureType.GR:
                {
                    //recover gr percentage of subcat
                    string specs = IdentifySpecsOfBGI(infrastructureToBuild);
                    //add it to the infrastructure type str
                    infrastructureTypeStr = "Green Roof " + specs + "% coverage";
                    break;
                }
            case InfrastructureType.PP:
                {
                    infrastructureTypeStr = "Permeable Pavement";
                    break;
                }
            case InfrastructureType.RB:
                {
                    //recover rb type (1 or 2)
                    string specs = IdentifySpecsOfBGI(infrastructureToBuild);
                    //add it to the infrastructure type str
                    infrastructureTypeStr = "Rain Barrel " + specs;
                    break;
                }
        }

        return infrastructureTypeStr;
    }

    public void HideInfoTab()
    {
        InfoTab.SetActive(false);
    }


    /*This function returns the specifi percentage of coverage of roofs
      in case of a GR or the specific type in case of a RB
     */
    string IdentifySpecsOfBGI(BuildStatus bgi)
    {
        string specs = "";
        //if it's a GR
        if (bgi.ToString().Contains("GR"))
        {
            specs = bgi.ToString().TrimStart('G');
            specs = specs.ToString().TrimStart('R');
        }//if it's a RB
        else if (bgi.ToString().Equals("RB1"))
        {
            specs = "small tanks";
        }
        else if (bgi.ToString().Equals("RB2"))
        {
            specs = "big tanks";
        }

        return specs;
    }

    public void SetMaxMinPopulationDensity(float max, float min)
    {
        populationDensity.maxValue = max;
        populationDensity.minValue = min;
    }

    public void SetMaxMinUnemployment(float max, float min)
    {
        unemploymentRate.maxValue = max;
        unemploymentRate.minValue = min;
    }

    public void UpdatePopulationDesitySlider(float popDensity)
    {
        populationDensity.value = popDensity;
        UpdatePopulationDensityTxt(popDensity.ToString("F2"));
    }

    public void UpdateUnemploymentSlider(float unmlpRate)
    {
        unemploymentRate.value = unmlpRate;
        UpdateUnemploymentPercentageTxt(unmlpRate.ToString("F2"));
    }

    public void UpdateTrafficSlider()
    {
        trafficIntensity.value = TrafficManager.Instance.GetTrafficIntensity();
        UpdateTrafficTxt(TrafficManager.Instance.GetTrafficIntensityPercentage().ToString("F2"));
    }

    public void ChangeButtonColorToPressed(Button btn)
    {
        Sprite currentSprite = btn.image.sprite;
        SpriteState tmpState;
        tmpState.pressedSprite = currentSprite;
        btn.image.sprite = btn.spriteState.pressedSprite;
        btn.spriteState = tmpState;
    }

    public void SocialButtonPressed()
    {
        if (!uiState.Equals(UIState.Social))
        {
            if (uiState.Equals(UIState.Rain))
            {
                ChangeButtonColorToPressed(RainButton.GetComponent<Button>());
            }
            else
            {
                ChangeButtonColorToPressed(TrafficButton.GetComponent<Button>());
            }
            HideRunoffReductions();
            ChangeButtonColorToPressed(SocialButton.GetComponent<Button>());
            uiState = UIState.Social;
            unemploymentRate.gameObject.SetActive(true);
            populationDensity.gameObject.SetActive(true);
            flashFloodRisk.gameObject.SetActive(false);
            RainDistributionPanel.SetActive(false);
            RainDistributionGraph.SetActive(false);
            trafficIntensity.gameObject.SetActive(false);
            emissions.gameObject.SetActive(false);
            TrafficInfoTab.SetActive(false);
            _showingRainDistributionGraph = false;
        }
    }

    public void RainButtonPressed()
    {
        if (TutorialManager.Instance.TutorialDialogue <= 3)
        {
            TutorialManager.Instance.HanldePlayer();
            return;
        }
        else if (TutorialManager.Instance.TutorialDialogue == 6)
        {
            TutorialManager.Instance.DeactivateArrow();
            TutorialManager.Instance.NextTutorialDialogue();
        }
        else if (TutorialManager.Instance.TutorialDialogue == 5)
        {
            TutorialManager.Instance.HanldePlayer();
        }

        if (!uiState.Equals(UIState.Rain))
        {
            if (uiState.Equals(UIState.Social))
            {
                ChangeButtonColorToPressed(SocialButton.GetComponent<Button>());
            }
            else
            {
                ChangeButtonColorToPressed(TrafficButton.GetComponent<Button>());
            }
            ExitBuildMode();
            ShowRunoffReductions();
            uiState = UIState.Rain;
            ChangeButtonColorToPressed(RainButton.GetComponent<Button>());
            trafficIntensity.gameObject.SetActive(false);
            emissions.gameObject.SetActive(false);
            RainDistributionPanel.SetActive(true);
            unemploymentRate.gameObject.SetActive(false);
            populationDensity.gameObject.SetActive(false);
            flashFloodRisk.gameObject.SetActive(true);
            TrafficInfoTab.SetActive(false);
            flashFloodRisk.value = Mathf.Clamp(RainEventsManager.Instance.CalculateTotalRunoff(), 0f, flashFloodRiskSliderCap);
            UpdateFlashFloodRiskTxt(RainEventsManager.Instance.CalculateFlashFloorRisk().ToString("F2"));

            //call next tutorial panel
            if (_tutorialOn)
            {
                Subcatchment subcat = MapManager.Instance.GetSubcatchment(7);
                ShowRainInfoTab(subcat.SubcatchmentNumber, subcat.BuildStatus, subcat.GetBGIsBuiltOnSubcatchment());
            }
        }
    }

    public void TrafficButtonPressed()
    {
        if (TutorialManager.Instance.TutorialDialogue <= 3)
        {
            TutorialManager.Instance.HanldePlayer();
            return;
        }
        else if (TutorialManager.Instance.TutorialDialogue == 4)
        {
            TutorialManager.Instance.NextTutorialDialogue();
            TutorialManager.Instance.DeactivateArrow();
        }

        if (!uiState.Equals(UIState.Traffic))
        {
            if (uiState.Equals(UIState.Social))
            {
                ChangeButtonColorToPressed(SocialButton.GetComponent<Button>());
            }
            else
            {
                ChangeButtonColorToPressed(RainButton.GetComponent<Button>());
            }
            uiState = UIState.Traffic;
            ExitBuildMode();
            HideRunoffReductions();
            ChangeButtonColorToPressed(TrafficButton.GetComponent<Button>());
            RainDistributionPanel.SetActive(false);
            unemploymentRate.gameObject.SetActive(false);
            populationDensity.gameObject.SetActive(false);
            flashFloodRisk.gameObject.SetActive(false);
            trafficIntensity.gameObject.SetActive(true);
            emissions.gameObject.SetActive(true);

            UpdateTrafficSlider();

            UpdateEmissionSlider();

        }
    }

    public void UpdateEmissionSlider()
    {
        float emissionsVal = TrafficManager.Instance.GetTrafficEmissions();
        UpdateEmissionsTxt(emissionsVal.ToString("F2"));
        emissions.value = emissionsVal;
    }

    public void ShowRainInfoTab(int subcatNumber, BuildStatus subcatBuildStatus, Dictionary<InfrastructureType, BuildStatus> bgisBuilt)
    {
        float runoffLv1 = RainEventsManager.Instance.GetRunoffReductionPercentage(1, subcatNumber, subcatBuildStatus);
        float runoffLv2 = RainEventsManager.Instance.GetRunoffReductionPercentage(2, subcatNumber, subcatBuildStatus);
        float runoffLv3 = RainEventsManager.Instance.GetRunoffReductionPercentage(3, subcatNumber, subcatBuildStatus);
        string bgi1 = "No BGI";
        string bgi2 = "No BGI";
        if (bgisBuilt.Count > 0)
        {
            foreach (InfrastructureType key in Enum.GetValues(typeof(InfrastructureType)))
            {
                if (bgisBuilt.ContainsKey(key))
                {
                    if (bgi1.Equals("No BGI"))
                    {
                        bgi1 = ConvertBGIToString(key, bgisBuilt[key]);
                    }
                    else if (bgi2.Equals("No BGI"))
                    {
                        bgi2 = ConvertBGIToString(key, bgisBuilt[key]);
                    }
                }
            }
        }
        RainInfoTab.SetActive(true);
        RainInfoTab.GetComponent<RainInfoTab>().UpdateRainTextFields(runoffLv1, runoffLv2, runoffLv3, subcatNumber, bgi1, bgi2);

        //deactivate other stuff if in use
        if (_showingRainDistributionGraph)
        {
            //hide it [name of the function is counterintuitive]
            ShowRainDistributionGraph();
        }
    }

    public void ShowRainDistributionGraph()
    {
        _showingRainDistributionGraph = !_showingRainDistributionGraph;

        if (_showingRainDistributionGraph)
        {
            RainDistributionGraph.SetActive(true);
            RainDistributionPanel.SetActive(false);
            //deactivate other stuff
            if (RainInfoTab.activeInHierarchy)
            {
                RainInfoTab.SetActive(false);
            }
        }
        else
        {
            RainDistributionGraph.SetActive(false);
            RainDistributionPanel.SetActive(true);
        }

    }

    public IEnumerator FillSlidersAnimation(float _actualRunoff, float _actualPrecipitation, float _predictedRunoff, float _predictedPrecipitation)
    {
        float timeToWait = sliderFillingTime / _actualRunoff;
        float biggerValToConsider = _actualRunoff;
        float fillFraction = _actualRunoff / (sliderFillingTime * 60);
        if (_actualRunoff < _actualPrecipitation)
        {
            fillFraction = _actualPrecipitation / (sliderFillingTime * 60);
            timeToWait = sliderFillingTime / _actualPrecipitation;
            biggerValToConsider = _actualPrecipitation;
        }

        if (biggerValToConsider < _predictedRunoff)
        {
            fillFraction = _predictedRunoff / (sliderFillingTime * 60);
            timeToWait = sliderFillingTime / _predictedRunoff;
            biggerValToConsider = _predictedRunoff;
        }
        else if (biggerValToConsider < _predictedPrecipitation)
        {
            fillFraction = _predictedPrecipitation / (sliderFillingTime * 60);
            timeToWait = sliderFillingTime / _predictedPrecipitation;
            biggerValToConsider = _predictedPrecipitation;
        }

        float fillValue = fillFraction;
        while (fillValue < biggerValToConsider)
        {
            fillValue += fillFraction;
            if (fillValue < _actualRunoff)
            {
                actualRunoff.value = fillValue;
            }

            if (fillValue < _actualPrecipitation)
            {
                actualPrecipitation.text = fillValue.ToString("F2");
            }

            if (fillValue < _predictedRunoff)
            {
                predictedRunoff.value = fillValue;
            }

            if (fillValue < _predictedPrecipitation)
            {
                predictedPrecipitation.text = fillValue.ToString("F2");
            }

            yield return new WaitForSeconds(timeToWait);
        }
        _canPlayNextAnimation = true;
    }

    public void ShowEndRoundInfos()
    {
        if (TutorialManager.Instance.TutorialDialogue <= 2 && MapManager.Instance.GetNumberOfBusinessSubcatchmentsBuilt() == 0)
        {
            TutorialManager.Instance.HanldePlayer();
            return;
        }
        _showingRainEventInfos = true;
        DeactivateButtons();
        StartCoroutine(RainEventInfosVisualization());
        StartCoroutine(ShowScoreGraph());
    }

    public IEnumerator RainEventInfosVisualization()
    {
        float _actualRunoff = RainEventsManager.Instance.CalculateTotalRunoff(true);
        float _actualPrecipitation = RainEventsManager.Instance.RainPerRound[RoundManager.Instance.CurrentRound] + RainEventsManager.Instance.RainPredictionDeviationValue;
        float _predictedRunoff = RainEventsManager.Instance.CalculateTotalRunoff(false);
        float _predictedPrecipitation = RainEventsManager.Instance.RainPerRound[RoundManager.Instance.CurrentRound];
        //recover lenght of animations
        Dictionary<string, AnimationClip> RainEventInfoPanelAnimationsLenghts = GetAnimatorClips(RainEventInfoPanelAnimator);
        //deactivate final flash flood result panels
        foreach (Transform child in RainEventInfoPanelAnimator.transform)
        {
            if (child.name.Equals("FloodResult"))
            {
                foreach (Transform result in child)
                {
                    result.gameObject.SetActive(false);

                }
            }
        }
        if (RainEventInfoPanelAnimator.enabled)
        {
            RainEventInfoPanelAnimator.Play("Appear", 0, 0f);
        }
        else
        {
            RainEventInfoPanelAnimator.enabled = true;
        }
        yield return new WaitForSeconds(RainEventInfoPanelAnimationsLenghts["Appear"].length);


        //Setup trigger to activate next animation after sliders fill
        _canPlayNextAnimation = false;
        StartCoroutine(FillSlidersAnimation(_actualRunoff, _actualPrecipitation, _predictedRunoff, _predictedPrecipitation));

        //Wait untill sliders are filled before playing next animation
        yield return new WaitUntil(() => _canPlayNextAnimation);
        yield return new WaitForSeconds(timeToWaitBeforeNextAnimation);


        //activate correspondent feedback if there is the flash flood or not
        bool flahsFlood = _actualRunoff > RainEventsManager.Instance.FlashFloodThreshold;
        foreach (Transform child in RainEventInfoPanelAnimator.transform)
        {
            if (child.name.Equals("FloodResult"))
            {
                foreach (Transform result in child)
                {
                    if (flahsFlood)
                    {
                        if (result.name.Equals("Flood"))
                        {
                            result.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (result.name.Equals("NoFlood"))
                        {
                            result.gameObject.SetActive(true);
                        }
                    }

                }
            }
        }

        RainEventInfoPanelAnimator.Play("FloodResult", 0, 0f);
        yield return new WaitForSeconds(RainEventInfoPanelAnimationsLenghts["FloodResult"].length + timeToWaitBeforeNextAnimation / 2);
        RainEventInfoPanelAnimator.Play("Disappear", 0, 0f); //THIS IS JUST A PLACEHOLDER IT SHOULD BE FLASH FLOOD TRUE OR FALSE ANIMATION

        //wait for rain event info panel to disappear and then visualize flash flood eventually
        yield return new WaitForSeconds(RainEventInfoPanelAnimationsLenghts["Disappear"].length);
        if (flahsFlood)
        {
            TrafficManager.Instance.UpdateTrafficData(true);
            foreach (Transform particleSys in flashFloodVfx)
            {
                particleSys.GetComponent<ParticleSystem>().Play();
            }
            yield return new WaitForSeconds(4/*timeToWaitBeforeNextAnimation / 2*/);
            foreach (Transform particleSys in flashFloodVfx)
            {
                particleSys.GetComponent<ParticleSystem>().Stop();
            }

            TrafficManager.Instance.UpdateStreetsColor(true);

        }
        _showingRainEventInfos = false;
    }

    public Dictionary<string, AnimationClip> GetAnimatorClips(Animator anim)
    {
        Dictionary<string, AnimationClip> animationsLenghts = new Dictionary<string, AnimationClip>();
        foreach (AnimationClip animation in anim.runtimeAnimatorController.animationClips)
        {
            animationsLenghts.Add(animation.name, animation);
        }
        return animationsLenghts;
    }

    public IEnumerator ShowScoreGraph()
    {
        //Update score
        if (RoundManager.Instance.CurrentRound == 11)
        {
            ResourceManager.Instance.UpdateBudgetsAtEndRound();
        }
        ScoreManager.Instance.UpdateScore();
        Dictionary<int, float> graphData = ScoreManager.Instance.TotalScores();

        //setup the graph
        scoreGraph.GetComponentInChildren<ScoreGraph>().UpdateGraph(graphData, ScoreManager.Instance.MaxTotalPoints);
        yield return new WaitUntil(() => !_showingRainEventInfos);

        //show score graph
        scoreGraph.GetComponent<Animator>().enabled = true;
        scoreGraph.GetComponent<Animator>().Play("Appear", 0, 0f);
        //control goes to score graph button
    }

    public void HideScoreGraph()
    {
        scoreGraph.GetComponent<Animator>().Play("Disappear", 0, 0f);
    }

    public void ShowScoreBoard()
    {
        //SCORE SUMMARY UPDATE IS HERE ONLY FOR TIMING PURPOSES, SHOULD BE CODED BETTER :/
        RoundSnapshot snap = ScoreManager.Instance.GetRoundSnapshot(RoundManager.Instance.CurrentRound);
        scoreSummary.UpdateSummaryScore(ScoreManager.Instance.SocialScore.ToString(), ScoreManager.Instance.EnvironmentalScore.ToString(), ScoreManager.Instance.EconomicScore.ToString());

        scoreBoard.GetComponent<DetaildScoreBoard>().SetBoardData(ScoreManager.Instance.GetRoundSnapshot(RoundManager.Instance.CurrentRound));
        scoreBoard.GetComponent<DetaildScoreBoard>().Appear();
    }

    public void DeactivateButtons()
    {
        InfrasructureButton.GetComponent<Button>().enabled = false;
        RoundButton.GetComponent<Button>().enabled = false;
        SocialButton.GetComponent<Button>().enabled = false;
        RainButton.GetComponent<Button>().enabled = false;
        GRButtonDefault.GetComponent<Button>().enabled = false;
        RBButtonDefault.GetComponent<Button>().enabled = false;
        PPButtonDefault.GetComponent<Button>().enabled = false;
    }

    public void ActivateButtons()
    {
        InfrasructureButton.GetComponent<Button>().enabled = true;
        RoundButton.GetComponent<Button>().enabled = true;
        SocialButton.GetComponent<Button>().enabled = true;
        RainButton.GetComponent<Button>().enabled = true;
        GRButtonDefault.GetComponent<Button>().enabled = true;
        RBButtonDefault.GetComponent<Button>().enabled = true;
        PPButtonDefault.GetComponent<Button>().enabled = true;
    }

    public void ActivateTutorialInfrastructureButton()
    {
        _tutorialOn = true;
        InfrasructureButton.GetComponent<Button>().enabled = true;
    }
    public void ActivateRainButton()
    {
        //_tutorialOn = true;
        RainButton.GetComponent<Button>().enabled = true;
    }

    public void ActivateTrafficButton()
    {
        TrafficButton.GetComponent<Button>().enabled = true;
    }

    public void DeactivateTutorialMode()
    {
        _tutorialOn = false;
    }

    public void ActivateTutorialMode()
    {
        _tutorialOn = true;
    }
}

public enum UIState
{
    Social,
    Rain,
    Traffic
}
