using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class RainEventsManager : MonoBehaviour
{
    private static RainEventsManager _instance;
    #region getter
    public static RainEventsManager Instance { get { return _instance; } }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [SerializeField] GameObject rainEventInfoPanel;
    [SerializeField] GameObject rainParticles;

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

    float runoffReductionPercentageAccumulated = 0;

    private void Start()
    {
        rainParticles.GetComponent<ParticleSystem>().Stop();
        rainEventInfoPanel.GetComponent<Animator>().enabled = false;
    }

    /*TEST THIS FUNCTION:
     to test this function you need to
    1. set all the runoff reduction values manually in the inspector of this game object
    3. create a button that allow testing
    */
    public IEnumerator RainEvent()
    {
        float rainEventChance = UnityEngine.Random.Range(0f, 1f);
        int rainEventIntesity = 1; //50% chance
        if(rainEventChance < 0.5f)
        {
            rainEventIntesity = 2; // 30% chance

                if(rainEventChance < 0.2f)
                {
                    rainEventIntesity = 3; //20% chance
                }
        }

        //"turn off the light"
        StartCoroutine(MapManager.Instance.FadeOffLights());

        //show rain event info panel
        rainEventInfoPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "A rain event of intesity " + rainEventIntesity + " is coming!";
        rainEventInfoPanel.GetComponent<Animator>().enabled = true;
        rainEventInfoPanel.GetComponent<Animator>().Play("Appear", -1, 0f);
        yield return new WaitForSeconds(1);

        //start rain particles
        rainParticles.GetComponent<ParticleSystem>().Play();

        //pick random rain event intensity

        //pick 2 random subcats
        int subcatNum1 = UnityEngine.Random.Range(1, 12 + 1);
        int subcatNum2 = UnityEngine.Random.Range(1, 12 + 1);
        Debug.Log("Subcat affected: " + subcatNum1 + "; " + subcatNum2 + ";");

        while (subcatNum1 == subcatNum2)
        {
            subcatNum2 = UnityEngine.Random.Range(1, 12 + 1);
        }
        Subcatchment subcat1 = MapManager.Instance.GetSubcatchment(subcatNum1);
        Subcatchment subcat2 = MapManager.Instance.GetSubcatchment(subcatNum2);
        //get budget loss
        int budgetLossSubcat1 = CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat1, rainEventIntesity);
        int budgetLossSubcat2 = CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat2, rainEventIntesity);
        int budgetLoss = budgetLossSubcat1 + budgetLossSubcat2;
        //get deactivation
        //get citizen satisfaction loss
        float runoffReductionPercentageSubcat1 = GetRunoffReductionPercentage(subcat1, rainEventIntesity);
        float runoffReductionPercentageSubcat2 = GetRunoffReductionPercentage(subcat2, rainEventIntesity);
        int citizenSatisfactionDecresase1 = 0;
        int citizenSatisfactionDecresase2 = 0;
        if (subcat1.IsBuilt)
        {
            citizenSatisfactionDecresase1 = GetCitizenSatisfactionModifier(runoffReductionPercentageSubcat1, rainEventIntesity);
        }
        if (subcat2.IsBuilt)
        {
            citizenSatisfactionDecresase2 = GetCitizenSatisfactionModifier(runoffReductionPercentageSubcat2, rainEventIntesity);
        }

        yield return new WaitForSeconds(4);
        rainParticles.GetComponent<ParticleSystem>().Stop();


        //Call UI text effect on budget decrease
        UIManager.Instance.ShowFloatingTxt(-budgetLossSubcat1, "b", subcat1);
        ResourceManager.Instance.UpdateBudget(-budgetLossSubcat1);
        UIManager.Instance.ShowFloatingTxt(-budgetLossSubcat2, "b", subcat2);
        ResourceManager.Instance.UpdateBudget(-budgetLossSubcat2);
        yield return new WaitForSeconds(1);

        //Call UI text effect on citizen satisfaction
        if (citizenSatisfactionDecresase1 != 0 || citizenSatisfactionDecresase2 != 0)
        {
            if (citizenSatisfactionDecresase1 != 0)
            {
                UIManager.Instance.ShowFloatingTxt(citizenSatisfactionDecresase1, "c", subcat1);
                ResourceManager.Instance.UpdateCitizenSatisfaction(citizenSatisfactionDecresase1);
            }
            if (citizenSatisfactionDecresase2 != 0)
            {
                UIManager.Instance.ShowFloatingTxt(citizenSatisfactionDecresase2, "c", subcat2);
                ResourceManager.Instance.UpdateCitizenSatisfaction(citizenSatisfactionDecresase2);
            }

            yield return new WaitForSeconds(1);

        }

        subcat1.SetSubcatchmentActive(!CostsManager.Instance.SubcatchmentDeactivation(subcat1, rainEventIntesity));
        subcat2.SetSubcatchmentActive(!CostsManager.Instance.SubcatchmentDeactivation(subcat2, rainEventIntesity));
        yield return new WaitForSeconds(1);

        StartCoroutine(MapManager.Instance.FadeInLights());

        //ask to restart round
        RoundManager.Instance.StartRound();
    }

    public float GetRunoffReductionPercentage(Subcatchment subcat, int rainIntesity)
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
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.GR) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
                }
                if (rb)
                {
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.RB) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
                }
                if (pp)
                {
                    runoffReductionPercentage = Array.Find(SingleBGIRunoffReduction.ToArray(), elem => elem.BGI.Equals(InfrastructureType.PP) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
                }
            }
            else
            {
                if (gr && pp)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.GR_PP) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
                }
                if (gr && rb)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.RB_GR) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
                }
                if (rb && pp)
                {
                    runoffReductionPercentage = Array.Find(BGIComboRunoffReduction.ToArray(), elem => elem.BGICombo.Equals(BGICombo.PP_RB) && elem.RainfallIntensity.Equals(rainIntesity)).RunoffReductionPercentage;
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