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
        UIManager.Instance.UpdateRoundTxt(CurrentRound);
    }

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
        if (!Phase.Equals(RoundPhase.RainEvent))
        {
            if (_currentRound < 10)
            {
                _currentRound++;
                _phase = RoundPhase.RainEvent;
                MapManager.Instance.ReactivateAllSubcatchments();
                StartCoroutine(RainEventsManager.Instance.RainEvent());
            }
            else
            {
                //calculate final score
                //take current citizen satisfaction
                int citizenSatisfaction = ResourceManager.Instance.CitizenSatisfaction;
                //take current citizen number / 10000
                int citizenNumberScore = ResourceManager.Instance.CitizenNumber / 10000;
                //sum citizen number + citizen satisfaction = social score
                int socialScore = citizenSatisfaction + citizenNumberScore;
                //take current budget/5000000

                //sum budget + income = economic score
                //show how many bgis are built 
                //show final score to player
                //end the game or replay
            }
        }
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
                UIManager.Instance.UpdateRoundTxt(_currentRound);
            }
        }
    }

}
