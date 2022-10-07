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

                //if: ap costs < ap AND build costs < budget
                if (buildCost <= currentBudget)
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
            if (!subcatchment.SubcatchmentHostsBGI(BGIToBuild) && subcatchment.BGIHosted.Count < 2)
            {

                //show info panels
                subcatchment.ShowBuildInfoPanels(BGIToBuild);

                //get budget from current resources
                float currentBudget = ResourceManager.Instance.BGIbudget;

                //get ap from current resources
                float currentAp = ResourceManager.Instance.ActionPoints;

                BuildStatus specificSubcatBGI = subcatchment.ConvertInfrastructureTypeToBuildStatus(BGIToBuild);

                //get build costs for current subcatchment
                float buildCost = CostsManager.Instance.GetSubcatchmentBuildCosts(subcatchment.SubcatchmentNumber, specificSubcatBGI);

                //get ap costs for current subcatchment
                float apCost = CostsManager.Instance.GetActionPointCosts(subcatchment.SubcatchmentNumber, specificSubcatBGI);

                //if: ap costs < ap AND build costs < budget
                if (buildCost <= currentBudget && subcatchment.IsBuilt)
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


        if (SelectedInfrastructure.Equals(InfrastructureType.Building))
        {
            //update budget
            ResourceManager.Instance.UpdateBudget((int)buildCost);
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
        } else
        {
            //update budget
            ResourceManager.Instance.UpdateBGIBudget((int)buildCost);
        }

        //update build status of subcatchment
        SelectedSubcatchment.BuildStatus = newBuildStatus;

        //update subcat visual
        SelectedSubcatchment.BuildInfrastructureOnSubcatchment(SelectedInfrastructure);

        //update income
        ResourceManager.Instance.UpdateIncome();

        //Dehighlight subcat
        MapManager.Instance.DehighlightBuildableSubcatchments();


        //reset selected infra
        ResetSelectedInfrastructure();

        //exit build mode
        UIManager.Instance.ExitBuildMode();
        //Dehighlight subcat
        MapManager.Instance.DehighlightBuildableSubcatchments();
        //reset selected infra
        ResetSelectedInfrastructure();
    }
}
