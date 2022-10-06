using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSummary : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI social;
    [SerializeField] TextMeshProUGUI env;
    [SerializeField] TextMeshProUGUI eco;


    private void Start()
    {
        social.text = "0";
        env.text = "0";
        eco.text = "0";
    }

    public void UpdateSummaryScore(string socialScore, string envScore, string ecoScore)
    {
        social.text = socialScore;
        env.text = envScore;
        eco.text = ecoScore;
    }
}
