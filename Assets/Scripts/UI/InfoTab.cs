using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoTab : MonoBehaviour
{
    [Header("Text Fields")]
    [SerializeField] GameObject BGIContainer;
    [SerializeField] GameObject InfrastructuresContainer;
    [SerializeField] protected TextMeshProUGUI SubcatchmentNumber;
    [SerializeField] protected TextMeshProUGUI InfrastructureType;
    [SerializeField] TextMeshProUGUI BuildignCost_op1;
    [SerializeField] TextMeshProUGUI BuildignCost_op2;
    [SerializeField] TextMeshProUGUI BuildignCost_result;
    [SerializeField] TextMeshProUGUI CurrentBenefit_op1;
    [SerializeField] TextMeshProUGUI BenefitIncrease_op2;
    [SerializeField] TextMeshProUGUI Benefit_result;
    [SerializeField] TextMeshProUGUI InfrastructureBenefitLabel;
    [SerializeField] protected TextMeshProUGUI BGIBenefitLabel;
    [SerializeField] TextMeshProUGUI NewRunoffReductionLv1;
    [SerializeField] TextMeshProUGUI NewRunoffReductionLv2;
    [SerializeField] TextMeshProUGUI NewRunoffReductionLv3;
    [SerializeField] TextMeshProUGUI CurrentRunoffReductionLv1;
    [SerializeField] TextMeshProUGUI CurrentRunoffReductionLv2;
    [SerializeField] TextMeshProUGUI CurrentRunoffReductionLv3;
    [SerializeField] protected TextMeshProUGUI ResultRunoffReductionLv1;
    [SerializeField] protected TextMeshProUGUI ResultRunoffReductionLv2;
    [SerializeField] protected TextMeshProUGUI ResultRunoffReductionLv3;
    [SerializeField] Image BenefitIcon;
    [SerializeField] Image CurrentBenefitIcon;
    [SerializeField] Image IncreaseBenefitIcon;
    [SerializeField] GameObject SeparatorInfrastructure;
    [SerializeField] GameObject SeparatorBGI;
    [SerializeField] Sprite HostablePeopleBenefitIcon;
    [SerializeField] Sprite JobsBenefitIcon;
    [SerializeField] string BenefitLabelStaticString = "runoff reduction:";

    public void UpdateTextFieldsInfrastructure(float subcatchmentNumber, string infrastcutureType, float bc_op1, float bc_op2, float ap_op1, float ap_op2, string benefitLable)
    {
        BGIContainer.SetActive(false);
        SeparatorBGI.SetActive(false);
        InfrastructuresContainer.SetActive(true);
        SeparatorInfrastructure.SetActive(true);
        InfrastructureBenefitLabel.text = benefitLable;
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

    public void UpdateTextFieldsBGI(
        float subcatchmentNumber, 
        string infrastcutureType, 
        float bc_op1, 
        float bc_op2, 
        Dictionary<int, float> roReduction_before, 
        Dictionary<int, float> roReduction_after)
    {
        //Show BGIContainer and Hide InfrastructureContainer
        BGIContainer.SetActive(true);
        SeparatorBGI.SetActive(true);
        InfrastructuresContainer.SetActive(false);
        SeparatorInfrastructure.SetActive(false);

        //Change BenefitLabel to "Specific BGI runoff reduction:"
        BGIBenefitLabel.text = BenefitLabelStaticString;

        //New runoff reduction rain lv1
        NewRunoffReductionLv1.text = roReduction_after[1] + "%";
        //New runoff reduction rain lv2
        NewRunoffReductionLv2.text = roReduction_after[2] + "%";
        //New runoff reduction rain lv3
        NewRunoffReductionLv3.text = roReduction_after[3] + "%";

        //Current runoff reduction rain lv1
        CurrentRunoffReductionLv1.text = roReduction_before[1] + "%";
        //Current runoff reduction rain lv2
        CurrentRunoffReductionLv2.text = roReduction_before[2] + "%";
        //Current runoff reduction rain lv3
        CurrentRunoffReductionLv3.text = roReduction_before[3] + "%";

        //Result of runoff reduction lv1
        ResultRunoffReductionLv1.text = (roReduction_after[1] + roReduction_before[1]) + "%";
        //Result of runoff reduction lv2
        ResultRunoffReductionLv2.text = (roReduction_after[2] + roReduction_before[2]) + "%";
        //Result of runoff reduction lv3
        ResultRunoffReductionLv3.text = (roReduction_after[3] + roReduction_before[3]) + "%";

        SubcatchmentNumber.text = "" + subcatchmentNumber;
        InfrastructureType.text = infrastcutureType;
        BuildignCost_op1.text = bc_op1.ToString("G30");
        BuildignCost_op2.text = "" + bc_op2;
        float bcResult = bc_op1 - bc_op2;
        BuildignCost_result.text = "" + bcResult;
    }
}


