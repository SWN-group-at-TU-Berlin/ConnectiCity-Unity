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
        RRscoreTxt.text = rrs.ToString() + "%";
        SSscoreTxt.text = sss.ToString() + "%";
        EscoreTxt.text = es.ToString() + "%";
    }
}
