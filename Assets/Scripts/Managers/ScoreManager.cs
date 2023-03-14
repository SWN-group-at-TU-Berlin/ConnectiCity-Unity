using System.Collections.Generic;
using UnityEngine;

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

    [Header("Emissions threshold")]
    [SerializeField] float minThresholdEmissions = 1;
    [SerializeField] float maxThresholdEmissions = 8;

    [Header("Traffic threshold")]
    [SerializeField] float maxTrafficIntensityPercentage = 40;



    //int = round; 
    Dictionary<int, RoundSnapshot> RoundsSnapshots;

    float _socialScore;
    #region getter
    public float SocialScore { get { return _socialScore; } }
    #endregion

    float _environmentalScore;
    #region getter
    public float EnvironmentalScore { get { return _environmentalScore; } }
    #endregion

    float _economicScore;
    #region getter
    public float EconomicScore { get { return _economicScore; } }
    #endregion

    float populationDensity;
    #region getter
    public float PopulationDensity { get { return populationDensity; } }
    #endregion

    float unempolymentPercentage;
    #region getter
    public float UnempolymentPercentage { get { return unempolymentPercentage; } }
    #endregion

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

        //Emissions
        roundSnap.Stats.Add(UpdateEmissionScore());

        //Traffic
        roundSnap.Stats.Add(UpdateTrafficScore());

        //if final round, calculate final budget score
        if(RoundManager.Instance.CurrentRound == 11)
        {
            roundSnap.Stats.Add(UpdateFinalBudgetScore());
        }

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

        _socialScore += socialScore;
        _environmentalScore += environmentlaScore;
        _economicScore += economicScore;


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


        popDensity.name = "Population Capacity";
        popDensity.value = populationDensity.ToString("F2") + "%";
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
        else if (unempolymentPercentage > maxThresholdUR)
        {
            socialUnemploymentRate = -1;
            economicUnemploymentRate = -1;
        }

        UnemploymentRate.name = "Unemployment rate";
        UnemploymentRate.value = unempolymentPercentage.ToString("F2") + "%";
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
            environmentFlashFlood = -2;
            economicFlashFlood = -1;
            val = "Flood";
        } else
        {
            environmentFlashFlood = 0.5f;
            val = "No Flood";
        }

        FlashFlood.name = "FlashFlood";
        FlashFlood.value = val;
        FlashFlood.socialScore = socialFlashFlood;
        FlashFlood.economicScore = economicFlashFlood;
        FlashFlood.environmentalScore = environmentFlashFlood;

        return FlashFlood;
    }

    public GameStat UpdateFinalBudgetScore()
    {
        GameStat FinalBudget = new GameStat();
        float socialFinalBudget = 0;
        float economicFinalBudget = 0;
        float environmentFinalBudget = 0;
        float val = ResourceManager.Instance.Budget;

        if (ResourceManager.Instance.Budget < -ResourceManager.Instance.Income)
        {
            economicFinalBudget = -1;
        }
        else
        {
            economicFinalBudget = 1f;
        }

        FinalBudget.name = "Final Budget";
        FinalBudget.value = val.ToString("F2");
        FinalBudget.socialScore = socialFinalBudget;
        FinalBudget.economicScore = economicFinalBudget;
        FinalBudget.environmentalScore = environmentFinalBudget;

        return FinalBudget;
    }

    public GameStat UpdateEmissionScore()
    {
        GameStat Emissions = new GameStat();
        float socialEmission = 0;
        float economicEmission = 0;
        float environmentEmission = 0;
        float val = TrafficManager.Instance.GetTrafficEmissions();

        if (val < minThresholdEmissions)
        {
            environmentEmission = 1;
        }
        else if( val > maxThresholdEmissions)
        {
            environmentEmission = -1;
            socialEmission = -1f;
        }

        Emissions.name = "Emissions";
        Emissions.value = val.ToString("F2") + "kg";
        Emissions.socialScore = socialEmission;
        Emissions.economicScore = economicEmission;
        Emissions.environmentalScore = environmentEmission;

        return Emissions;
    }

    public GameStat UpdateTrafficScore()
    {
        GameStat Traffic = new GameStat();
        float socialTraffic = 0;
        float economicTraffic = 0;
        float environmentTraffic = 0;
        float val = TrafficManager.Instance.GetTrafficIntensityPercentage();

        if (val < maxTrafficIntensityPercentage)
        {
            socialTraffic = 1;
        }
        else
        {
            socialTraffic = -1;
        }

        Traffic.name = "Traffic Intensity";
        Traffic.value = val.ToString("F2") + "%";
        Traffic.socialScore = socialTraffic;
        Traffic.economicScore = economicTraffic;
        Traffic.environmentalScore = environmentTraffic;

        return Traffic;
    }

    public void UpdatePopulationDensity()
    {
        int pop = ResourceManager.Instance.CitizenNumber;
        int spots = ResourceManager.Instance.HostablePeople;
        populationDensity = CalculatePopulationDensity(pop, spots);

        //update UI
        UIManager.Instance.UpdatePopulationDesitySlider(populationDensity, maxPopDensity);
    }


    public float CalculatePopulationDensity(int citizenNumber, int spots)
    {
        return Mathf.Clamp((float)citizenNumber / (float)spots, minPopDensity, maxPopDensity);
    }

    public void UpdateUnemploymentPercentage()
    {
        int jobs = ResourceManager.Instance.Jobs;
        unempolymentPercentage = CalculateUnemploymentPercentage(ResourceManager.Instance.Workers, jobs);
        //update UI
        UIManager.Instance.UpdateUnemploymentSlider(unempolymentPercentage);
    }

    public float CalculateUnemploymentPercentage(int workingPop, int jobs)
    {
        float unemploymentPercentage = 0;
        if (jobs == 0)
        {
            unemploymentPercentage = (unemploymentPercentage + 1f) * 100f;
        } else
        {
            unemploymentPercentage = (((float)workingPop / (float)jobs) - 1f) * 100f;
        }
        //clamp value to keep it into the sliders limits
        return Mathf.Clamp( unemploymentPercentage, minUnemploymentRate, maxUnemploymentRate);
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