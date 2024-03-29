using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CostsManager : MonoBehaviour
{
    public static CostsManager _instance;
    #region singleton
    public static CostsManager Instance { get { return _instance; } }
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

        //allocate memory for dictionaries
        rainCosts = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
        buildingCosts = new Dictionary<SubcatchmentKey, float>();
        benefits = new Dictionary<Benefit, Dictionary<SubcatchmentKey, float>>();
        actionPoints = new Dictionary<SubcatchmentKey, float>();
    }
    #endregion

    public List<BasicInfrastructureStats> InfrastructuresStats;
    public List<BasicBGIStats> BGIStats;
    public List<FloodCostsPerSqrmt> ResidentialFloodCosts;
    public List<FloodCostsPerSqrmt> CommercialFloodCosts;

    //RAIN COSTS
    Dictionary<int, Dictionary<SubcatchmentKey, float>> rainCosts;
    #region getter
    public Dictionary<int, Dictionary<SubcatchmentKey, float>> RainCosts{ get { return rainCosts; } }
    #endregion

    //BUILDING COSTS
    Dictionary<SubcatchmentKey, float> buildingCosts;
    #region getter
    public Dictionary<SubcatchmentKey, float> BuildingCosts { get { return buildingCosts; } }
    #endregion

    //BENEFITS
    Dictionary<Benefit, Dictionary<SubcatchmentKey, float>> benefits;
    #region getter
    public Dictionary<Benefit, Dictionary<SubcatchmentKey, float>> Benefits { get { return benefits; } }
    #endregion

    //ACTION POINT
    Dictionary<SubcatchmentKey, float> actionPoints;
    #region getter
    public Dictionary<SubcatchmentKey, float> ActionPointsCosts { get { return actionPoints; } }
    #endregion

    Dictionary<int, float> transportCosts;
    #region getter
    public Dictionary<int, float> TransportCosts { get { return transportCosts; } }
    #endregion

    /*Dictionaries of data
     * citizen satisfaction loss
     */

    /*Functions to get costs
     * get rain costs per subcatchment (rain level, subcatchment)
     * get building costs (subcatchment number)
     * get building income increase (subcatchment number)
     * getbuilding citizen number increase (subcatchment number)
     * get building citizen satisfaction increase (subcatchment number)
     */

    private void Start()
    {
        //initialize data dictionaries
        rainCosts = DataReader.Instance.RainCostsDictionaries;
        buildingCosts = DataReader.Instance.BuildingCosts;
        benefits = DataReader.Instance.Benefits;
        actionPoints = DataReader.Instance.ActionPointsCosts;
        transportCosts = DataReader.Instance.TransportationCosts;
    }

    public float GetSubcatchmentRainCost(int rainLv, int subcatchmentNumber, BuildStatus subcatchmentBuildStatus)
    {
        return rainCosts[rainLv][new SubcatchmentKey(subcatchmentNumber, subcatchmentBuildStatus)];
    }

    public float GetSubcatchmentBuildCosts(int subcatchmentNumber, BuildStatus subcatchmentBuildStatus)
    {
        return buildingCosts[new SubcatchmentKey(subcatchmentNumber, subcatchmentBuildStatus)];
    }

    public float GetBuildBenefit(Benefit benefitType, int subcatchmentNumber, BuildStatus infrastructureType)
    {
        return benefits[benefitType][new SubcatchmentKey(subcatchmentNumber, infrastructureType)];
    }

    public float GetActionPointCosts(int subcatchmentNumber, BuildStatus infrastructureType)
    {
        return actionPoints[new SubcatchmentKey(subcatchmentNumber, infrastructureType)];
    }

    public BasicInfrastructureStats GetInfrastructureStats(InfrastructureType infrastructure)
    {
        BasicInfrastructureStats infrastructureStats = Array.Find(InfrastructuresStats.ToArray(), stats => stats.Name.Equals(infrastructure.ToString()));
        return infrastructureStats;
    }
    public BasicBGIStats GetBGIStats(InfrastructureType BGIName)
    {
        BasicBGIStats infrastructureStats = Array.Find(BGIStats.ToArray(), stats => stats.Name.Equals(BGIName.ToString()));
        return infrastructureStats;
    }

    public int GetRainfallDamagesCostsPerSubcatchment(Subcatchment subcat, int rainfallIntensity)
    {
        int damagesCosts = 0;
        int costPerSqrmt = 0;

        FloodCostsPerSqrmt floodCosts = GetFloodCostsByIntensity(subcat, rainfallIntensity);
        damagesCosts = GetSubcatFloodCostPerSqrmt(subcat, floodCosts) * (int)subcat.Size;
        return damagesCosts;
    }

    public int GetSubcatFloodCostPerSqrmt(Subcatchment subcat, FloodCostsPerSqrmt floodCosts)
    {
        int damagesCosts;
        if (!subcat.IsBuilt)
        {
            damagesCosts = floodCosts.Unbuild;
        }
        else if (!subcat.IsHostingBGI)
        {
            damagesCosts = floodCosts.Infrastructure;
        }
        else
        {
            string BGIs = "";
            foreach (InfrastructureType bgi in subcat.BGIHosted)
            {
                BGIs += bgi.ToString();
            }
            damagesCosts = floodCosts.GetBGIFloodCost(BGIs);
        }

        return damagesCosts;
    }

    private FloodCostsPerSqrmt GetFloodCostsByIntensity(Subcatchment subcat, int rainfallIntensity)
    {
        FloodCostsPerSqrmt flco = new FloodCostsPerSqrmt();
        switch (rainfallIntensity)
        {
            case 1:
                {
                    if (subcat.Usage.Equals(AreaUsage.Residential))
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(ResidentialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(1));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Residential Flood Cost count = " + ResidentialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    else
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(CommercialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(1));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Commercial Flood Cost count = " + CommercialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    break;
                }
            case 2:
                {
                    if (subcat.Usage.Equals(AreaUsage.Residential))
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(ResidentialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(2));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Residential Flood Cost count = " + ResidentialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    else
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(CommercialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(2));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Commercial Flood Cost count = " + ResidentialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    break;
                }
            case 3:
                {
                    if (subcat.Usage.Equals(AreaUsage.Residential))
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(ResidentialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(3));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Residential Flood Cost count = " + ResidentialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    else
                    {
                        try
                        {
                            flco = Array.Find<FloodCostsPerSqrmt>(CommercialFloodCosts.ToArray(), c => c.RainfallIntensity.Equals(3));
                        }
                        catch (Exception objReference)
                        {
                            Debug.LogWarning("Commercial Flood Cost count = " + ResidentialFloodCosts.Count + " exception: " + objReference);
                        }
                    }
                    break;
                }
        }

        return flco;
    }

    /*This function is the interpretation of the costs tables that we have written
     we need to identify a general rule to calculate deactivation based on the percentage of
     runoff retention given by BGIs. I already commented this in the table
     "CC_paperprototype_docu" in the tab "overview" (cell B53)*/
    public bool SubcatchmentDeactivation(Subcatchment subcat, int rainfallIntensity)
    {
        bool deactivation = false;
        if (rainfallIntensity > 1 && subcat.IsBuilt)
        {
            if (!subcat.IsHostingBGI)
            {
                if (rainfallIntensity == 3)
                {
                    deactivation = true;
                }
                else if (subcat.IsBuilt)
                {
                    deactivation = true;
                }
            }
            else
            {
                if (subcat.BGIHosted.Count > 1)
                {
                    if (rainfallIntensity == 3 &&
                        ((subcat.BGIHosted[0].Equals(InfrastructureType.GR) && subcat.BGIHosted[1].Equals(InfrastructureType.RB)
                        || (subcat.BGIHosted[1].Equals(InfrastructureType.GR) && subcat.BGIHosted[0].Equals(InfrastructureType.RB)))))
                    {
                        deactivation = true;
                    }
                }
                else
                {
                    if (rainfallIntensity == 3 && !subcat.BGIHosted[0].Equals(InfrastructureType.PP))
                    {
                        deactivation = true;
                    }
                }
            }
        }
        return deactivation;
    }

}


[System.Serializable]
public class FloodCostsPerSqrmt
{
    [SerializeField] int _rainfallIntensity;
    #region getter setter
    public int RainfallIntensity { get { return _rainfallIntensity; } set { _rainfallIntensity = value; } }
    #endregion
    [SerializeField] int _unbuild;
    #region getter setter
    public int Unbuild { get { return _unbuild; } set { _unbuild = value; } }
    #endregion
    [SerializeField] int _infrastructure;
    #region getter setter
    public int Infrastructure { get { return _infrastructure; } set { _infrastructure = value; } }
    #endregion
    [SerializeField] int _gr;
    #region getter setter
    public int GR { get { return _gr; } set { _gr = value; } }
    #endregion
    [SerializeField] int _pp;
    #region getter setter
    public int PP { get { return _pp; } set { _pp = value; } }
    #endregion
    [SerializeField] int _rb;
    #region getter setter
    public int RB { get { return _rb; } set { _rb = value; } }
    #endregion
    [SerializeField] int _gr_pp;
    #region getter setter
    public int GR_PP { get { return _gr_pp; } set { _gr_pp = value; } }
    #endregion
    [SerializeField] int _gr_rb;
    #region getter setter
    public int GR_RB { get { return _gr_rb; } set { _gr_rb = value; } }
    #endregion
    [SerializeField] int _rb_pp;
    #region getter setter
    public int RB_PP { get { return _rb_pp; } set { _rb_pp = value; } }
    #endregion

    public int GetBGIFloodCost(string infrastructure)
    {
        int cost = 0;
        if (infrastructure.Equals(nameof(GR)))
        {
            cost = GR;
        }
        if (infrastructure.Equals(nameof(PP)))
        {
            cost = PP;
        }
        if (infrastructure.Equals(nameof(RB)))
        {
            cost = RB;
        }
        if (infrastructure.Equals(nameof(GR) + nameof(PP)) || infrastructure.Equals(nameof(PP) + nameof(GR)))
        {
            cost = GR_PP;
        }
        if (infrastructure.Equals(nameof(GR) + nameof(RB)) || infrastructure.Equals(nameof(RB) + nameof(GR)))
        {
            cost = GR_RB;
        }
        if (infrastructure.Equals(nameof(RB) + nameof(PP)) || infrastructure.Equals(nameof(PP) + nameof(RB)))
        {
            cost = RB_PP;
        }
        return cost;
    }
}