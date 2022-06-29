using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI RRscoreTxt; 
    [SerializeField] TextMeshProUGUI SSscoreTxt; 
    [SerializeField] TextMeshProUGUI EscoreTxt;
    
    public void SetUpEndPanelStats(float rrs, float sss, float es)
    {
        RRscoreTxt.text = rrs.ToString("0.0") + "%";
        SSscoreTxt.text = sss.ToString("0.0") + "%";
        EscoreTxt.text = es.ToString("0.0") + "%";
    }
}
