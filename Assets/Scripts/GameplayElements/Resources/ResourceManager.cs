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

    [SerializeField] int _citizenNumber;
    #region getter
    public int CitizenNumber { get { return _citizenNumber; } }
    #endregion

    [SerializeField] int _citizenSatisfaction;
    #region getter
    public int CitizenSatisfaction { get { return _citizenSatisfaction; } }
    #endregion

    [SerializeField] int _maxCitizenSatisfaction;
    #region getter
    public int MaxCitizenSatisfaction { get { return _maxCitizenSatisfaction; } }
    #endregion

    [SerializeField] int _budget;
    #region getter
    public int Budget { get { return _budget; } }
    #endregion

    [SerializeField] int _income;
    #region getter
    public int Income { get { return _income; } }
    #endregion

    [SerializeField] int _actionPoints;
    #region getter
    public int ActionPoints { get { return _actionPoints; } }
    #endregion

    int defaultAP;

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
    public void UpdateIncome(int valToAdd)
    {
        if (_income >= 0)
        {
            _income += valToAdd;
            UIManager.Instance.UpdateIncomeTxt(Income);
        }
    }
    public void UpdateBudgetAtRoundStart()
    {
        UpdateBudget(Income * 1000000);
    }
    public void UpdateBudget(int valToAdd)
    {
        _budget += valToAdd;
        UIManager.Instance.UpdateBudgetTxt(Budget);
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

}
