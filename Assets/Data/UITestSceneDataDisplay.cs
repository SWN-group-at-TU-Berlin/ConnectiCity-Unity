using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UITestSceneDataDisplay : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI rainEventCosts;
    [SerializeField] TextMeshProUGUI subcatArea;
    [SerializeField] TextMeshProUGUI RunoffReduction;

    [SerializeField] TMP_Dropdown rainEventBuildStatusDropdown;
    [SerializeField] TMP_Dropdown rainEventLvDropdown;
    [SerializeField] TMP_Dropdown rainEventSubcatDropdown;

    [SerializeField] TMP_Dropdown runoffPercentageBuildStatusDropdown;
    [SerializeField] TMP_Dropdown runoffPercentageLvDropdown;
    [SerializeField] TMP_Dropdown runoffPercentageSubcatDropdown;

    [SerializeField] TMP_Dropdown areaSubcatDropdown;


    BuildStatus buildStatus_costs;


    BuildStatus buildStatus_perc;

    private void Start()
    {
        List<TMP_Dropdown.OptionData> buildStatusDropdownOption = new List<TMP_Dropdown.OptionData>();
        List<TMP_Dropdown.OptionData> runoffDropdownOption = new List<TMP_Dropdown.OptionData>();
        foreach (BuildStatus status in Enum.GetValues(typeof(BuildStatus)))
        {
            if(status.ToString().Equals("Unbuild") || status.ToString().Equals("Built"))
            {
                buildStatusDropdownOption.Add(new TMP_Dropdown.OptionData(status.ToString()));
            } else
            {
                buildStatusDropdownOption.Add(new TMP_Dropdown.OptionData(status.ToString()));
                runoffDropdownOption.Add(new TMP_Dropdown.OptionData(status.ToString()));

            }
        }
        rainEventBuildStatusDropdown.AddOptions(buildStatusDropdownOption);
        runoffPercentageBuildStatusDropdown.AddOptions(runoffDropdownOption);
        buildStatus_perc = DataReader.Instance.ConvertStringToStatus(runoffPercentageBuildStatusDropdown.options[0].text);
    }

    public void rainEventCostStatusChange()
    {
        buildStatus_costs = DataReader.Instance.ConvertStringToStatus(rainEventBuildStatusDropdown.options[rainEventBuildStatusDropdown.value].text);
    }


    public void rainEventPercStatusChange()
    {
        buildStatus_perc = DataReader.Instance.ConvertStringToStatus(runoffPercentageBuildStatusDropdown.options[runoffPercentageBuildStatusDropdown.value].text);
    }

    public void GetRainEventCost()
    {
        float cost = DataReader.Instance.RainCostsDictionaries[rainEventLvDropdown.value+1][new SubcatchmentKey(rainEventSubcatDropdown.value+1, buildStatus_costs)];
        rainEventCosts.text = "Rain level: " + (rainEventLvDropdown.value+1) + "\nSubcat: " + (rainEventSubcatDropdown.value+1) + "\nBuild status: " + buildStatus_costs.ToString() + "\nCost = " + cost;
    }

    public void GetRunoffReductionPercentage()
    {
        float runoff = DataReader.Instance.GetRunoffReductionPercentage(runoffPercentageLvDropdown.value+1,new SubcatchmentKey(runoffPercentageSubcatDropdown.value+1, buildStatus_perc));
        RunoffReduction.text = "Rain level: " + (runoffPercentageLvDropdown.value+1) + "\nSubcat: " + (runoffPercentageSubcatDropdown.value+1) + "\nBuild status: " + buildStatus_perc.ToString() + "\nRunoff = " + runoff + "%";
    }

    public void GetSubcatArea()
    {
        float area = DataReader.Instance.SubcatchmentsAreas[areaSubcatDropdown.value+1];
        subcatArea.text = "Subcat number: " + areaSubcatDropdown.value+1 + "\nArea:" + " = " + area;
    }
}
