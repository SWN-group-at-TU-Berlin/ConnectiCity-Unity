using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoTab : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] TextMeshProUGUI SubcatchmentNumber;
    [SerializeField] TextMeshProUGUI InfrastructureType;
    [SerializeField] TextMeshProUGUI BuildignCost_op1;
    [SerializeField] TextMeshProUGUI BuildignCost_op2;
    [SerializeField] TextMeshProUGUI BuildignCost_result;
    [SerializeField] TextMeshProUGUI CurrentBenefit_op1;
    [SerializeField] TextMeshProUGUI BenefitIncrease_op2;
    [SerializeField] TextMeshProUGUI Benefit_result;
    [SerializeField] TextMeshProUGUI BenefitLable;
    [SerializeField] Image BenefitIcon;
    [SerializeField] Image CurrentBenefitIcon;
    [SerializeField] Image IncreaseBenefitIcon;
    [SerializeField] Sprite HostablePeopleBenefitIcon;
    [SerializeField] Sprite JobsBenefitIcon;

    public void UpdateTextFieldsInfrastructure(float subcatchmentNumber, string infrastcutureType, float bc_op1, float bc_op2, float ap_op1, float ap_op2, string benefitLable)
    {
        BenefitLable.text = benefitLable;
        if (benefitLable.Contains("Jobs"))
        {
            BenefitIcon.sprite = JobsBenefitIcon;
            CurrentBenefitIcon.sprite = JobsBenefitIcon;
            IncreaseBenefitIcon.sprite = JobsBenefitIcon;
        } else
        {
            BenefitIcon.sprite = HostablePeopleBenefitIcon;
            CurrentBenefitIcon.sprite = HostablePeopleBenefitIcon;
            IncreaseBenefitIcon.sprite = HostablePeopleBenefitIcon;
        }
        SubcatchmentNumber.text = "" + subcatchmentNumber;
        InfrastructureType.text = infrastcutureType;
        BuildignCost_op1.text = bc_op1.ToString("G30");
        BuildignCost_op2.text = ""+bc_op2;
        float bcResult = bc_op1 - bc_op2;
        BuildignCost_result.text = "" + bcResult;
        CurrentBenefit_op1.text = "" + ap_op1;
        BenefitIncrease_op2.text = "" + ap_op2;
        float apResult = ap_op1 + ap_op2;
        Benefit_result.text = "" + apResult;
    }

    public void UpdateTextFieldsBGI(float subcatchmentNumber, string infrastcutureType, float bc_op1, float bc_op2, Dictionary<int, float> roReduction_before, Dictionary<int, float> roReduction_after)
    {
        SubcatchmentNumber.text = "" + subcatchmentNumber;
        InfrastructureType.text = infrastcutureType;
        BuildignCost_op1.text = bc_op1.ToString("G30");
        BuildignCost_op2.text = "" + bc_op2;
        float bcResult = bc_op1 - bc_op2;
        BuildignCost_result.text = "" + bcResult;
        //APCost_op1.text = "" + ap_op1;
        //APCost_op2.text = "" + ap_op2;
        //float apResult = ap_op1 + ap_op2;
        //APCost_result.text = "" + apResult;
    }
}
