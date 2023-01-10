using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RainInfoTab : Tab
{
    [SerializeField] TextMeshProUGUI ResultRunoffReductionLv1;
    [SerializeField] TextMeshProUGUI ResultRunoffReductionLv2;
    [SerializeField] TextMeshProUGUI ResultRunoffReductionLv3     ;
    [SerializeField] TextMeshProUGUI SubcatchmentNumber;
    [SerializeField] TextMeshProUGUI BGI1;
    [SerializeField] TextMeshProUGUI BGI2;
    public void UpdateRainTextFields(float runoffLv1, float runoffLv2, float runoffLv3, int subcatNumber, string bgi1 = "No BGI", string bgi2 = "No BGI")
    {
        PlayOpenTab(true);
        ResultRunoffReductionLv1.text = runoffLv1 + "%";
        ResultRunoffReductionLv2.text = runoffLv2 + "%";
        ResultRunoffReductionLv3.text = runoffLv3 + "%";
        SubcatchmentNumber.text = subcatNumber.ToString();
        BGI1.text = bgi1;
        BGI2.text = bgi2;
    }
}
