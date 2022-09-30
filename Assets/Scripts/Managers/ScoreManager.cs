using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    #region singleton
    private static ScoreManager instance;
    #region getter
    public static ScoreManager Instance { get { return instance; } }
    #endregion
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        RoundsSnapshots = new Dictionary<int, RoundSnapshot>();
    }
    #endregion

    [Header("UI scoring system references")]
    [SerializeField] float maxPopDensity = 3f;
    [SerializeField] float minPopDensity = -3f;
    [SerializeField] float minUnemploymentRate = -100f;
    [SerializeField] float maxUnemploymentRate = 100f;

    [Header("Populaion Density")]
    [SerializeField] float minThresholdPD;
    [SerializeField] float midThresholdPD1;
    [SerializeField] float midThresholdPD2;
    [SerializeField] float maxThresholdPD;
    [SerializeField] float minusPDScore;
    [SerializeField] float plusPDScore;

    [Header("Unemployment rate")]
    [SerializeField] float minThresholdUR;
    [SerializeField] float midThresholdUR1;
    [SerializeField] float midThresholdUR2;
    [SerializeField] float maxThresholdUR;

    [Header("Unemployment rate")]
    bool flashFlood;


    [Header("Debug references")]
    [SerializeField] TextMeshProUGUI debugRates;

    //int = round; 
    Dictionary<int, RoundSnapshot> RoundsSnapshots;

    float socialScore;
    #region getter
    public float SocialScore { get { return socialScore; } }
    #endregion

    float environmentalScore;
    #region getter
    public float EnvironmentalScore { get { return environmentalScore; } }
    #endregion

    float economicScore;
    #region getter
    public float EconomicScore { get { return economicScore; } }
    #endregion

    float populationDensity;
    #region getter
    public float PopulationDensity { get { return populationDensity; } }
    #endregion

    float unempolymentPercentage;
    #region getter
    public float UnempolymentPercentage { get { return unempolymentPercentage; } }
    #endregion

    private void Start()
    {
        UIManager.Instance.SetMaxMinPopulationDensity(maxPopDensity, minPopDensity);
        UIManager.Instance.SetMaxMinUnemployment(maxUnemploymentRate, minUnemploymentRate);
    }

    public void UpdateScore()
    {
        RoundSnapshot roundSnap = new RoundSnapshot();

        //Pop density update
        roundSnap = UpdatePopulatioDensityScore(roundSnap);

        //Unemployment percentage update
        roundSnap = UpdateUnemploymentRateScore(roundSnap);

        //Emissios update

        //Flash flood update
        roundSnap = UpdateFlashFloodScore(roundSnap);

        //Traffic congestion update

        float socialScore = roundSnap.socialFlashFlood + roundSnap.socialPopulationDensity + roundSnap.socialUnemploymentRate;
        float economicScore = roundSnap.economicFlashFlood + roundSnap.economicPopulationDensity + roundSnap.economicUnemploymentRate;
        float environmentlaScore = roundSnap.environmentalFlashFlood + roundSnap.environmentalPopulationDensity + roundSnap.environmentalUnemploymentRate;

        RoundsSnapshots.Add(RoundManager.Instance.CurrentRound, roundSnap);

        Debug.Log("Social: " +
            "\nPop density: " + roundSnap.socialPopulationDensity + 
            "\nUnemployment: " + roundSnap.socialUnemploymentRate + 
            "\nFlahFlood: " + roundSnap.socialFlashFlood);

        Debug.Log("Environmental: " +
            "\nPop density: " + roundSnap.environmentalPopulationDensity +
            "\nUnemployment: " + roundSnap.environmentalUnemploymentRate +
            "\nFlahFlood: " + roundSnap.environmentalFlashFlood);

        Debug.Log("Economic: " +
            "\nPop density: " + roundSnap.economicPopulationDensity +
            "\nUnemployment: " + roundSnap.economicUnemploymentRate +
            "\nFlahFlood: " + roundSnap.economicFlashFlood);

        Debug.Log("Social: " + socialScore +
            "\nEnvironmental: " + environmentlaScore +
            "\nEconomic: " + economicScore);
    }

    public RoundSnapshot UpdatePopulatioDensityScore(RoundSnapshot snap)
    {
        float socialPopDensityScore = 0;
        float economicPopDensityScore = 0;
        float environmentPopDensityScore = 0;

        if (populationDensity < minThresholdPD)
        {
            socialPopDensityScore = -1;
        }
        else if (populationDensity > midThresholdPD1 && populationDensity < midThresholdPD2)
        {
            socialPopDensityScore = 1;
        }
        else if (populationDensity > maxThresholdPD)
        {
            socialPopDensityScore = -1;
        }

        snap.socialPopulationDensity = socialPopDensityScore;
        snap.economicPopulationDensity = economicPopDensityScore;
        snap.environmentalPopulationDensity = environmentPopDensityScore;

        return snap;
    }

    public RoundSnapshot UpdateUnemploymentRateScore(RoundSnapshot snap)
    {
        float socialUnemploymentRate = 0;
        float economicUnemploymentRate = 0;
        float environmentUnemploymentRate = 0;

        if (unempolymentPercentage < minThresholdUR)
        {
            economicUnemploymentRate = -1;
        }
        else if (unempolymentPercentage > midThresholdUR1 && unempolymentPercentage < midThresholdUR2)
        {
            economicUnemploymentRate = 1;
        }
        else if (populationDensity > maxThresholdUR)
        {
            socialUnemploymentRate = -1;
            economicUnemploymentRate = -1;
        }

        snap.socialUnemploymentRate = socialUnemploymentRate;
        snap.economicUnemploymentRate = economicUnemploymentRate;
        snap.environmentalUnemploymentRate = environmentUnemploymentRate;

        return snap;
    }

    public RoundSnapshot UpdateFlashFloodScore(RoundSnapshot snap)
    {
        float socialFlashFlood = 0;
        float economicFlashFlood = 0;
        float environmentFlashFlood = 0;

        if (RainEventsManager.Instance.FlashFloodCheck())
        {
            socialFlashFlood = -1;
            environmentFlashFlood = -1;
        }

        snap.socialUnemploymentRate = socialFlashFlood;
        snap.economicUnemploymentRate = economicFlashFlood;
        snap.environmentalUnemploymentRate = environmentFlashFlood;

        return snap;
    }

    public void UpdatePopulationDensity()
    {
        int pop = ResourceManager.Instance.CitizenNumber;
        int spots = ResourceManager.Instance.HostablePeople;
        populationDensity = CalculatePopulationDensity(pop, spots);

        //update UI
        UIManager.Instance.UpdatePopulationDesitySlider(populationDensity);
        debugRates.text = "Citizens: " + pop + " | Spots: " +
            spots +
            " | pop density: " +
            populationDensity.ToString("F2") +
            "\nWorking pop: " +
            ResourceManager.Instance.GetWorkingPopulation() +
            "Jobs: " +
            ResourceManager.Instance.Jobs +
            " | employment percentage: " +
            unempolymentPercentage + "%";
    }


    public float CalculatePopulationDensity(int citizenNumber, int spots)
    {
        return (float)citizenNumber / (float)spots;
    }

    public void UpdateUnemploymentPercentage()
    {
        int pop = ResourceManager.Instance.CitizenNumber;
        int jobs = ResourceManager.Instance.Jobs;
        unempolymentPercentage = CalculateUnemploymentPercentage(ResourceManager.Instance.GetWorkingPopulation(), jobs);
        //update UI
        UIManager.Instance.UpdateUnemploymentSlider(unempolymentPercentage);

        debugRates.text = "Citizens: " +
            pop +
            " | Spots: " +
            ResourceManager.Instance.HostablePeople +
            " | pop density: " +
            populationDensity.ToString("F2") +
            "\nWorking pop: " +
            ResourceManager.Instance.GetWorkingPopulation() +
            " | Jobs: " +
            jobs +
            " | employment percentage: " +
            unempolymentPercentage.ToString("F2") +
            "%";
    }

    public float CalculateUnemploymentPercentage(int workingPop, int jobs)
    {
        if (jobs == 0)
        {
            jobs = 1;
        }
        return (((float)workingPop / (float)jobs) - 1f) * 100f;
    }
}


public struct RoundSnapshot
{
    //population density
    public float socialPopulationDensity;
    public float environmentalPopulationDensity;
    public float economicPopulationDensity;

    //unempoloyment rate
    public float socialUnemploymentRate;
    public float environmentalUnemploymentRate;
    public float economicUnemploymentRate;

    //flash flood
    public float socialFlashFlood;
    public float environmentalFlashFlood;
    public float economicFlashFlood;
}