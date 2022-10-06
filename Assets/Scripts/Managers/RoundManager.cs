using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public static RoundManager _instance;
    #region singleton
    public static RoundManager Instance { get { return _instance; } }
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

        _currentRound = 1;
    }
    #endregion

    private void Start()
    {
        //UIManager.Instance.UpdateRoundTxt(CurrentRound);
    }

    [SerializeField] EndGamePanel endGamePanel;

    [SerializeField] int maxRounds = 10;
    #region getter
    public int MaxRounds { get { return maxRounds; } }
    #endregion

    int _currentRound;
    #region getter
    public int CurrentRound { get { return _currentRound; } }
    #endregion

    private RoundPhase _phase;
    #region getters
    public RoundPhase Phase { get { return _phase; } }
    #endregion

    public void NextRound()
    {
        _currentRound++;
        ResourceManager.Instance.UpdateBudgetsAtEndRound();
        ResourceManager.Instance.IncreaseCitizens();
    }
    public void StartRound()
    {
        if (!Phase.Equals(RoundPhase.Playing))
        {
            _phase = RoundPhase.Playing;
            ResourceManager.Instance.ResetActionPoints();
            ResourceManager.Instance.UpdateBudgetAtRoundStart();
            if (_currentRound < 10)
            {
               // UIManager.Instance.UpdateRoundTxt(_currentRound);
            }
        }
    }

}
