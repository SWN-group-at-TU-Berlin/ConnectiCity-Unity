using System;
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
        SelectedInfrastructure = InfrastructureType.House;
    }
    #endregion

    private void Start()
    {
        //ResourceManager.Instance.UpdateActionPoints(6);
        //UIManager.Instance.HouseButtonPressed();
    }

    public InfrastructureType SelectedInfrastructure { get; set; }
    public Subcatchment SelectedSubcatchment { get; set; }
    public bool isBuilding { get; set; }

    /*Purpose of the function:
     - Show all subcatchments info panels
     - Highlight all buildable subcatchments
     */
    public void EnterInfrastructureBuildStatus()
    {
        //change selected infrastructure to building
        SelectedInfrastructure = InfrastructureType.Building;

        //instantiate a list of buildable subcatchments
        List<Subcatchment> buildableSubcatchments = new List<Subcatchment>();

        //foreach subcatchment:
        foreach (Subcatchment subcatchment in MapManager.Instance.GetSubcatchments())
        {
            //if subcat not built
            if (!subcatchment.IsBuilt)
            {

                //show info panels
                subcatchment.ShowBuildInfoPanels(InfrastructureType.Building);

                //get budget from current resources
                float currentBudget = ResourceManager.Instance.Budget;

                //get ap from current resources
                float currentAp = ResourceManager.Instance.ActionPoints;

                //get build costs for current subcatchment
                float buildCost = CostsManager.Instance.GetSubcatchmentBuildCosts(subcatchment.SubcatchmentNumber, BuildStatus.Built);

                //get ap costs for current subcatchment
                float apCost = CostsManager.Instance.GetActionPointCosts(subcatchment.SubcatchmentNumber, BuildStatus.Built);

                //if: ap costs < ap AND build costs < budget
                if (apCost <= currentAp && buildCost <= currentBudget)
                {
                    //add subcatchment to buildable subcatchments
                    buildableSubcatchments.Add(subcatchment);
                }
            }

        }
        //highlight those for which you have enough resources to build
        MapManager.Instance.DehighlightBuildableSubcatchments();
        MapManager.Instance.HighlightBuildableSubcatchments(buildableSubcatchments.ToArray());
    }

    public void EnterBGIBuildStatus(InfrastructureType BGIToBuild)
    {
        //update selected infrastructure
        SelectedInfrastructure = BGIToBuild;

        //instantiate a list of buildable subcatchments
        List<Subcatchment> buildableSubcatchments = new List<Subcatchment>();

        //foreach subcatchment:
        foreach (Subcatchment subcatchment in MapManager.Instance.GetSubcatchments())
        {
            //if subcat not built
            if (!subcatchment.SubcatchmentHostsBGI(BGIToBuild))
            {

                //show info panels
                subcatchment.ShowBuildInfoPanels(BGIToBuild);

                //get budget from current resources
                float currentBudget = ResourceManager.Instance.Budget;

                //get ap from current resources
                float currentAp = ResourceManager.Instance.ActionPoints;

                //get build costs for current subcatchment
                float buildCost = CostsManager.Instance.GetSubcatchmentBuildCosts(subcatchment.SubcatchmentNumber, BuildStatus.Built);

                //get ap costs for current subcatchment
                float apCost = CostsManager.Instance.GetActionPointCosts(subcatchment.SubcatchmentNumber, BuildStatus.Built);

                //if: ap costs < ap AND build costs < budget
                if (apCost <= currentAp && buildCost <= currentBudget && subcatchment.IsBuilt)
                {
                    //add subcatchment to buildable subcatchments
                    buildableSubcatchments.Add(subcatchment);
                }
            }

        }
        //highlight those for which you have enough resources to build
        MapManager.Instance.DehighlightBuildableSubcatchments();
        MapManager.Instance.HighlightBuildableSubcatchments(buildableSubcatchments.ToArray());
    }

    public void ResetSelectedInfrastructure()
    {
        SelectedInfrastructure = InfrastructureType.Null;
        isBuilding = false;
    }

    public void BuildInfrastructure()
    {
        //recover infrastructure to build
        BuildStatus infrastructureToBuild = SelectedSubcatchment.ConvertInfrastructureTypeToBuildStatus(SelectedInfrastructure);

        //recover the newBuildStatus of the subcat (AKA: BGI combinations)
        BuildStatus newBuildStatus = SelectedSubcatchment.NewBuildStatus(infrastructureToBuild);

        //recover costs
        float buildCost = CostsManager.Instance.GetSubcatchmentBuildCosts(SelectedSubcatchment.SubcatchmentNumber, infrastructureToBuild);

        //update budget
        ResourceManager.Instance.UpdateBudget((int)buildCost);

        if (SelectedInfrastructure.Equals(InfrastructureType.Building))
        {
            //recover benefit
            float benefit = SelectedSubcatchment.SubcatchmentBenefit;
            //if business
            if (SelectedSubcatchment.Usage.Equals(AreaUsage.Commercial))
            {
                //update jobs
                ResourceManager.Instance.UpdateJobs((int)benefit);
            }
            else
            {
                //update hostable spots
                ResourceManager.Instance.UpdateHostablePeople((int)benefit);
            }
        }

        //update build status of subcatchment
        SelectedSubcatchment.BuildStatus = newBuildStatus;

        //update subcat visual
        SelectedSubcatchment.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);

        //Dehighlight subcat
        MapManager.Instance.DehighlightBuildableSubcatchments();

        //reset selected infra
        ResetSelectedInfrastructure();

        /*//check for different tipe of infrastructure to build
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
                        ResourceManager.Instance.UpdateIncome(stats.BCitizenSatisfaction);
                        UIManager.Instance.BusinessButtonPressed();
                    }
                    break;
                }
            case InfrastructureType.RB:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.RB);

                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.RBButtonPressed();
                    break;
                }
            case InfrastructureType.GR:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.GR);

                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.GRButtonPressed();
                    break;
                }
            case InfrastructureType.PP:
                {
                    BasicBGIStats stats = new BasicBGIStats();
                    stats = CostsManager.Instance.GetBGIStats(InfrastructureType.PP);

                    BGIResourceUpdate(subcatToBuildOn, stats);
                    UIManager.Instance.PPButtonPressed();
                    break;
                }
            default:
                Debug.LogWarning("No infrstructure selected");
                return;

        }*/

        //exit build mode
        UIManager.Instance.ExitBuildMode();
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
                ResourceManager.Instance.UpdateIncome(stats.BIncomeSmall);
                ResourceManager.Instance.UpdateCitizenNumber(stats.BCitizenNumberSmall);
                break;
            case AreaSize.Medium:
                ResourceManager.Instance.UpdateBudget(-stats.CBudgetMedium);
                ResourceManager.Instance.UpdateIncome(stats.BIncomeMedium);
                ResourceManager.Instance.UpdateCitizenNumber(stats.BCitizenNumberMedium);
                break;
            case AreaSize.Large:
                ResourceManager.Instance.UpdateBudget(-stats.CBudgetLarge);
                ResourceManager.Instance.UpdateIncome(stats.BIncomeLarge);
                ResourceManager.Instance.UpdateCitizenNumber(stats.BCitizenNumberLarge);
                break;

        }
        subcatToBuildOn.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);
    }

    private void BGIResourceUpdate(Subcatchment subcatToBuildOn, BasicBGIStats stats)
    {
        int ap = stats.CActionPoints;
        if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
        {
            ap += 1;
        }
        ResourceManager.Instance.UpdateActionPoints(-ap);
        if (subcatToBuildOn.Usage.Equals(AreaUsage.Commercial))
        {
            switch (subcatToBuildOn.Size)
            {
                case AreaSize.Small:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialSmall);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Medium:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialMedium);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Large:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetCommercialLarge);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;

            }
        }
        else
        {
            switch (subcatToBuildOn.Size)
            {
                case AreaSize.Small:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialSmall);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Medium:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialMedium);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;
                case AreaSize.Large:
                    ResourceManager.Instance.UpdateBudget(-stats.CBudgetResidentialLarge);
                    ResourceManager.Instance.UpdateCitizenSatisfaction(stats.BCitizenSatisfaction);
                    break;

            }
        }
        subcatToBuildOn.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);
    }
}
