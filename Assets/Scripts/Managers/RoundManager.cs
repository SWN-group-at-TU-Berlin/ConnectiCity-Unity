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

    [SerializeField] EndGamePanel endGamePanel;

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
                int budget = ResourceManager.Instance.Budget / 5000000;
                //sum budget + income = economic score
                int economicScore = budget + ResourceManager.Instance.Income;
                //calculate avarage runoff reduction for built subcatchments
                Subcatchment[] builtSubcatchment = MapManager.Instance.GetBuiltSubcatchments();
                float avarageWeight = (float)builtSubcatchment.Length / 12f; //weighting with the built subcats over the total buildable subcats
                float runoffReductionPercentageAggregated = 0;
                foreach(Subcatchment subcat in builtSubcatchment)
                {
                    //calculating 
                    float weightedRunoffReductionRain1 = RainEventsManager.Instance.GetRunoffReductionPercentage(subcat, 1) * avarageWeight;
                    float weightedRunoffReductionRain2 = RainEventsManager.Instance.GetRunoffReductionPercentage(subcat, 2) * avarageWeight;
                    float weightedRunoffReductionRain3 = RainEventsManager.Instance.GetRunoffReductionPercentage(subcat, 3) * avarageWeight;
                    runoffReductionPercentageAggregated += weightedRunoffReductionRain1 + weightedRunoffReductionRain1 + weightedRunoffReductionRain3;
                }
                float runoffReductionWeightedMean = runoffReductionPercentageAggregated / (builtSubcatchment.Length * 3);
                float socialScorePercentage = (float)socialScore / 29f;
                float economicScorePercentage = (float)economicScore / 25f;
                //show final score to player
                endGamePanel.gameObject.SetActive(true);
                endGamePanel.SetUpEndPanelStats(runoffReductionWeightedMean, socialScorePercentage, economicScorePercentage);
                Debug.Log("Final score;\n" + "Runoff reduction score: " + runoffReductionWeightedMean + "%\n" + "Social score: " + socialScorePercentage + "%\n" + "Economic Score: " + economicScorePercentage + "%\n");
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
