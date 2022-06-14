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

    int _citizenNumber;
    #region getter
    public int CitizenNumber { get { return _citizenNumber; } }
    #endregion

    int _citizenSatisfaction;
    #region getter
    public int CitizenSatisfaction { get { return _citizenSatisfaction; } }
    #endregion

    [SerializeField] int _budget;
    #region getter
    public int Budget { get { return _budget; } }
    #endregion

    int _income;
    #region getter
    public int Income { get { return _income; } }
    #endregion

    int _actionPoints;
    #region getter
    public int ActionPoints { get { return _actionPoints; } }
    #endregion

    private void UpdateCitiyenNumber(int valToAdd)
    {
        _citizenNumber += valToAdd;
    }
    private void UpdateCitizenSatisfaction(int valToAdd)
    {
        _citizenSatisfaction += valToAdd;
    }
    private void UpdateIncome(int valToAdd)
    {
        _income += valToAdd;
    }
    private void UpdateBudget(int valToAdd)
    {
        _budget += valToAdd;
    }
    private void UpdateActionPoints(int valToAdd)
    {
        _actionPoints += valToAdd;
    }

}
