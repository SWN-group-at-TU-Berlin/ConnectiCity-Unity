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
        rainParticles.GetComponent<ParticleSystem>().Stop();
        runoffs = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
        runoffReductionPercentages = new Dictionary<int, Dictionary<SubcatchmentKey, float>>();
        rainPerRound = new Dictionary<int, float>();
        subcatchmentsRainDistribution = new Dictionary<int, float>();
    }

    [SerializeField] GameObject rainEventInfoPanel;
    [SerializeField] GameObject rainParticles;
    [SerializeField] float flashFloodThreshold = 670;
    #region getter
    public float FlashFloodThreshold { get { return flashFloodThreshold; } }
    #endregion

    [SerializeField] float rainPredictionUncertanty = 0.05f;
    #region getter
    public float RainPredictionUncertanty { get { return rainPredictionUncertanty; } }
    #endregion

    float totalRunoff;
    #region getter
    public float TotalRunoff { get { return totalRunoff; } }
    #endregion

    float rainPredictionDeviationVal = 0;
    #region getter
    public float RainPredictionDeviationValue { get { return rainPredictionDeviationVal; } }
    #endregion

    //DEPRECATED
    [HideInInspector]
    [SerializeField] int maxRainEvent1Threshold;
    [HideInInspector]
    [SerializeField] int maxRainEvent2Threshold;
    [HideInInspector]
    [SerializeField] int maxRainEvent3Threshold;

    [SerializeField] int currentRainIntensity;
    #region getter
    public int CurrentRainIntensity { get { return currentRainIntensity; } }
    #endregion

    Dictionary<int, Dictionary<SubcatchmentKey, float>> runoffs;
    #region getter
    public Dictionary<int, Dictionary<SubcatchmentKey, float>> Runoffs { get { return runoffs; } }
    #endregion

    Dictionary<int, Dictionary<SubcatchmentKey, float>> runoffReductionPercentages;
    #region getter
    public Dictionary<int, Dictionary<SubcatchmentKey, float>> RunoffReductionPercentages { get { return runoffReductionPercentages; } }
    #endregion

    Dictionary<int, float> subcatchmentsRainDistribution;
    #region getter
    public Dictionary<int, float> SubcatchmentsRainDistribution { get { return subcatchmentsRainDistribution; } }
    #endregion

    Dictionary<int, float> rainPerRound;
    #region getter
    public Dictionary<int, float> RainPerRound { get { return rainPerRound; } }
    #endregion

    //DEPRECATED
    [SerializeField] List<RunoffReductionPercentageSingleBGI> _singleBGIRunoffReduction;
    #region getter
    public List<RunoffReductionPercentageSingleBGI> SingleBGIRunoffReduction { get { return _singleBGIRunoffReduction; } }
    #endregion
    //DEPRECATED
    [SerializeField] List<RunoffReductionPercentageBGICombo> _BGIComboRunoffReduction;
    #region getter
    public List<RunoffReductionPercentageBGICombo> BGIComboRunoffReduction { get { return _BGIComboRunoffReduction; } }
    #endregion

    private void Start()
    {
        currentRainIntensity = 1;
        rainEventInfoPanel.GetComponent<Animator>().enabled = false;

        runoffs = DataReader.Instance.RunoffsDictionaries;
        runoffReductionPercentages = DataReader.Instance.RunoffReductionPercentagesDictionaries;
        subcatchmentsRainDistribution = DataReader.Instance.SubcatchmentsRainDistributionDictionary;
        rainPerRound = DataReader.Instance.RainPerRoundDictionary;
    }

    /*Purpose of the function:
     *Sum all the runoffs of the built subcatchments at the current build status
     */
    public float CalculateTotalRunoff(bool considerPredictionUncertanty = false)
    {
        //initialize variable
        float totRunoff = 0;

        //retrieve built subcatchments
        Subcatchment[] builtSubcats = MapManager.Instance.GetBuiltSubcatchments();

        //Retrieve rain precipitation per round
        float currentRain = rainPerRound[RoundManager.Instance.CurrentRound];

        //calculate and add the prediction uncertanty value to current rain of the round
        if (considerPredictionUncertanty)
        {
            currentRain += CalculatePredictionUncertantyValue(currentRain); ;
        }

        foreach (Subcatchment subcat in builtSubcats)
        {
            //set runoff reduction to 0
            float runoffReductionpercentage = 0;
            //retrieve runoff reduction only if subcat build status isn't Built or Unbuilt
            if (!subcat.BuildStatus.Equals(BuildStatus.Built) && !subcat.BuildStatus.Equals(BuildStatus.Unbuild))
            {
                runoffReductionpercentage = runoffReductionPercentages[currentRainIntensity][new SubcatchmentKey(subcat.SubcatchmentNumber, subcat.BuildStatus)] / 100;
            }


            //Calculate rain on subcatchment
            float subcatRain = currentRain * subcatchmentsRainDistribution[subcat.SubcatchmentNumber];

            //Calculate runoff reduction of subcat for specific round and rain precipitation
            float subcatRuonff = subcatRain - (subcatRain * runoffReductionpercentage);

            //sum everything to total runoff
            totRunoff += subcatRuonff;
        }
        return totRunoff;
    }

    private float CalculatePredictionUncertantyValue(float currentRain)
    {
        //if the prediction deviation val has never been calculated
        //random value to determine if prediction uncertanty value is positive or negative
        int signModifierValue = 1;
        if (UnityEngine.Random.Range(0f, 1f) <= 0.5f)
        {
            signModifierValue = -1;
        }

        //determine actual random deviation on prediction based on rain prediction uncertanty
        float predictionDeviaton = UnityEngine.Random.Range(0f, RainPredictionUncertanty);

        //multiply rain prediction uncertanty per current Rain to determine value to add to current rain
        float predictionUncertantyRainValue = currentRain * predictionDeviaton * signModifierValue;
        rainPredictionDeviationVal = predictionUncertantyRainValue;
        return predictionUncertantyRainValue;
    }

    public void UpdateTotalRunoff()
    {
        totalRunoff = CalculateTotalRunoff();
        Debug.Log("Total runoff: " + TotalRunoff);
    }

    public float CalculateFlashFloorRisk()
    {
        UpdateTotalRunoff();
        return (totalRunoff / flashFloodThreshold) * 100;
    }

    public IEnumerator RainEvent()
    {
        float rainEventChance = UnityEngine.Random.Range(0f, 1f);
        int rainEventIntesity = 1; //50% chance
        if (rainEventChance < 0.5f)
        {
            rainEventIntesity = 2; // 30% chance

            if (rainEventChance < 0.2f)
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

        //Avoid selection of same subcatchment
        while (subcatNum1 == subcatNum2)
        {
            subcatNum2 = UnityEngine.Random.Range(1, 12 + 1);
        }

        Debug.Log("Subcat affected: " + subcatNum1 + "; " + subcatNum2 + ";");
        Subcatchment subcat1 = MapManager.Instance.GetSubcatchment(subcatNum1);
        Subcatchment subcat2 = MapManager.Instance.GetSubcatchment(subcatNum2);

        //get budget loss
        int budgetLossSubcat1 = CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat1, rainEventIntesity);
        int budgetLossSubcat2 = CostsManager.Instance.GetRainfallDamagesCostsPerSubcatchment(subcat2, rainEventIntesity);
        int budgetLoss = budgetLossSubcat1 + budgetLossSubcat2;

        //get deactivation
        //get citizen satisfaction loss
        //float runoffReductionPercentageSubcat1 = GetRunoffReductionPercentage(subcat1, rainEventIntesity);
        //float runoffReductionPercentageSubcat2 = GetRunoffReductionPercentage(subcat2, rainEventIntesity);
        int citizenSatisfactionDecresase1 = 0;
        int citizenSatisfactionDecresase2 = 0;
        if (subcat1.IsBuilt)
        {
            //citizenSatisfactionDecresase1 = GetCitizenSatisfactionModifier(runoffReductionPercentageSubcat1, rainEventIntesity);
        }
        if (subcat2.IsBuilt)
        {
            //citizenSatisfactionDecresase2 = GetCitizenSatisfactionModifier(runoffReductionPercentageSubcat2, rainEventIntesity);
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

    public float GetRunoffReductionPercentage(int rainLevelIntesity, int subcatNumber, BuildStatus subcatStatus)
    {
        float runoffReductionPercentage = 0;
        if (!subcatStatus.Equals(BuildStatus.Built) && !subcatStatus.Equals(BuildStatus.Unbuild))
        {
            runoffReductionPercentage = runoffReductionPercentages[CurrentRainIntensity][new SubcatchmentKey(subcatNumber, subcatStatus)];
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

    public void ConfrontFlashFloodRisks()
    {
        float runoffBasedOnPrediction = CalculateTotalRunoff();
        float actualrunoff = CalculateTotalRunoff(true);

        bool flashflood = actualrunoff > flashFloodThreshold;

        Debug.Log("Predicted runoff: " + runoffBasedOnPrediction + " | actrual runoff: " + actualrunoff + " | flash flood: " + flashflood);
    }

    public bool FlashFloodCheck()
    {
        return CalculateTotalRunoff(true) > flashFloodThreshold;
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