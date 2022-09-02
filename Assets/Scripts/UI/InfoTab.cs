using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoTab : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] TextMeshProUGUI SubcatchmentNumber;
    [SerializeField] TextMeshProUGUI InfrastructureType;
    [SerializeField] TextMeshProUGUI BuildignCost_op1;
    [SerializeField] TextMeshProUGUI BuildignCost_op2;
    [SerializeField] TextMeshProUGUI BuildignCost_result;
    [SerializeField] TextMeshProUGUI APCost_op1;
    [SerializeField] TextMeshProUGUI APCost_op2;
    [SerializeField] TextMeshProUGUI APCost_result;

    public void UpdateTextFields(float subcatchmentNumber, string infrastcutureType, float bc_op1, float bc_op2, float ap_op1, float ap_op2)
    {
        SubcatchmentNumber.text = "" + subcatchmentNumber;
        InfrastructureType.text = infrastcutureType;
        BuildignCost_op1.text = bc_op1.ToString("G30");
        BuildignCost_op2.text = ""+bc_op2;
        float bcResult = bc_op1 - bc_op2;
        BuildignCost_result.text = "" + bcResult;
        APCost_op1.text = "" + ap_op1;
        APCost_op2.text = "" + ap_op2;
        float apResult = ap_op1 + ap_op2;
        APCost_result.text = "" + apResult;
    }
}
