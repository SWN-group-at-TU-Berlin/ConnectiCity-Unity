using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreRow : MonoBehaviour
{
    [SerializeField] Image Background;
    [SerializeField] Color palette1;
    [SerializeField] Color palette2;
    [SerializeField] bool palette1On;
    #region getter
    public bool Palette1On { get { return palette1On; } }
    #endregion
    [SerializeField] TextMeshProUGUI gameStats;
    [SerializeField] TextMeshProUGUI value;
    [SerializeField] TextMeshProUGUI socialScore;
    [SerializeField] TextMeshProUGUI environmentalScore;
    [SerializeField] TextMeshProUGUI economicScore;

    public void UpdateRowText(string gStat, string val, string sScore, string envScore, string ecoScore, bool pal1On)
    {
        gameStats.text = gStat;
        value.text = val;
        socialScore.text = sScore;
        environmentalScore.text = envScore;
        economicScore.text = ecoScore;

        if (pal1On)
        {
            Background.color = palette1;
            palette1On = true;
        }
        else
        {
            Background.color = palette2;
            palette1On = false;
        }
    }

}
