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

    [SerializeField] AreaUsage _usage;
    #region getter
    public AreaUsage Usage { get { return _usage; } }
    #endregion

    [SerializeField] AreaSize _size;
    #region getter
    public AreaSize Size { get { return _size; } }
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
        defaultMaterial = GetComponent<MeshRenderer>().material;
        highlightSelectionColor = GetComponent<Outline>().OutlineColor;
        _BGIHosted = new List<InfrastructureType>();
        _isHostingBGI = false;
        _active = true;
    }

    private void Start()
    {
        if (SubcatchmentNumber == 7)
        {

            InfrastructureBuilder.Instance.BuildInfrastructure(this);
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
            if (hit.transform.gameObject.tag.Equals("Subcatchment") && hit.transform.gameObject.name.Equals("SC" + SubcatchmentNumber) && IsHighlighted)
            {
                if (!IsHovered)
                {
                    MouseHoveringOnSubcatchment();
                }

                if (input.MouseLeftButton())
                {
                    InfrastructureBuilder.Instance.BuildInfrastructure(GetComponent<Subcatchment>());
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

    private void ShowInfrastructure(InfrastructureType infrastructure)
    {
        foreach (Transform child in transform)
        {
            if (child.name.Equals(infrastructure.ToString()))
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void BuildInfrastructureOnSubcatchment(InfrastructureType infrastructure)
    {
        ShowInfrastructure(infrastructure);
        if (infrastructure.Equals(InfrastructureType.House) || infrastructure.Equals(InfrastructureType.Business))
        {
            _isBuilt = true;
        }
        else
        {
            BGIHosted.Add(infrastructure);
            _isHostingBGI = true;
        }
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

    public void ShowInfos(InfrastructureType infrastructure)
    {
        int budgetCost = 0;
        int apCost = 0;
        if (infrastructure.Equals(InfrastructureType.House) || infrastructure.Equals(InfrastructureType.Business))
        {
            BasicInfrastructureStats stats = CostsManager.Instance.GetInfrastructureStats(infrastructure);
            if (_size.Equals(AreaSize.Small))
            {
                budgetCost = stats.CBudgetSmall;
            }
            else if (_size.Equals(AreaSize.Medium))
            {

                budgetCost = stats.CBudgetMedium;
            }
            else if (_size.Equals(AreaSize.Large))
            {

                budgetCost = stats.CBudgetLarge;
            }
            apCost = stats.CActionPoints;
        }
        else
        {
            BasicBGIStats stats = CostsManager.Instance.GetBGIStats(infrastructure);
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

        UIManager.Instance.ShowInfoPanel(position, apCost, budgetCost);
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

    public void SetSubcatchmentActive(bool activeState)
    {
        if (activeState)
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
        }
    }
}
