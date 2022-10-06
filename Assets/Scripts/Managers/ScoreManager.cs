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
    [SerializeField] float maxTotalPoints = 30;
    #region getter
    public float MaxTotalPoints { get { return maxTotalPoints; } }
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
        roundSnap.Stats = new List<GameStat>();

        //Pop density update
        roundSnap.Stats.Add(UpdatePopulatioDensityScore());

        //Unemployment percentage update
        roundSnap.Stats.Add(UpdateUnemploymentRateScore());

        //Emissios update

        //Flash flood update
        roundSnap.Stats.Add(UpdateFlashFloodScore());

        //Traffic congestion update
        float socialScore = 0;
        float economicScore = 0;
        float environmentlaScore = 0;

        foreach(GameStat stat in roundSnap.Stats)
        {
            socialScore += stat.socialScore;
            economicScore += stat.economicScore;
            environmentlaScore += stat.environmentalScore;
        }


        roundSnap.total = socialScore + economicScore + environmentlaScore;

        RoundsSnapshots.Add(RoundManager.Instance.CurrentRound, roundSnap);
    }

    public GameStat UpdatePopulatioDensityScore()
    {
        GameStat popDensity = new GameStat();

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


        popDensity.name = "Population Density";
        popDensity.value = populationDensity.ToString();
        popDensity.socialScore = socialPopDensityScore;
        popDensity.economicScore = economicPopDensityScore;
        popDensity.environmentalScore = environmentPopDensityScore;

        return popDensity;
    }

    public GameStat UpdateUnemploymentRateScore()
    {
        GameStat UnemploymentRate = new GameStat();
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

        UnemploymentRate.name = "Unemployment rate";
        UnemploymentRate.value = unempolymentPercentage.ToString() + "%";
        UnemploymentRate.socialScore = socialUnemploymentRate;
        UnemploymentRate.economicScore = economicUnemploymentRate;
        UnemploymentRate.environmentalScore = environmentUnemploymentRate;

        return UnemploymentRate;
    }

    public GameStat UpdateFlashFloodScore()
    {
        GameStat FlashFlood = new GameStat();
        string val = "No Flood";
        float socialFlashFlood = 0;
        float economicFlashFlood = 0;
        float environmentFlashFlood = 0;

        if (RainEventsManager.Instance.FlashFloodCheck())
        {
            socialFlashFlood = -1;
            environmentFlashFlood = -1;
            val = "Flood";
        }

        FlashFlood.name = "FlashFlood";
        FlashFlood.value = val;
        FlashFlood.socialScore = socialFlashFlood;
        FlashFlood.economicScore = economicFlashFlood;
        FlashFlood.environmentalScore = environmentFlashFlood;

        return FlashFlood;
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

    public Dictionary<int, float> TotalScores()
    {
        Dictionary<int, float> totScores = new Dictionary<int, float>();
        for(int i = 1; i <= 10; i++)
        {
            if (RoundsSnapshots.ContainsKey(i))
            {
                totScores.Add(i, RoundsSnapshots[i].total);
            }
        }
        return totScores;
    }

    public RoundSnapshot GetRoundSnapshot(int round)
    {
        return RoundsSnapshots[round];
    }
}


public struct RoundSnapshot
{
    public List<GameStat> Stats;

    public float total;
}

public struct GameStat
{
    public string name;
    public string value;
    public float socialScore;
    public float environmentalScore;
    public float economicScore;
}