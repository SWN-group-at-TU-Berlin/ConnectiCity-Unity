using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subcatchment : MonoBehaviour
{
    [Header("Subcatchment settings")]
    [SerializeField] int _subcatchmentNumber;
    #region getter
    public int SubcatchmentNumber { get { return _subcatchmentNumber; } }
    #endregion

    [SerializeField] int _subcatchmentBenefit;
    #region getter-setter
    public int SubcatchmentBenefit { get { return _subcatchmentBenefit; } set { _subcatchmentBenefit = value; } }
    #endregion

    [SerializeField] AreaUsage _usage;
    #region getter
    public AreaUsage Usage { get { return _usage; } }
    #endregion

    [SerializeField] AreaSize _size;
    #region getter
    public AreaSize Size { get { return _size; } }
    #endregion

    [SerializeField] BuildStatus _buildStatus;
    #region getter
    public BuildStatus BuildStatus { get { return _buildStatus; } set { _buildStatus = value; } }
    #endregion

    [SerializeField] BuildStatus _grPercentage;
    #region getter
    public BuildStatus GRPercentage { get { return _grPercentage; } set { _grPercentage = value; } }
    #endregion

    [SerializeField] BuildStatus _rbType;
    #region getter
    public BuildStatus RBType { get { return _rbType; } set { _rbType = value; } }
    #endregion

    [Header("Highlighted Materials")]
    [SerializeField] Material _highlightedMaterial;
    [SerializeField] Material _deactivatedMaterial;

    Material defaultMaterial;
    Color highlightSelectionColor;


    bool _isBuilt;
    #region getter
    public bool IsBuilt { get { return _isBuilt; } }
    #endregion
    bool _isHostingBGI;
    #region getter
    public bool IsHostingBGI { get { return _isHostingBGI; } }
    #endregion
    bool _isHighlighted;
    #region getter
    public bool IsHighlighted { get { return _isHighlighted; } }
    #endregion

    Outline outline;

    List<InfrastructureType> _BGIHosted;
    #region getter
    public List<InfrastructureType> BGIHosted { get { return _BGIHosted; } }
    #endregion
    bool IsSelected { get; set; } // still don't know if this will be useful
    bool IsHovered { get; set; }
    bool _active;//getting deactivated after rainfall event
    #region getter
    public bool Active { get { return _active; } }
    #endregion

    float defaultHighlightedMatIntesity;

    InputProvider input;

    private void Awake()
    {
        _buildStatus = BuildStatus.Unbuild;
        defaultMaterial = GetComponent<MeshRenderer>().material;
        highlightSelectionColor = GetComponent<Outline>().OutlineColor;
        _BGIHosted = new List<InfrastructureType>();
        _isHostingBGI = false;
        _active = true;
    }

    private void Start()
    {
        _subcatchmentBenefit = (int)DataReader.Instance.SubcatchmentsBenefits[_subcatchmentNumber];
        if (SubcatchmentNumber == 7)
        {
            InfrastructureBuilder.Instance.SelectedInfrastructure = InfrastructureType.Building;
            InfrastructureBuilder.Instance.SelectedSubcatchment = this;
            InfrastructureBuilder.Instance.BuildInfrastructure();
            _buildStatus = BuildStatus.Built;
        }
    }

    private void OnEnable()
    {
        outline = GetComponent<Outline>();
        input = new InputProvider();
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(input.MousePosition());
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag.Equals("Subcatchment") && hit.transform.gameObject.name.Equals("SC" + SubcatchmentNumber) && (IsHighlighted || UIManager.Instance.ShowingRunoffReduction))
            {
                if (!IsHovered)
                {
                    MouseHoveringOnSubcatchment();
                }

                if (input.MouseLeftButton())
                {
                    if (IsHighlighted)
                    {
                        CallUIInfoTab();
                    }
                    else
                    {
                        UIManager.Instance.ShowRainInfoTab(_subcatchmentNumber, _buildStatus, GetBGIsBuiltOnSubcatchment());
                    }
                }
            }
            else
            {
                if (IsHovered)
                {
                    IsHovered = false;
                    outline.OutlineColor = Color.white;
                    outline.enabled = IsHovered;
                }
            }

        }
        else
        {
            if (IsHovered)
            {
                IsHovered = false;
                outline.OutlineColor = Color.white;
                outline.enabled = IsHovered;
            }
        }
    }

    public void UpdateInfoTabInfrastructure()
    {
        if (_isHighlighted)
        {
            CallUIInfoTab();
        }
    }

    private void CallUIInfoTab()
    {
        InfrastructureType infrastructureTypeToBuild = InfrastructureType.Null;
        bool buildingInfrastructure = false;
        if (UIManager.Instance.InfrastructureTypeButtonPressed.Equals(InfrastructureType.Building))
        {
            buildingInfrastructure = true;
            if (Usage.Equals(AreaUsage.Commercial))
            {
                infrastructureTypeToBuild = InfrastructureType.Business;
            }
            else
            {
                infrastructureTypeToBuild = InfrastructureType.House;
            }
        }
        else
        {
            infrastructureTypeToBuild = UIManager.Instance.InfrastructureTypeButtonPressed;
        }
        BuildStatus infrastructureToBuild = ConvertInfrastructureTypeToBuildStatus(UIManager.Instance.InfrastructureTypeButtonPressed);
        float buildingCost = CostsManager.Instance.GetSubcatchmentBuildCosts(SubcatchmentNumber, infrastructureToBuild);
        if (buildingInfrastructure)
        {
            UIManager.Instance.ShowInfoTabInfrastructure(_subcatchmentNumber, infrastructureTypeToBuild, infrastructureToBuild, buildingCost, SubcatchmentBenefit);
        }
        else if (!_buildStatus.Equals(BuildStatus.Unbuild))
        {
            //take into account the bgi combinations
            infrastructureToBuild = NewBuildStatus(infrastructureToBuild);

            //recover runoff reductions change
            float BGIRainReductionLv1_n = DataReader.Instance.GetRunoffReductionPercentage(1, new SubcatchmentKey(SubcatchmentNumber, infrastructureToBuild));
            float BGIRainReductionLv2_n = DataReader.Instance.GetRunoffReductionPercentage(2, new SubcatchmentKey(SubcatchmentNumber, infrastructureToBuild));
            float BGIRainReductionLv3_n = DataReader.Instance.GetRunoffReductionPercentage(3, new SubcatchmentKey(SubcatchmentNumber, infrastructureToBuild));
            //store them in dictionary
            Dictionary<int, float> newRunoffReductions = new Dictionary<int, float>();
            newRunoffReductions.Add(1, BGIRainReductionLv1_n);
            newRunoffReductions.Add(2, BGIRainReductionLv2_n);
            newRunoffReductions.Add(3, BGIRainReductionLv3_n);

            //recover current runoff reductions
            float BGIRuonffReductionLv1_c = DataReader.Instance.GetRunoffReductionPercentage(1, new SubcatchmentKey(SubcatchmentNumber, _buildStatus));
            float BGIRuonffReductionLv2_c = DataReader.Instance.GetRunoffReductionPercentage(2, new SubcatchmentKey(SubcatchmentNumber, _buildStatus));
            float BGIRuonffReductionLv3_c = DataReader.Instance.GetRunoffReductionPercentage(3, new SubcatchmentKey(SubcatchmentNumber, _buildStatus));
            //store them in dictionary
            Dictionary<int, float> currentRunoffReductions = new Dictionary<int, float>();
            currentRunoffReductions.Add(1, BGIRuonffReductionLv1_c);
            currentRunoffReductions.Add(2, BGIRuonffReductionLv2_c);
            currentRunoffReductions.Add(3, BGIRuonffReductionLv3_c);
            UIManager.Instance.ShowInfoTabBGI(_subcatchmentNumber, infrastructureTypeToBuild, infrastructureToBuild, buildingCost, newRunoffReductions, currentRunoffReductions);
        }
        InfrastructureBuilder.Instance.SelectedSubcatchment = this;
    }

    public BuildStatus ConvertInfrastructureTypeToBuildStatus(InfrastructureType toConvert)
    {
        BuildStatus converted;
        if (toConvert.Equals(InfrastructureType.Building) || toConvert.Equals(InfrastructureType.House) || toConvert.Equals(InfrastructureType.Business))
        {
            converted = BuildStatus.Built;
        }
        else if (toConvert.Equals(InfrastructureType.GR))
        {
            converted = GRPercentage;
        }
        else if (toConvert.Equals(InfrastructureType.RB))
        {
            converted = RBType;
        }
        else
        {
            converted = BuildStatus.PP;
        }
        return converted;
    }

    /*this function returns the build status to update in case of construction
     of the infrastructure given as parameter "infrastructureToBuild" */
    public BuildStatus NewBuildStatus(BuildStatus infrastructureToBuild)
    {
        BuildStatus newBuildStatus = _buildStatus;
        if (infrastructureToBuild.Equals(BuildStatus.Built))
        {
            if (_buildStatus.Equals(BuildStatus.Unbuild))
            {
                newBuildStatus = infrastructureToBuild;
            }
        }
        else if (!infrastructureToBuild.Equals(BuildStatus.Unbuild))
        {
            //only if buid status is not "Built", since it means that there is alreadz one BGI
            if (!_buildStatus.Equals(BuildStatus.Built))
            {
                /*to understand which combination of BGI would be the next build status
                 *we are going to compare the combination of the current _buildStatus 
                 *concatenated with the infrastructureToBuild.
                 *There are only two possible combinations that these variables can form:*/

                //first combination
                string combo1 = _buildStatus + "_" + infrastructureToBuild;

                //second combo
                string combo2 = infrastructureToBuild + "_" + _buildStatus;

                //now we compare them with every element.ToString() of the BuildStatus array
                foreach (BuildStatus status in Enum.GetValues(typeof(BuildStatus)))
                {
                    if (status.ToString().Equals(combo1) || status.ToString().Equals(combo2))
                    {
                        newBuildStatus = status;
                    }
                }
            }
            else //if it's the first BGI on the subcat
            {
                newBuildStatus = infrastructureToBuild;
            }
        }
        return newBuildStatus;
    }

    private void ShowInfrastructure(string infrastructure)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals(infrastructure))
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public bool SubcatchmentHostsBGI(InfrastructureType bgi)
    {
        bool hostingBGI = false;
        foreach (InfrastructureType infra in BGIHosted)
        {
            if (infra.Equals(bgi))
            {
                hostingBGI = true;
            }
        }
        return hostingBGI;
    }

    public void BuildInfrastructureOnSubcatchment(InfrastructureType infrastructure)
    {
        string infrastructureStr = infrastructure.ToString();
        if (infrastructure.Equals(InfrastructureType.Building))
        {
            _isBuilt = true;
            if (_usage.Equals(AreaUsage.Commercial))
            {
                infrastructureStr = InfrastructureType.Business.ToString();
            }
            else
            {
                infrastructureStr = InfrastructureType.House.ToString();
            }
        }
        else
        {
            BGIHosted.Add(infrastructure);
            _isHostingBGI = true;
        }
        ShowInfrastructure(infrastructureStr);
    }

    public bool CanHostBGI(InfrastructureType BGI)
    {
        bool canHostBGI = false;
        if (BGI.Equals(InfrastructureType.House) || BGI.Equals(InfrastructureType.Business))
        {
            canHostBGI = false;
        }
        else if (BGIHosted.Count >= 2)
        {
            canHostBGI = false;
        }
        else if (BGIHosted.Count == 0)
        {
            canHostBGI = true;
        }
        else if (!BGIHosted[0].Equals(BGI))
        {
            canHostBGI = true;
        }
        return canHostBGI;
    }

    public void ShowBuildInfoPanels(InfrastructureType infrastructureType)
    {
        //convert infratructure type to build status
        BuildStatus buildStatus = BuildStatus.Unbuild;
        switch (infrastructureType)
        {
            case InfrastructureType.Building:
                {
                    buildStatus = BuildStatus.Built;
                    break;
                }
            case InfrastructureType.GR:
                {
                    buildStatus = _grPercentage;
                    break;
                }
            case InfrastructureType.RB:
                {
                    buildStatus = _rbType;
                    break;
                }
            case InfrastructureType.PP:
                {
                    buildStatus = BuildStatus.PP;
                    break;
                }
            default:
                {
                    Debug.LogWarning("The infrastructure type: " + infrastructureType + " is not considered buildable");
                    return;
                    break;
                }
        }

        //get build cost
        float buildCost = CostsManager.Instance.GetSubcatchmentBuildCosts(_subcatchmentNumber, buildStatus);

        //get ap cost [DEPRECATED]
        float apCost = CostsManager.Instance.GetActionPointCosts(_subcatchmentNumber, buildStatus);

        //get income benefit
        float benefit = SubcatchmentBenefit;

        //if trying to build BGI
        if (!buildStatus.Equals(BuildStatus.Built))
        {
            //benefit is the difference between the current runoff reduction of the subcat vs the new one after building the bgi
            float currentBGIReduction = RainEventsManager.Instance.GetRunoffReductionPercentage(RainEventsManager.Instance.CurrentRainIntensity, _subcatchmentNumber, _buildStatus);
            //retrieve eventual new biuild status
            BuildStatus newBuildStatus = NewBuildStatus(buildStatus);
            float newBGIReduction = RainEventsManager.Instance.GetRunoffReductionPercentage(RainEventsManager.Instance.CurrentRainIntensity, _subcatchmentNumber, buildStatus);
            if (!newBuildStatus.Equals(_buildStatus))
            {
                newBGIReduction = RainEventsManager.Instance.GetRunoffReductionPercentage(RainEventsManager.Instance.CurrentRainIntensity, _subcatchmentNumber, NewBuildStatus(buildStatus));
                newBGIReduction = newBGIReduction -currentBGIReduction;
            }
            benefit = newBGIReduction;
        }

        //get cn benefit [DEPRECATED]
        float citizenNumberBenefit = CostsManager.Instance.GetBuildBenefit(Benefit.citizenNumber, _subcatchmentNumber, buildStatus);

        //get cs benefit [DEPRECATED]
        float citizenSatisfactionBenefit = CostsManager.Instance.GetBuildBenefit(Benefit.citizenSatisfaction, _subcatchmentNumber, buildStatus);

        //call UIManager to show info panel with previous info
        Vector3 position = GetInfoPanelPosition();
        UIManager.Instance.ShowSocialInfoPanel(Usage, infrastructureType, position, ((int)apCost), ((int)buildCost), ((int)benefit), ((int)citizenSatisfactionBenefit), ((int)citizenNumberBenefit));
    }

    //DEPRECATED
    public void ShowInfos(InfrastructureType infrastructure)
    {
        int budgetCost = 0;
        int income = 0;
        int apCost = 0;
        int citizenSatisfactionIncrease = 0;
        int citizenNumberIncrease = 0;
        if (infrastructure.Equals(InfrastructureType.House) || infrastructure.Equals(InfrastructureType.Business))
        {
            BasicInfrastructureStats stats = CostsManager.Instance.GetInfrastructureStats(infrastructure);
            citizenSatisfactionIncrease = stats.BCitizenSatisfaction;
            if (_size.Equals(AreaSize.Small))
            {
                budgetCost = stats.CBudgetSmall;
                income = stats.BIncomeSmall;
                citizenNumberIncrease = stats.BCitizenNumberSmall;
            }
            else if (_size.Equals(AreaSize.Medium))
            {

                budgetCost = stats.CBudgetMedium;
                income = stats.BIncomeMedium;
                citizenNumberIncrease = stats.BCitizenNumberMedium;
            }
            else if (_size.Equals(AreaSize.Large))
            {

                budgetCost = stats.CBudgetLarge;
                income = stats.BIncomeLarge;
                citizenNumberIncrease = stats.BCitizenNumberLarge;
            }
            apCost = stats.CActionPoints;
        }
        else
        {
            BasicBGIStats stats = CostsManager.Instance.GetBGIStats(infrastructure);
            citizenSatisfactionIncrease = stats.BCitizenSatisfaction;
            if (Usage.Equals(AreaUsage.Residential))
            {
                if (_size.Equals(AreaSize.Small))
                {
                    budgetCost = stats.CBudgetResidentialSmall;
                }
                else if (_size.Equals(AreaSize.Medium))
                {

                    budgetCost = stats.CBudgetResidentialMedium;
                }
                else if (_size.Equals(AreaSize.Large))
                {

                    budgetCost = stats.CBudgetResidentialLarge;
                }
                apCost = stats.CActionPoints;
            }
            else
            {
                if (_size.Equals(AreaSize.Small))
                {
                    budgetCost = stats.CBudgetCommercialSmall;
                }
                else if (_size.Equals(AreaSize.Medium))
                {

                    budgetCost = stats.CBudgetCommercialMedium;
                }
                else if (_size.Equals(AreaSize.Large))
                {

                    budgetCost = stats.CBudgetCommercialLarge;
                }
                apCost = stats.CActionPoints + 1;
            }
        }
        Vector3 position = GetInfoPanelPosition();

        //UIManager.Instance.ShowInfoPanel(Usage, infrastructureType, position, apCost, budgetCost, SubcatchmentBenefit, citizenSatisfactionIncrease, citizenNumberIncrease);
    }

    public Vector3 GetInfoPanelPosition()
    {
        Vector3 position = Vector3.zero;
        foreach (Transform child in transform)
        {
            if (child.name.Equals("InfoPanelPoint"))
            {
                position = child.position;
            }
        }
        return position;
    }

    void MouseHoveringOnSubcatchment()
    {
        IsHovered = true;
        outline.enabled = IsHovered;
        outline.OutlineColor = highlightSelectionColor;
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse on subcat: " + this.SubcatchmentNumber);
    }

    private void OnMouseOver()
    {
        Debug.Log("Mouse on subcat: " + this.SubcatchmentNumber);
    }

    public void HighlightSubcatchment()
    {
        _isHighlighted = true;
        GetComponent<MeshRenderer>().material = _highlightedMaterial;
    }

    public void DehighlightSubcatchment()
    {
        _isHighlighted = false;
        GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    public void ShowRunoffReductionReductionInfo()
    {
        Vector3 position = GetInfoPanelPosition();
        float runoffReduction = RainEventsManager.Instance.GetRunoffReductionPercentage(RainEventsManager.Instance.CurrentRainIntensity, _subcatchmentNumber, _buildStatus);
        UIManager.Instance.ShowRainInfoPanel(runoffReduction, position);
        //make subcat selectable
    }

    public void SetSubcatchmentActive(bool activeState)
    {
        /*if (activeState)
        {
            //update income on activation
            if (_usage.Equals(AreaUsage.Commercial) && !_active && _isBuilt)
            {
                GetComponent<MeshRenderer>().material = defaultMaterial;
                _active = true;
                if (Size.Equals(AreaSize.Large))
                {
                    int incomeUpdate = CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeLarge;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
                if (Size.Equals(AreaSize.Medium))
                {
                    int incomeUpdate = CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeMedium;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
                if (Size.Equals(AreaSize.Small))
                {
                    int incomeUpdate = CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeSmall;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
            }
        }
        else
        {
            //update income on deactivation
            if (_usage.Equals(AreaUsage.Commercial) && _active && _isBuilt)
            {
                GetComponent<MeshRenderer>().material = _deactivatedMaterial;
                _active = false;
                if (Size.Equals(AreaSize.Large))
                {
                    int incomeUpdate = -CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeLarge;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
                if (Size.Equals(AreaSize.Medium))
                {
                    int incomeUpdate = -CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeMedium;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
                if (Size.Equals(AreaSize.Small))
                {
                    int incomeUpdate = -CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).BIncomeSmall;
                    ResourceManager.Instance.UpdateIncome(incomeUpdate);
                    UIManager.Instance.ShowFloatingTxt(incomeUpdate, "i", this);
                }
            }
        }*/
    }

    public Dictionary<InfrastructureType, BuildStatus> GetBGIsBuiltOnSubcatchment()
    {
        Dictionary<InfrastructureType, BuildStatus> hostedBGIs = new Dictionary<InfrastructureType, BuildStatus>();
        foreach (InfrastructureType bgi in BGIHosted)
        {
            if (bgi.ToString().Equals("GR"))
            {
                hostedBGIs.Add(bgi, _grPercentage);
            }
            if (bgi.ToString().Equals("RB"))
            {
                hostedBGIs.Add(bgi, _rbType);
            }
            if (bgi.ToString().Equals("PP"))
            {
                hostedBGIs.Add(bgi, BuildStatus.PP);
            }
        }
        return hostedBGIs;
    }
}
