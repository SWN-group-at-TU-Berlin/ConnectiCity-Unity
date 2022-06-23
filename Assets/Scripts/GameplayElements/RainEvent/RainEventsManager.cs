using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class RainEventsManager : MonoBehaviour
{
    [SerializeField] int maxRainEvent1Threshold;
    [SerializeField] int maxRainEvent2Threshold;
    [SerializeField] int maxRainEvent3Threshold;

    [SerializeField] List<RunoffReductionPercentageSingleBGI> _singleBGIRunoffReduction;
    #region getter
    public List<RunoffReductionPercentageSingleBGI> SingleBGIRunoffReduction { get { return _singleBGIRunoffReduction; } }
    #endregion
    [SerializeField] List<RunoffReductionPercentageBGICombo> _BGIComboRunoffReduction;
    #region getter
    public List<RunoffReductionPercentageBGICombo> BGIComboRunoffReduction { get { return _BGIComboRunoffReduction; } }
    #endregion

    /*TEST THIS FUNCTION:
     to test this function you need to
    1. set all the runoff reduction values manually in the inspector of this game object
    3. create a button that allow testing
    */
    public void RainEvent()
    {
        //pick random rain event intensity
        int rainEventIntesity = UnityEngine.Random.Range(1, 3+1);

        //pick 2 random subcats
        int subcatNum1 = UnityEngine.Random.Range(1, 12+1);
        int subcatNum2 = UnityEngine.Random.Range(1, 12+1);
        while (subcatNum1 == subcatNum2)
        {
            subcatNum2 = UnityEngine.Random.Range(1, 12+1);
        }
        Subcatchment subcat1 = MapManager.Instance.GetSubcatchment(subcatNum1);
        Subcatchment subcat2 = MapManager.Instance.GetSubcatchment(subcatNum2);
        //get budget loss
        int budgetLoss = CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat1, rainEventIntesity) + CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat2, rainEventIntesity);
        //get deactivation
        bool deactivation1 = CostsManager.Instance.SubcatchmentDeactivation(subcat1, rainEventIntesity);
        bool deactivation2 = CostsManager.Instance.SubcatchmentDeactivation(subcat2, rainEventIntesity);
        //get citizen satisfaction loss
        int citizenSatisfactionDecresase1 = GetCitizenSatisfactionModifier(GetRunoffReductionPercentage(subcat1), rainEventIntesity);
        int citizenSatisfactionDecresase2 = GetCitizenSatisfactionModifier(GetRunoffReductionPercentage(subcat2), rainEventIntesity);

        /*TEST THIS */
        Debug.Log("Rain event intesity: " + rainEventIntesity + "\n"
                   + "Subcatchment affected: " + "\n"
                   + "- "+ subcat1.SubcatchmentNumber + " deactivation: " + deactivation1 + " citizen satisfaction loss: " + citizenSatisfactionDecresase1 + "\n"
                   + "- " + subcat2.SubcatchmentNumber + " deactivation: " + deactivation2 + " citizen satisfaction loss: " + citizenSatisfactionDecresase2 + "\n"
                   + "Total budget loss: " + budgetLoss + "\n");

    }

    public float GetRunoffReductionPercentage(Subcatchment subcat)
    {
        float runoffReductionPercentage = 0;
        bool gr = false;
        bool pp = false;
        bool rb = false;
        if (subcat.IsBuilt && subcat.BGIHosted.Count > 0)
        {
            foreach (InfrastructureType bgi in subcat.BGIHosted)
            {
                if (!gr)
                {
                    if (bgi.Equals(InfrastructureType.GR))
                    {
                        gr = true;
                    }
                }
                if (!rb)
                {
                    if (bgi.Equals(InfrastructureType.GR))
                    {
                        rb = true;
                    }
                }
                if (!pp)
                {
                    if (bgi.Equals(InfrastructureType.GR))
                    {
                        pp = true;
                    }
                }

            }
            if (subcat.BGIHosted.Count == 1)
            {
                if (gr)
                {
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.GR)).RunoffReductionPercentage;
                }
                if (rb)
                {
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.RB)).RunoffReductionPercentage;
                }
                if (pp)
                {
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.PP)).RunoffReductionPercentage;
                }
            }
            else
            {
                if (gr && pp)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.GR_PP)).RunoffReductionPercentage;
                }
                if (gr && rb)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.RB_GR)).RunoffReductionPercentage;
                }
                if (rb && pp)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.PP_RB)).RunoffReductionPercentage;
                }
            }
        }
        return runoffReductionPercentage;
    }

    public int GetCitizenSatisfactionModifier(float runoffReductionPercetnage, int rainEventIntensity)
    {
        int mod = 0;
        if (rainEventIntensity == 1)
        {
            if (runoffReductionPercetnage > maxRainEvent1Threshold * 0.9f)
            {
                mod = 2;
            }
            else if (runoffReductionPercetnage > maxRainEvent1Threshold * 0.8f)
            {
                mod = 1;
            }
            else if (runoffReductionPercetnage < maxRainEvent1Threshold * 0.1f)
            {
                mod = -1;
            }


        }
        else if (rainEventIntensity == 2)
        {
            if (runoffReductionPercetnage > maxRainEvent1Threshold * 0.8f)
            {
                mod = 1;
            }
            else if (runoffReductionPercetnage < maxRainEvent1Threshold * 0.1f)
            {
                mod = -1;
            }
        }
        else if (rainEventIntensity == 3)
        {
            if (runoffReductionPercetnage > maxRainEvent1Threshold * 0.8f)
            {
                mod = 1;
            }
            else if (runoffReductionPercetnage < maxRainEvent1Threshold * 0.1f)
            {
                mod = -1;
            }
        }
        return mod;
    }

}

[System.Serializable]
public class RunoffReductionPercentageSingleBGI
{
    [SerializeField] int _rainfallIntensity;
    #region getter setter
    public int RainfallIntensity { get { return _rainfallIntensity; } }
    #endregion

    [SerializeField] AreaUsage _subcatchmentType;
    #region getter setter
    public AreaUsage SubcatchmentType { get { return _subcatchmentType; } }
    #endregion

    [SerializeField] InfrastructureType _bgi;
    #region getter setter
    public InfrastructureType BGI { get { return _bgi; } }
    #endregion

    [SerializeField] float _runoffReductionPercentage;
    #region getter setter
    public float RunoffReductionPercentage { get { return _runoffReductionPercentage; } }
    #endregion
}

[System.Serializable]
public class RunoffReductionPercentageBGICombo
{
    [SerializeField] int _rainfallIntensity;
    #region getter setter
    public int RainfallIntensity { get { return _rainfallIntensity; } }
    #endregion

    [SerializeField] AreaUsage _subcatchmentType;
    #region getter setter
    public AreaUsage SubcatchmentType { get { return _subcatchmentType; } }
    #endregion

    [SerializeField] BGICombo _bgiCombo;
    #region getter setter
    public BGICombo BGICombo { get { return _bgiCombo; } }
    #endregion

    [SerializeField] float _runoffReductionPercentage;
    #region getter setter
    public float RunoffReductionPercentage { get { return _runoffReductionPercentage; } }
    #endregion
}