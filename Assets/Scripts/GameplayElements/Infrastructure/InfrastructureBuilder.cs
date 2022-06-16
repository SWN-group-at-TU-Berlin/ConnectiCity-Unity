using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfrastructureBuilder : MonoBehaviour
{
    private static InfrastructureBuilder _instance;
    #region singleton
    public static InfrastructureBuilder Instance { get { return _instance; } }
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
    }
    #endregion

    public InfrastructureType SelectedInfrastructure { get; set; }
    public GameObject SubcatchmentSelected { get; set; }
    public bool isBuilding { get; set; }

    public void CheckResidentialSubcatchmentAvailabilities()
    {
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.House;
        Subcatchment[] houseSubcatchments = MapManager.Instance.GetHousesSubcatchments();
        int budget = ResourceManager.Instance.Budget;

        //Determine max area size to highlight in order to chose the subcats to highlight
        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetInfrastructureStats(InfrastructureType.House).CActionPoints)
        {
            AreaSize maxSizeToHighlight = AreaSize.Null;
            if (budget < CostsManager.Instance.GetInfrastructureStats(InfrastructureType.House).CBudgetLarge)
            {
                if (budget < CostsManager.Instance.GetInfrastructureStats(InfrastructureType.House).CBudgetMedium)
                {
                    if (budget > CostsManager.Instance.GetInfrastructureStats(InfrastructureType.House).CBudgetSmall)
                    {
                        maxSizeToHighlight = AreaSize.Small;
                    }
                }
                else
                {
                    maxSizeToHighlight = AreaSize.Medium;
                }
            }
            else
            {
                maxSizeToHighlight = AreaSize.Large;
            }

            //Calculate which subcats to highlight
            List<Subcatchment> subcatchmentToHighlight = new List<Subcatchment>();
            foreach (Subcatchment house in houseSubcatchments)
            {
                if (house.Size <= maxSizeToHighlight && !house.IsBuilt)
                {
                    subcatchmentToHighlight.Add(house);
                }
            }
            MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
        }
        else
        {
            UIManager.Instance.MissingActionPointMessage();
            UIManager.Instance.HouseButtonPressed();
        }
    }
    public void CheckCommercialSubcatchmentAvailabilities()
    {
        //Change InfrastructureBuilder status variables
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.Business;
        //Get subcatchment needed
        Subcatchment[] businessSubcatchments = MapManager.Instance.GetBusinessSubcatchments();
        int budget = ResourceManager.Instance.Budget;

        //Check if enough action points are available
        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).CActionPoints)
        {

            //Determine max area size to highlight in order to chose the subcats to highlight based on budget
            AreaSize maxSizeToHighlight = AreaSize.Null;
            if (budget < CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).CBudgetLarge)
            {
                if (budget < CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).CBudgetMedium)
                {
                    if (budget > CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business).CBudgetSmall)
                    {
                        maxSizeToHighlight = AreaSize.Small;
                    }
                }
                else
                {
                    maxSizeToHighlight = AreaSize.Medium;
                }
            }
            else
            {
                maxSizeToHighlight = AreaSize.Large;
            }

            //Calculate which subcats to highlight
            List<Subcatchment> subcatchmentToHighlight = new List<Subcatchment>();
            foreach (Subcatchment business in businessSubcatchments)
            {
                if (business.Size <= maxSizeToHighlight && !business.IsBuilt)
                {
                    subcatchmentToHighlight.Add(business);
                }
            }
            //Highlight subcatchments
            MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
        }
        else
        {
            //Pop up info message
            UIManager.Instance.MissingActionPointMessage();
            UIManager.Instance.BusinessButtonPressed();
        }
    }
    public void CheckGRSubcatchmentAvailabilities()
    {
        //Change InfrastructureBuilder status variables
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.GR;

        //Get needed subcatchements
        Subcatchment[] builtSubcatchments = MapManager.Instance.GetBuiltSubcatchments();
        int budget = ResourceManager.Instance.Budget;

        //CHECK ON COMMERCIAL AREA
        //Check if enough action points are available
        Dictionary<AreaUsage, AreaSize> typeAndSizeOfSubcatToHighlight = new Dictionary<AreaUsage, AreaSize>();
        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CActionPoints + 1)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetCommercialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetCommercialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetCommercialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Small);
                    }

                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Large);
            }
        }


        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CActionPoints)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetResidentialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetResidentialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.GR).CBudgetResidentialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Small);
                    }
                    else
                    {
                        //not enough budget message
                        UIManager.Instance.MissingBudgetMessage();
                        UIManager.Instance.GRButtonPressed();
                        return;
                    }
                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Large);
            }
        }
        else
        {
            //Pop up info message
            UIManager.Instance.MissingActionPointMessage();
            UIManager.Instance.GRButtonPressed();
            return;
        }

        //Get subcatchment needed based on area size and if built or not
        List<Subcatchment> subcatchmentToHighlight = new List<Subcatchment>();
        foreach (Subcatchment subcat in builtSubcatchments)
        {
            if (subcat.Usage.Equals(AreaUsage.Residential))
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Residential])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.GR))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.GRButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.GRButtonPressed();
                        return;
                    }
                }
            }
            else
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Commercial])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.GR))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.GRButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.GRButtonPressed();
                        return;
                    }
                }
            }
        }
        if (subcatchmentToHighlight.Count > 0)
        {
            MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
        }
        else
        {
            UIManager.Instance.SubcatchmentNotBuiltMessage();
            UIManager.Instance.GRButtonPressed();
            Debug.Log("LAST BUTTON PRESSED");
            return;
        }
    }
    public void CheckPPSubcatchmentAvailabilities()
    {
        //Change InfrastructureBuilder status variables
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.PP;

        //Get needed subcatchements
        Subcatchment[] builtSubcatchments = MapManager.Instance.GetBuiltSubcatchments();
        int budget = ResourceManager.Instance.Budget;

        //CHECK ON COMMERCIAL AREA
        //Check if enough action points are available
        Dictionary<AreaUsage, AreaSize> typeAndSizeOfSubcatToHighlight = new Dictionary<AreaUsage, AreaSize>();
        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CActionPoints + 1)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetCommercialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetCommercialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetCommercialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Small);
                    }
                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Large);
            }
        }


        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CActionPoints)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetResidentialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetResidentialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.PP).CBudgetResidentialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Small);
                    }
                    else
                    {
                        //not enough budget message
                        UIManager.Instance.MissingBudgetMessage();
                        UIManager.Instance.PPButtonPressed();
                        return;
                    }
                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Large);
            }
        }
        else
        {
            //Pop up info message
            UIManager.Instance.MissingActionPointMessage();
            UIManager.Instance.PPButtonPressed();
            return;
        }

        //Get subcatchment needed based on area size and if built or not
        List<Subcatchment> subcatchmentToHighlight = new List<Subcatchment>();
        foreach (Subcatchment subcat in builtSubcatchments)
        {
            if (subcat.Usage.Equals(AreaUsage.Residential))
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Residential])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.PP))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.PPButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.PPButtonPressed();
                        return;
                    }
                }
            }
            else
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Commercial])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.PP))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.PPButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.PPButtonPressed();
                        return;
                    }
                }
            }
        }
        if (subcatchmentToHighlight.Count > 0)
        {
            MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
        }
        else
        {
            UIManager.Instance.SubcatchmentNotBuiltMessage();
            UIManager.Instance.PPButtonPressed();
            return;
        }
    }
    public void CheckRBSubcatchmentAvailabilities()
    {
        //Change InfrastructureBuilder status variables
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.RB;

        //Get needed subcatchements
        Subcatchment[] builtSubcatchments = MapManager.Instance.GetBuiltSubcatchments();
        int budget = ResourceManager.Instance.Budget;

        //CHECK ON COMMERCIAL AREA
        //Check if enough action points are available
        Dictionary<AreaUsage, AreaSize> typeAndSizeOfSubcatToHighlight = new Dictionary<AreaUsage, AreaSize>();
        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CActionPoints + 1)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetCommercialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetCommercialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetCommercialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Small);
                    }
                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Commercial, AreaSize.Large);
            }
        }


        if (ResourceManager.Instance.ActionPoints >= CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CActionPoints)
        {
            //Determine size of commercial subcatchment to highlight
            if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetResidentialLarge)
            {
                if (budget < CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetResidentialMedium)
                {
                    if (budget > CostsManager.Instance.GetBGIStats(InfrastructureType.RB).CBudgetResidentialSmall)
                    {
                        typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Small);
                    }
                    else
                    {
                        //not enough budget message
                        UIManager.Instance.MissingBudgetMessage();
                        UIManager.Instance.RBButtonPressed();
                        return;
                    }
                }
                else
                {
                    typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Medium);
                }
            }
            else
            {
                typeAndSizeOfSubcatToHighlight.Add(AreaUsage.Residential, AreaSize.Large);
            }
        }
        else
        {
            //Pop up info message
            UIManager.Instance.MissingActionPointMessage();
            UIManager.Instance.RBButtonPressed();
            return;
        }

        //Get subcatchment needed based on area size and if built or not
        List<Subcatchment> subcatchmentToHighlight = new List<Subcatchment>();
        foreach (Subcatchment subcat in builtSubcatchments)
        {
            if (subcat.Usage.Equals(AreaUsage.Residential))
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Residential])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.RB))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.RBButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.RBButtonPressed();
                        return;
                    }
                }
            }
            else
            {
                if (subcat.Size <= typeAndSizeOfSubcatToHighlight[AreaUsage.Commercial])
                {
                    if (subcat.IsBuilt)
                    {
                        if (subcat.CanHostBGI(InfrastructureType.RB))
                        {
                            subcatchmentToHighlight.Add(subcat);
                        }
                        else
                        {
                            UIManager.Instance.NoSubcatchmentCanHostBGIMessage();
                            UIManager.Instance.RBButtonPressed();
                            return;
                        }
                    }
                    else
                    {
                        UIManager.Instance.SubcatchmentNotBuiltMessage();
                        UIManager.Instance.RBButtonPressed();
                        return;
                    }
                }
            }
        }
        if (subcatchmentToHighlight.Count > 0)
        {
            MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
        }
        else
        {
            UIManager.Instance.SubcatchmentNotBuiltMessage();
            UIManager.Instance.RBButtonPressed();
            return;
        }
    }

    public void ResetSelectedInfrastructure()
    {
        SelectedInfrastructure = InfrastructureType.Null;
        isBuilding = false;
    }

    public void BuildInfrastructure(Subcatchment subcatToBuildOn)
    {
        //check for different tipe of infrastructure to build
        switch (SelectedInfrastructure)
        {
            case InfrastructureType.House:
                {
                    BasicInfrastructureStats stats = new BasicInfrastructureStats();
                    stats = CostsManager.Instance.GetInfrastructureStats(InfrastructureType.House);
                    if (ResourceManager.Instance.ActionPoints >= stats.CActionPoints)
                    {
                        GeneralInfrastructureResourceUpdate(subcatToBuildOn, stats);
                        ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                        UIManager.Instance.HouseButtonPressed();
                    }
                    break;
                }
            case InfrastructureType.Business:
                {
                    BasicInfrastructureStats stats = new BasicInfrastructureStats();
                    stats = CostsManager.Instance.GetInfrastructureStats(InfrastructureType.Business);
                    if (ResourceManager.Instance.ActionPoints >= stats.CActionPoints)
                    {
                        GeneralInfrastructureResourceUpdate(subcatToBuildOn, stats);
                        ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                        UIManager.Instance.BusinessButtonPressed();
                    }
                    break;
                }
            case InfrastructureType.RB:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.RB);
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.RBButtonPressed();
                    break;
                }
            case InfrastructureType.GR:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.GR);
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.GRButtonPressed();
                    break;
                }
            case InfrastructureType.PP:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.PP);
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.PPButtonPressed();
                    break;
                }
            default:
                Debug.LogWarning("No infrstructure selected");
                return;

        }

        //compare Action points needed

        //update resources
        //Dehighlight subcat
        MapManager.Instance.DehighlightBuildableSubcatchments();
        //reset selected infra
        ResetSelectedInfrastructure();
    }

    private void GeneralInfrastructureResourceUpdate(Subcatchment subcatToBuildOn, BasicInfrastructureStats stats)
    {
        ResourceManager.Instance.UpdateActionPoints(-stats.CActionPoints);
        switch (subcatToBuildOn.Size)
        {
            case AreaSize.Small:
                ResourceManager.Instance.UpdateBudget(-stats.CBudgetSmall);
                ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenNumberSmall);
                break;
            case AreaSize.Medium:
                ResourceManager.Instance.UpdateBudget(-stats.CBudgetMedium);
                ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenNumberMedium);
                break;
            case AreaSize.Large:
                ResourceManager.Instance.UpdateBudget(-stats.CBudgetLarge);
                ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenNumberLarge);
                break;

        }
        subcatToBuildOn.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);
    }

    private void BGIResourceUpdate(Subcatchment subcatToBuildOn, BasicBGIStats stats)
    {
        ResourceManager.Instance.UpdateActionPoints(-stats.CActionPoints);
        if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
        {
            switch (subcatToBuildOn.Size)
            {
                case AreaSize.Small:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialSmall);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Medium:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialMedium);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Large:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialLarge);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;

            }
        }
        else
        {
            switch (subcatToBuildOn.Size)
            {
                case AreaSize.Small:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialSmall);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Medium:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialMedium);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Large:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialLarge);
                    ResourceManager.Instance.UpdateCitiyenNumber(stats.BCitizenSatisfaction);
                    break;

            }
        }
        subcatToBuildOn.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);
    }
}
