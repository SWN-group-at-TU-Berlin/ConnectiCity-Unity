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
    }
    #endregion

    [Header("Scoring system references")]
    [SerializeField] float maxPopDensity = 3f;
    [SerializeField] float minPopDensity = -3f;
    [SerializeField] float minUnemploymentRate = -100f;
    [SerializeField] float maxUnemploymentRate = 100f;

    [Header("Debug references")]
    [SerializeField] TextMeshProUGUI debugRates;

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

    private void UpdateSocialScore()
    {
        //Pop density update
        //Unemployment percentage update
        //Emissios update
        //Flash flood update
        //Traffic congestion update
    }

    private void UpdateEnvironmentalScore(float score)
    {
        //Unemployment percentage update
        //Flash flood update [TAKES INTO CONSIDERATION BUDGET]
        environmentalScore += score;
    }

    private void UpdateEconomicScore(float score)
    {
        //Emissios update
        //Environmental objectives like green space
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
            "\nWorking pop: "+
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
        if(jobs == 0)
        {
            jobs = 1;
        }
        return (((float)workingPop / (float)jobs) - 1f) * 100f;
    }
}
