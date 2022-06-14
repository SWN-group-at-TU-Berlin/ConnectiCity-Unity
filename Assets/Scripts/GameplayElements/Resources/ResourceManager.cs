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

    public void UpdateCitiyenNumber(int valToAdd)
    {
        _citizenNumber += valToAdd;
        UIManager.Instance.UpdateCitizenNumberTxt(CitizenNumber);
    }
    public void UpdateCitizenSatisfaction(int valToAdd)
    {
        _citizenSatisfaction += valToAdd;
        UIManager.Instance.UpdateCitizenSatisfactionTxt(CitizenSatisfaction);
    }
    public void UpdateIncome(int valToAdd)
    {
        _income += valToAdd;
        UIManager.Instance.UpdateIncomeTxt(Income);
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

}
