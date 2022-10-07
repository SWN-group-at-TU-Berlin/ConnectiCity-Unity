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
    }

    #endregion


    [Header("Social")]

    [SerializeField] int _citizenNumber;
    #region getter
    public int CitizenNumber { get { return _citizenNumber; } }
    #endregion

    [SerializeField] int _workers;
    #region getter
    public int Workers { get { return _workers; } }
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

    [SerializeField] int _baseIncome;//basic income to add to the one produced by workers
    #region getter
    public int BasicIncome { get { return _baseIncome; } }
    #endregion

    int _income;
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
    private void Start()
    {
        PopulationIncreaseInitialization();
        InitializeBudgets();
        UIManager.Instance.UpdateJobsAvailableTxt(_jobs.ToString());
    }

    private void InitializeBudgets()
    {
        _BGIbudget = (int)((int)Budget * BGIbudgetPercentage);
        _budget -= _BGIbudget;
        UIManager.Instance.UpdateBGIsBudgetTxt(_BGIbudget.ToString());
        UIManager.Instance.UpdateGeneralBudgetTxt(_budget.ToString());
    }

    public void UpdateCitizenNumber(int valToAdd)
    {
        _citizenNumber += valToAdd;
    }

    //DEPRECATED
    public void UpdateCitizenSatisfaction(int valToAdd)
    {
        if (_citizenSatisfaction < _maxCitizenSatisfaction || _citizenSatisfaction >= 0)
        {
            _citizenSatisfaction += valToAdd;
            _citizenSatisfaction = (int)Mathf.Clamp(_citizenSatisfaction, 0f, _maxCitizenSatisfaction);
            // UIManager.Instance.UpdateCitizenSatisfactionTxt(CitizenSatisfaction);
        }
    }

    public void UpdateIncome()
    {
        _income = (int)((int)((GetWorkingPopulation() * AvarageIncomePerPerson) * TaxationRate) * TotalBudgetRate);
        int bgiIncome = (int)(_income * BGIbudgetPercentage);
        int generalIncome = (_income - (int)(_income * BGIbudgetPercentage));
        UIManager.Instance.UpdateGeneralIncreaseTxt(generalIncome.ToString());
        UIManager.Instance.UpdateBGIsIncreaseTxt(bgiIncome.ToString());
    }
    public void UpdateBudgetsAtEndRound()
    {
        _BGIbudget += (int)((_income + _baseIncome) * BGIbudgetPercentage);
        _budget += (_income - (int)((_income + _baseIncome) * BGIbudgetPercentage));
        UIManager.Instance.UpdateBGIsBudgetTxt(_BGIbudget.ToString());
        UIManager.Instance.UpdateGeneralBudgetTxt(_budget.ToString());
    }
    public void UpdateBudgetAtRoundStart()
    {
        UpdateBudget(Income * 1000000);
    }
    public void UpdateBudget(int valToAdd)
    {
        _budget -= valToAdd;
        UIManager.Instance.UpdateGeneralBudgetTxt(_budget.ToString("F2"));
    }

    public void UpdateBGIBudget(int valToAdd)
    {
        _BGIbudget -= valToAdd;
        UIManager.Instance.UpdateBGIsBudgetTxt(_BGIbudget.ToString("F2"));
    }

    public void UpdateJobs(int valToAdd)
    {
        _jobs += valToAdd;
        //update jobs UI
        ScoreManager.Instance.UpdateUnemploymentPercentage();
        UIManager.Instance.UpdateJobsAvailableTxt(_jobs.ToString());
    }

    public void UpdateHostablePeople(int valToAdd)
    {
        _hostablePeople += valToAdd;
        //update hostable ppl UI
        ScoreManager.Instance.UpdatePopulationDensity();
        ScoreManager.Instance.UpdateUnemploymentPercentage();
        UIManager.Instance.UpdateHostablePeopleTxt(_hostablePeople.ToString());
    }

    public void UpdateActionPoints(int valToAdd)
    {
        _actionPoints += valToAdd;
        //UIManager.Instance.UpdateActionPointsTxt(ActionPoints);
    }

    public void ResetActionPoints()
    {
        _actionPoints = defaultAP;
        // UIManager.Instance.UpdateActionPointsTxt(ActionPoints);

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
        _workers = (int)(_citizenNumber * WorkingPopulationRate);
        UIManager.Instance.UpdateCitizensTxt(_citizenNumber.ToString());
        UIManager.Instance.UpdateWorkersTxt(_workers.ToString());
        UIManager.Instance.UpdateCitizensGrowthTxt(populationIncreasePerRound[RoundManager.Instance.CurrentRound].ToString());
        UIManager.Instance.UpdateWorkersGrowthTxt(GetWorkingPopulationIncrease().ToString());
    }

    public int GetWorkingPopulationIncrease()
    {
        int workingPopIncrease = (int)(populationIncreasePerRound[RoundManager.Instance.CurrentRound] * WorkingPopulationRate);
        return workingPopIncrease;
    }

    public void IncreaseCitizens()
    {
        _citizenNumber += populationIncreasePerRound[RoundManager.Instance.CurrentRound];
        _workers = (int)(_citizenNumber * WorkingPopulationRate);
        UIManager.Instance.UpdateCitizensTxt(_citizenNumber.ToString());
        UIManager.Instance.UpdateWorkersTxt(_workers.ToString());
        UIManager.Instance.UpdateCitizensGrowthTxt(populationIncreasePerRound[RoundManager.Instance.CurrentRound].ToString());
        UIManager.Instance.UpdateWorkersGrowthTxt(GetWorkingPopulationIncrease().ToString());
    }

    public int GetWorkingPopulation()
    {
        int peopleThatHaveAJob = 0;
        if (_jobs - _workers < 0)
        {
            peopleThatHaveAJob = _jobs;
        }
        else
        {
            peopleThatHaveAJob = _workers;
        }
        return peopleThatHaveAJob;
    }
}
