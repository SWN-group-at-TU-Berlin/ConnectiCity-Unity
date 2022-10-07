using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] GameObject _socialScore; 
    [SerializeField] GameObject _environmentalScore; 
    [SerializeField] GameObject _economicScore;
    [SerializeField] TextMeshProUGUI _totalScore;
    [SerializeField] ScoreGraph scoreGraph;

    float maxRectWith;

    private void Start()
    {
        maxRectWith = _socialScore.GetComponent<RectTransform>().rect.width;
    }

    public void SetUpEndPanelStats(float socialScore, float environmentalScore, float economicScore, float totalScore)
    {
        _socialScore.GetComponentInChildren<TextMeshProUGUI>().text = socialScore.ToString();
        _environmentalScore.GetComponentInChildren<TextMeshProUGUI>().text = environmentalScore.ToString();
        _economicScore.GetComponentInChildren<TextMeshProUGUI>().text = economicScore.ToString();
        _totalScore.text = totalScore.ToString();
        scoreGraph.UpdateGraph(ScoreManager.Instance.TotalScores(), ScoreManager.Instance.MaxTotalPoints);

        float socialScoreRectFactor = socialScore / totalScore;
        float environmentalScoreRectFactor = environmentalScore / totalScore;
        float economicScoreRectFactor = economicScore / totalScore;

        _socialScore.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxRectWith * socialScoreRectFactor);
        _environmentalScore.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxRectWith * environmentalScoreRectFactor);
        _economicScore.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxRectWith * economicScoreRectFactor);
    }
}
