using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCardStats
{
    [SerializeField] string _name;
    #region getter
    public string Name { get { return _name; } }
    #endregion
    [SerializeField] int _cActionPoints;
    #region getter
    public int CActionPoints { get { return _cActionPoints; } set { _cActionPoints = value; } }
    #endregion
}

[System.Serializable]
public class BasicInfrastructureStats : BasicCardStats
{
    [SerializeField] int _cBudgetSmall;
    #region getter
    public int CBudgetSmall { get { return _cBudgetSmall; } }
    #endregion
    [SerializeField] int _cBudgetMedium;
    #region getter
    public int CBudgetMedium { get { return _cBudgetMedium; } }
    #endregion
    [SerializeField] int _cBudgetLarge;
    #region getter
    public int CBudgetLarge { get { return _cBudgetLarge; } }
    #endregion
    [SerializeField] int _bCitizenNumberSmall;
    #region getter
    public int BCitizenNumberSmall { get { return _bCitizenNumberSmall; } }
    #endregion
    [SerializeField] int _bCitizenNumberMedium;
    #region getter
    public int BCitizenNumberMedium { get { return _bCitizenNumberMedium; } }
    #endregion
    [SerializeField] int _bCitizenNumberLarge;
    #region getter
    public int BCitizenNumberLarge { get { return _bCitizenNumberLarge; } }
    #endregion
    [SerializeField] int _bCitizenSatisfaction;
    #region getter
    public int BCitizenSatisfaction { get { return _bCitizenSatisfaction; } }
    #endregion
    [SerializeField] int _bIncomeSmall;
    #region getter
    public int BIncomeSmall { get { return _bIncomeSmall; } }
    #endregion
    [SerializeField] int _bIncomeMedium;
    #region getter
    public int BIncomeMedium { get { return _bIncomeMedium; } }
    #endregion
    [SerializeField] int _bIncomeLarge;
    #region getter
    public int BIncomeLarge { get { return _bIncomeLarge; } }
    #endregion
}

[System.Serializable]
public class BasicBGIStats : BasicCardStats
{
    [SerializeField] int _cBudgetResidentialSmall;
    #region getter
    public int CBudgetResidentialSmall { get { return _cBudgetResidentialSmall; } }
    #endregion
    [SerializeField] int _cBudgetResidentialMedium;
    #region getter
    public int CBudgetResidentialMedium { get { return _cBudgetResidentialMedium; } }
    #endregion
    [SerializeField] int _cBudgetResidentialLarge;
    #region getter
    public int CBudgetResidentialLarge { get { return _cBudgetResidentialLarge; } }
    #endregion
    [SerializeField] int _cBudgetCommercialSmall;
    #region getter
    public int CBudgetCommercialSmall { get { return _cBudgetCommercialSmall; } }
    #endregion
    [SerializeField] int _cBudgetCommercialMedium;
    #region getter
    public int CBudgetCommercialMedium { get { return _cBudgetCommercialMedium; } }
    #endregion
    [SerializeField] int _cBudgetCommercialLarge;
    #region getter
    public int CBudgetCommercialLarge { get { return _cBudgetCommercialLarge; } }
    #endregion
    [SerializeField] int _bCitizenSatisfaction;
    #region getter
    public int BCitizenSatisfaction { get { return _bCitizenSatisfaction; } }
    #endregion
}