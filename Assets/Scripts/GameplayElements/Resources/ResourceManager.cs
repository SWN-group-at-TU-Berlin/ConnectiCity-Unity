using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager _instance;
    #region singleton
    public static ResourceManager Instance { get { return _instance; } }
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

        defaultAP = _actionPoints;
        InitializeBudgets();
    }
    private void Start()
    {
        PopulationIncreaseInitialization();
    }

    #endregion


    [Header("Social")]

    [SerializeField] int _citizenNumber;
    #region getter
    public int CitizenNumber { get { return _citizenNumber; } }
    #endregion

    [SerializeField] int _finalRoundCitizenIncrease = 370;
    #region getter
    public int FinalRoundCitizenIncrease { get { return _finalRoundCitizenIncrease; } }
    #endregion

    [SerializeField] int _hostablePeople;
    #region getter
    public int HostablePeople { get { return _hostablePeople; } }
    #endregion

    [Header("Economics")]

    [SerializeField] int _budget;
    #region getter
    public int Budget { get { return _budget; } }
    #endregion 

    [SerializeField] int _BGIbudget;
    #region getter
    public int BGIbudget { get { return _BGIbudget; } }
    #endregion 

    [SerializeField, Range(0, 1)] float _BGIbudgetPercentage;
    #region getter
    public float BGIbudgetPercentage { get { return _BGIbudgetPercentage; } }
    #endregion 

    [SerializeField] int _jobs;
    #region getter
    public int Jobs { get { return _jobs; } }
    #endregion

    [SerializeField] float _workingPopulationRate = 0.54f;
    #region getter
    public float WorkingPopulationRate { get { return _workingPopulationRate; } }
    #endregion

    [SerializeField] int _avarageIncomePerPerson;
    #region getter
    public int AvarageIncomePerPerson { get { return _avarageIncomePerPerson; } }
    #endregion

    [SerializeField] float _taxationRate;
    #region getter
    public float TaxationRate { get { return _taxationRate; } }
    #endregion

    [SerializeField] float _totalBudgetRate;
    #region getter
    public float TotalBudgetRate { get { return _totalBudgetRate; } }
    #endregion

    [SerializeField] int _income;
    #region getter
    public int Income { get { return _income; } }
    #endregion




    //DEPRECATED
    int _citizenSatisfaction;
    #region getter
    public int CitizenSatisfaction { get { return _citizenSatisfaction; } }
    #endregion

    //DEPRECATED
    int _maxCitizenSatisfaction;
    #region getter
    public int MaxCitizenSatisfaction { get { return _maxCitizenSatisfaction; } }
    #endregion

    //DEPRECATED
    int _actionPoints;
    #region getter
    public int ActionPoints { get { return _actionPoints; } }
    #endregion


    int defaultAP;

    Dictionary<int, int> populationIncreasePerRound;

    private void InitializeBudgets()
    {
        _BGIbudget = (int)((int)Budget * BGIbudgetPercentage);
        _budget -= _BGIbudget;
    }

    public void UpdateCitizenNumber(int valToAdd)
    {
        _citizenNumber += valToAdd * 1000;
        UIManager.Instance.UpdateCitizenNumberTxt(CitizenNumber);
    }
    public void UpdateCitizenSatisfaction(int valToAdd)
    {
        if (_citizenSatisfaction < _maxCitizenSatisfaction || _citizenSatisfaction >= 0)
        {
            _citizenSatisfaction += valToAdd;
            _citizenSatisfaction = (int)Mathf.Clamp(_citizenSatisfaction, 0f, _maxCitizenSatisfaction);
            UIManager.Instance.UpdateCitizenSatisfactionTxt(CitizenSatisfaction);
        }
    }
    public void UpdateIncome()
    {
        /*if (_income >= 0)
        {
            _income += valToAdd;
            UIManager.Instance.UpdateIncomeTxt(Income);
        }*/
        _income = (int)((int)((GetWorkingPopulation() * AvarageIncomePerPerson)*TaxationRate) * TotalBudgetRate);
        UIManager.Instance.UpdateIncomeTxt(Income);
    }
    public void UpdateBudgetsAtEndRound()
    {
        _BGIbudget += (int)(_income * BGIbudgetPercentage);
        _budget += (Income - (int)(_income * BGIbudgetPercentage));
    }
    public void UpdateBudgetAtRoundStart()
    {
        UpdateBudget(Income * 1000000);
    }
    public void UpdateBudget(int valToAdd)
    {
        _budget -= valToAdd;
        UIManager.Instance.UpdateBudgetTxt(Budget);
    }

    public void UpdateJobs(int valToAdd)
    {
        _jobs += valToAdd;
        //update jobs UI
        ScoreManager.Instance.UpdateUnemploymentPercentage();
    }

    public void UpdateHostablePeople(int valToAdd)
    {
        _hostablePeople += valToAdd;
        //update hostable ppl UI
        ScoreManager.Instance.UpdatePopulationDensity();
        ScoreManager.Instance.UpdateUnemploymentPercentage();
    }

    public void UpdateActionPoints(int valToAdd)
    {
        _actionPoints += valToAdd;
        UIManager.Instance.UpdateActionPointsTxt(ActionPoints);
    }

    public void ResetActionPoints()
    {
        _actionPoints = defaultAP;
        UIManager.Instance.UpdateActionPointsTxt(ActionPoints);

    }

    private void PopulationIncreaseInitialization()
    {
        populationIncreasePerRound = new Dictionary<int, int>();
        int maxRounds = RoundManager.Instance.MaxRounds;
        int round = maxRounds;
        for (; round >= 0; round--)
        {
            int denominator = maxRounds + 1 - round;
            int popIncrease = Mathf.CeilToInt(FinalRoundCitizenIncrease / denominator);
            populationIncreasePerRound.Add(round, popIncrease);
        }
    }

    public void IncreaseCitizens()
    {
        _citizenNumber += populationIncreasePerRound[RoundManager.Instance.CurrentRound];
    }

    public int GetWorkingPopulation()
    {
        return Mathf.CeilToInt(_citizenNumber * WorkingPopulationRate);
    }
}
