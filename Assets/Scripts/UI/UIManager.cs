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
    [SerializeField] TextMeshProUGUI budget;
    [SerializeField] TextMeshProUGUI income;
    [SerializeField] TextMeshProUGUI citizenNumber;
    [SerializeField] TextMeshProUGUI citizenSatisfaction;
    [SerializeField] TextMeshProUGUI ActionPoints;
    [SerializeField] TextMeshProUGUI CurrentRound;

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

    [Header("Info boards references and properties")]
    [SerializeField] GameObject MessageBoard;
    [SerializeField] GameObject InfoTab;
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


    [Header("Floating text references")]
    [SerializeField] GameObject floatingTextParent;
    [SerializeField] GameObject floatingTextPrefab;
    [SerializeField] float floatingSpeed;
    [SerializeField] float txtFadeOutTime;

    //Button variables
    List<GameObject> DefaultButtons;
    InfrastructureType infrastructureTypeButtonPressed;
    #region getter
    public InfrastructureType InfrastructureTypeButtonPressed { get { return infrastructureTypeButtonPressed; } }
    #endregion
    bool _houseButtonPressed = false;
    bool _businessButtonPressed = false;
    bool _GRButtonPressed = false;
    bool _PPButtonPressed = false;
    bool _RBButtonPressed = false;
    bool _buildMode = false;

    //Info boards variables
    Color fullAlphaBackground;
    Color fullAlphaTitle;
    Color fullAlphaMessage;
    Queue<Transform> infoPanelsNotInUse; // deactivated
    Queue<Transform> infoPanelsInUse; // active
    float tooltipCooldownTimer;

    //InputProvider reference
    InputProvider input;

    private void Update()
    {
        //TooltipShow();
        if (input.PauseButton())
        {
            TogglePauseMenu();
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(input.MousePosition());
        if (!Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
        {
            /** Debug.Log("ray hitting");
             if (!hit.transform.gameObject.tag.Equals("Subcatchment") && !hit.transform.gameObject.layer.Equals(UILayer) && _buildMode)
             {*/
            if (input.MouseLeftButton())
            {
                ExitBuildMode();
            }
            //}
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

    public void InfrastructureButtoPressed()
    {
        //infrastructure type button pressed is null if you exited build mode without switching between infrastructure an bgis
        if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Null) || infrastructureTypeButtonPressed.Equals(InfrastructureType.Building))
        {
            _buildMode = !_buildMode;
        }


        if (_buildMode)
        {
            if (infoPanelsNotInUse.Count == 0)
            {
                foreach (Transform infoPanel in InfoPanels)
                {
                    infoPanelsNotInUse.Enqueue(infoPanel);
                }
            }
            infrastructureTypeButtonPressed = InfrastructureType.Building;
            InfrastructureBuilder.Instance.EnterInfrastructureBuildStatus();
        }
        else
        {
            ExitBuildMode();
        }
    }

    private void ExitBuildMode()
    {
        _buildMode = false;
        MapManager.Instance.DehighlightBuildableSubcatchments();
        HideInfoPanels();
        HideInfoTab();
        infrastructureTypeButtonPressed = InfrastructureType.Null;
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
        houseButtonDefault.SetActive(!_houseButtonPressed);
        if (houseButtonDefault.activeInHierarchy)
        {
            ActivateDefaultButtons(houseButtonDefault);
            HideInfoPanels();
        }
        else
        {
            DeactivateDefaultButtons(houseButtonDefault);
        }
    }

    /*public void GRButtonPressed()     OLD VER TO DELETE 
    {
        _GRButtonPressed = !_GRButtonPressed;
        GRButtonDown.SetActive(_GRButtonPressed);
        GRButtonDefault.SetActive(!_GRButtonPressed);
        if (GRButtonDefault.activeInHierarchy)
        {
            infrastructureTypeButtonPressed = InfrastructureType.GR;
            ActivateDefaultButtons(GRButtonDefault);
            HideInfoPanels();
        }
        else
        {
            infrastructureTypeButtonPressed = InfrastructureType.Null;
            DeactivateDefaultButtons(GRButtonDefault);
        }
    }*/

    public void GRButtonPressed()
    {
        //infrastructure type button pressed is null if you exited build mode without switching between infrastructure an bgis
        if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Null) || infrastructureTypeButtonPressed.Equals(InfrastructureType.GR))
        {
            _buildMode = !_buildMode;
        }


        if (_buildMode)
        {
            if (infoPanelsNotInUse.Count == 0)
            {
                foreach (Transform infoPanel in InfoPanels)
                {
                    infoPanelsNotInUse.Enqueue(infoPanel);
                }
            }
            infrastructureTypeButtonPressed = InfrastructureType.GR;
            InfrastructureBuilder.Instance.EnterBGIBuildStatus(infrastructureTypeButtonPressed);
        }
        else
        {
            ExitBuildMode();
        }
    }

    /* public void PPButtonPressed()
     {
         _PPButtonPressed = !_PPButtonPressed;
         PPButtonDown.SetActive(_PPButtonPressed);
         PPButtonDefault.SetActive(!_PPButtonPressed);
         if (PPButtonDefault.activeInHierarchy)
         {
             infrastructureTypeButtonPressed = InfrastructureType.PP;
             ActivateDefaultButtons(PPButtonDefault);
             HideInfoPanels();
         }
         else
         {
             infrastructureTypeButtonPressed = InfrastructureType.Null;
             DeactivateDefaultButtons(PPButtonDefault);
         }
     }*/

    public void PPButtonPressed()
    {
        //infrastructure type button pressed is null if you exited build mode without switching between infrastructure an bgis
        if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Null) || infrastructureTypeButtonPressed.Equals(InfrastructureType.PP))
        {
            _buildMode = !_buildMode;
        }


        if (_buildMode)
        {
            if (infoPanelsNotInUse.Count == 0)
            {
                foreach (Transform infoPanel in InfoPanels)
                {
                    infoPanelsNotInUse.Enqueue(infoPanel);
                }
            }
            infrastructureTypeButtonPressed = InfrastructureType.PP;
            InfrastructureBuilder.Instance.EnterBGIBuildStatus(infrastructureTypeButtonPressed);
        }
        else
        {
            ExitBuildMode();
        }
    }

    /*public void RBButtonPressed()
    {
        _RBButtonPressed = !_RBButtonPressed;
        RBButtonDown.SetActive(_RBButtonPressed);
        RBButtonDefault.SetActive(!_RBButtonPressed);
        if (RBButtonDefault.activeInHierarchy)
        {
            infrastructureTypeButtonPressed = InfrastructureType.RB;
            ActivateDefaultButtons(RBButtonDefault);
            HideInfoPanels();
        }
        else
        {
            infrastructureTypeButtonPressed = InfrastructureType.Null;
            DeactivateDefaultButtons(RBButtonDefault);
        }
    }*/

    public void RBButtonPressed()
    {
        //infrastructure type button pressed is null if you exited build mode without switching between infrastructure an bgis
        if (infrastructureTypeButtonPressed.Equals(InfrastructureType.Null) || infrastructureTypeButtonPressed.Equals(InfrastructureType.RB))
        {
            _buildMode = !_buildMode;
        }


        if (_buildMode)
        {
            if (infoPanelsNotInUse.Count == 0)
            {
                foreach(Transform infoPanel in InfoPanels)
                {
                    infoPanelsNotInUse.Enqueue(infoPanel);
                }
            }
            infrastructureTypeButtonPressed = InfrastructureType.RB;
            InfrastructureBuilder.Instance.EnterBGIBuildStatus(infrastructureTypeButtonPressed);
        }
        else
        {
            ExitBuildMode();
        }
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

    public void UpdateBudgetTxt(int newBudget) { budget.text = newBudget.ToString(); }
    public void UpdateIncomeTxt(int newIncome) { income.text = newIncome.ToString(); }
    public void UpdateCitizenNumberTxt(int newCN) { citizenNumber.text = newCN.ToString(); }
    public void UpdateCitizenSatisfactionTxt(int newCS) { citizenSatisfaction.text = newCS.ToString(); }
    public void UpdateActionPointsTxt(int newAP) { ActionPoints.text = newAP.ToString(); }
    public void UpdateRoundTxt(int currentRound) { CurrentRound.text = "Round" + currentRound.ToString(); }

    public void ShowInfoPanel(AreaUsage subcatType, Vector3 position, int actionPoints, int budget, int income, int citizenSatisfaction, int citizenNumber)
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
                    textField.GetComponent<TextMeshProUGUI>().text = "-" + budget.ToString();
                    textField.GetComponent<TextMeshProUGUI>().color = CanonRed;
                }
                if (textField.name.Equals("Benefit"))
                {
                    textField.GetComponent<TextMeshProUGUI>().text = "+" + income;
                    textField.GetComponent<TextMeshProUGUI>().color = CanonGreen;
                    if (subcatType.Equals(AreaUsage.Residential))
                    {
                        textField.GetChild(0).GetComponent<Image>().sprite = house;
                    }
                    else
                    {
                        textField.GetChild(0).GetComponent<Image>().sprite = jobs;
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

        /*THIS IS A QUICK N DIRTY FIX TO CONVERT THE ENUM INTO A STRING
         IT SHOULD BE ADDRESSED IN A MORE GENERAL WAY PLZ*/
        string infrastructureTypeStr = "";

        switch (infrastructureType)
        {
            case InfrastructureType.GR:
                {
                    //recover gr percentage of subcat
                    string specs = IdentifySpecsOfBGI(infrastructureToBuild);
                    //add it to the infrastructure type str
                    infrastructureTypeStr = "Gree Roof + " + specs + "% coverage";
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

        float currentBudget = ResourceManager.Instance.Budget;
        float currentap = ResourceManager.Instance.ActionPoints;
        InfoTab.GetComponent<InfoTab>().UpdateTextFieldsBGI(subcatchmentNumber, infrastructureTypeStr, currentBudget, buildCost, currentRunoffReductions, newRunoffReductions);

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
}
