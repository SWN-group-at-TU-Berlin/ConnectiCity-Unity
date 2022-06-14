using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfrastructureBuilder : MonoBehaviour
{
    public static InfrastructureBuilder _instance;
    #region singleton
    public InfrastructureBuilder Instance { get { return _instance; } }
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
        foreach(Subcatchment house in houseSubcatchments)
        {
            if(house.Size <= maxSizeToHighlight && !house.IsBuilt)
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

    public void BuildInfrastructure()
    {
        // Give permission to build infrastructure
    }
}
