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
        /*TODO: implement a Costs*/

        //Determine max area size to highlight in order to chose the subcats to highlight
        AreaSize maxSizeToHighlight = AreaSize.Null;
        if (budget < 2700000)
        {
            if (budget < 1600000)
            {
                if (budget > 1100000)
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
    public void CheckCommercialSubcatchmentAvailabilities()
    {
        isBuilding = true;
        SelectedInfrastructure = InfrastructureType.Business;
        Subcatchment[] businessSubcatchments = MapManager.Instance.GetBusinessSubcatchments();
        int budget = ResourceManager.Instance.Budget;
        /*TODO: implement a Costs*/

        //Determine max area size to highlight in order to chose the subcats to highlight
        AreaSize maxSizeToHighlight = AreaSize.Null;
        if (budget < 7500000)
        {
            if (budget < 4500000)
            {
                if (budget > 3100000)
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
        MapManager.Instance.HighlightBuildableSubcatchments(subcatchmentToHighlight.ToArray());
    }
    public void CheckGRSubcatchmentAvailabilities()
    {
        SelectedInfrastructure = InfrastructureType.GR;
    }
    public void CheckPPSubcatchmentAvailabilities()
    {
        SelectedInfrastructure = InfrastructureType.PP;
    }
    public void CheckRBSubcatchmentAvailabilities()
    {
        SelectedInfrastructure = InfrastructureType.RB;
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
                    stats = CostsManager.Instance.GetInfrastructureStats("House");
                    if (ResourceManager.Instance.ActionPoints > stats.CActionPoints)
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
                    stats = CostsManager.Instance.GetInfrastructureStats("Business");
                    if (ResourceManager.Instance.ActionPoints > stats.CActionPoints)
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
                    stats = CostsManager.Instance.GetBGIStats("Rain Barrel");
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
                    break;
                }
            case InfrastructureType.GR:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats("Green Roof");
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
                    break;
                }
            case InfrastructureType.PP:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats("Permeable Pavement");
                    if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
                    {
                        stats.CActionPoints += 1;
                    }
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

    private static void GeneralInfrastructureResourceUpdate(Subcatchment subcatToBuildOn, BasicInfrastructureStats stats)
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
        subcatToBuildOn.BuildInfrastructureOnSubcatchment();
    }
}
